Battle Manager
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using System;

public class BattleManager : MonoBehaviour
{
    public GameObject battleWindow;
    public Image enemyImage;
    public TextMeshProUGUI enemyNameText;
    public Image backGroundImage;

    [Header("Player/Enemy Settings")]
    public Player player;
    public Image battleCharacterImage;
    public Image battleWeaponImage;
    public Image battleTopImage;
    public Image battleBottomImage;
    public GameObject playerObject;
    public EnemyScript currentEnemy;
    public GameObject enemyObject;
    public List<Button> skillButtons;
    private Inventory inventory;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;

    public Camera mainCamera;
    private bool isBattleActive = true;

    private void Start()
    {
        inventory = Inventory.Instance ?? throw new NullReferenceException("Inventory.Instance가 null입니다.");
        player = playerObject.GetComponent<Player>() ?? Player.Instance;
        Player.Instance.OnCharacterUpdated += UpdateBattleCharacterImage;
        currentEnemy = enemyObject.GetComponent<EnemyScript>();

        if(player == null)
        {
            Debug.LogError("player 오브젝트를 찾을 수 없습니다");
        }

        if(currentEnemy == null)
        {
            Debug.LogError("enemyObject를 찾을 수 없습니다");
        }

        Debug.Log("Inventory 인스턴스 상태: " + (inventory != null ? "정상" : "null"));
        InitialzeSkillButtons();
        UpdateBattleState();

        //플레이어 죽는 이벤트 연결
        player.OnPlayerDeath += HandlePlayerDeath; 
    }

    private void UpdateBattleCharacterImage()
    {
        if(battleCharacterImage != null)
        {
            battleCharacterImage.sprite = Player.Instance.GetCompositeCharacterImage();

            if(Player.Instance.equippedWeapon != null)
            {
                battleWeaponImage.sprite = Player.Instance.equippedWeapon.itemSprite;
                battleWeaponImage.enabled = true;
                battleWeaponImage.gameObject.SetActive(true);
            }
            else if(battleWeaponImage != null)
            {
                battleWeaponImage.sprite = null;
                battleWeaponImage.enabled = false;
                battleWeaponImage.gameObject.SetActive(false);
            }
            if(Player.Instance.equippedTop != null)
            {
                battleTopImage.sprite = Player.Instance.baseCharacterSprite;
                battleTopImage.enabled = true;
                battleTopImage.gameObject.SetActive(true);
            }
            else if(battleTopImage != null)
            {
                battleTopImage.sprite = null;
                battleTopImage.enabled = false;
                battleTopImage.gameObject.SetActive(false);
            }
            if(Player.Instance.equippedBottom != null)
            {
                battleBottomImage.sprite = Player.Instance.baseCharacterSprite;
                battleBottomImage.enabled = true;
                battleBottomImage.gameObject.SetActive(true);
            }
            else if(battleBottomImage != null)
            {
                battleBottomImage.sprite = null;
                battleBottomImage.enabled = false;
                battleBottomImage.gameObject.SetActive(false);
            }
        }
    }

    private void InitialzeSkillButtons()
    {
        if(player == null)
        {
            Debug.LogError("Player 인스턴스가 null입니다");
            return;
        }

        List<Skill> battleSkills = player.GetBattleSkills() ?? new List<Skill>();

        for(int i = 0; i < skillButtons.Count; i++)
        {
            Transform imageTransform = skillButtons[i].transform.Find("Image");
            Image skillIconImage = imageTransform.GetComponent<Image>();
            if(i < battleSkills.Count && battleSkills[i] != null)
            {
                skillIconImage.sprite = battleSkills[i].skillIcon;
                skillIconImage.enabled = true; //이미지 활성화
                int skillIndex = i;
                skillButtons[i].onClick.RemoveAllListeners();
                skillButtons[i].onClick.AddListener(() => UseSkill(skillIndex));
            }
            else
            {
                skillIconImage.sprite = null;
                skillIconImage.enabled = false; //이미지 비활성화 
                skillButtons[i].onClick.RemoveAllListeners();
            }
        } 
    }

    private IEnumerator BattleRoutine()
    {
        while(isBattleActive)
        {
            //적 턴
            yield return StartCoroutine(EnemyTurn());

            if(!isBattleActive) break;

            yield return StartCoroutine(PlayerTurn());
        }

        EndBattle();
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log($"{currentEnemy.enemyData.enemyName}의 턴 시작");

        yield return StartCoroutine(ShowActionWithCameraZoom(() =>
        {
            currentEnemy.UseSkill(player, this);
        }));

        UpdateBattleState();

        if(player.currentHP <= 0)
        {
            isBattleActive = false;
            EndBattle();
        }

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PlayerTurn()
    {
        Debug.Log("player턴 시작");

        bool isActionCompleted = false;

        //스킬 버튼 활성호 및 클릭 대기
        for(int i = 0; i < skillButtons.Count; i++)
        {
            int skillIndex = i;
            skillButtons[i].onClick.RemoveAllListeners();
            skillButtons[i].onClick.AddListener(() =>
            {
                UseSkill(skillIndex);
                isActionCompleted = true;
            });
        }

        yield return new WaitUntil(() => isActionCompleted);

        UpdateBattleState();

        if(currentEnemy.currentHP <= 0)
        {
            isBattleActive = false;
        }

        yield return new WaitForSeconds(1f); //플레이어 턴이 끝난 후 짧은 대기 
    }

    public void UseSkill(int skillIndex)
    {
        List<Skill> battleSkills = player.GetBattleSkills();
        if(skillIndex < 0 || skillIndex >= battleSkills.Count || battleSkills[skillIndex] == null)
        {
            Debug.LogError("유효하지 않은 스킬 슬롯");
            return;
        }

        Skill skill = battleSkills[skillIndex];
        StartCoroutine(ShowActionWithCameraZoom(() =>
        {
            skill.ExecuteSkill(player.gameObject, currentEnemy.gameObject, this);
        }));
        
        if(currentEnemy.gameObject == null)
        {
            Debug.LogError("적 오브젝트가 없습니다");
        }


        UpdateBattleState();

        if(currentEnemy.currentHP <= 0)
        {
            currentEnemy.Die(); //적 죽음 처리
            isBattleActive = false;
            EndBattle();
        }
    }

    private IEnumerator ShowActionWithCameraZoom(System.Action action)
    {
        float originalSize = mainCamera.orthographicSize;
        float zoomedSize = originalSize * 2;
        float duration = 0.5f; //카메라 확대 축소 시간
        float elapsedTime = 0;

        //카메라 확대
        while(elapsedTime < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(originalSize, zoomedSize, elapsedTime * duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = zoomedSize;

        action?.Invoke();
        yield return new WaitForSecondsRealtime(1.0f);

        //카메라 복원
        elapsedTime = 0;
        while(elapsedTime < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(zoomedSize, originalSize, elapsedTime * duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mainCamera.orthographicSize = originalSize;
    }

    private void UpdateBattleState()
    {
        if(player == null)
        {
            Debug.LogError("플레이어 인스턴스가 null입니다");
        }
        if(currentEnemy == null)
        {
            Debug.LogError("currentEnemy가 null입니다");
        }
        if(playerHPText != null)
        {
            playerHPText.text = $"{player.currentHP}/{player.maxHP}";
        }
        else
        {
            Debug.LogError("playerHPText가 설정되지 않았습니다");
        }
        if(enemyHPText != null)
        {
            enemyHPText.text = $"{currentEnemy.currentHP}/{currentEnemy.maxHP}";
        }
        else
        {
            Debug.LogError("enemyHPText가 설정되지 않았습니다");
        }
    }

    public void StartBattle(EnemyData enemyData, string backGroundName)
    {
        currentEnemy = enemyObject.GetComponent<EnemyScript>();
        if (currentEnemy == null)
        {
            Debug.LogError("EnemyObject에 EnemyScript가 없습니다.");
            return;
        }
        
        if(enemyData != null)
        {
            currentEnemy.InitializeEnemy(enemyData);
            
            enemyNameText.text = enemyData.enemyName; //적 이름 설정
            enemyImage.sprite = enemyData.enemySprite; //적 이미지 설정

            SpriteRenderer enemyRenderer = enemyObject.GetComponentInChildren<SpriteRenderer>();
            if(enemyRenderer != null)
            {
                enemyRenderer.sprite = enemyData.enemySprite; //적 유닛 스프라이트
            }
            else
            {
                Debug.LogError("적 오브젝트에 SpriteRenderer가 없습니다");
            }
        }
        else
        {
            Debug.LogError("적 데이터가 null입니다. 초기화를 중단합니다.");
            return;
        }

        //배경 설정
        Sprite backGroundSprite = Resources.Load<Sprite>($"Battle Background/{backGroundName}");
        if(backGroundSprite != null)
        {
            backGroundImage.sprite = backGroundSprite;
        }
        else
        {
            Debug.LogError($"{backGroundName}을 찾을 수 없습니다");
        }

        battleWindow.SetActive(true);
        UpdateBattleCharacterImage();
        UpdateBattleState();
        InitialzeSkillButtons();
        StartCoroutine(BattleRoutine());
    }

    private void HandlePlayerDeath()
    {
        PlayPlayerDeathAnimation();
    }
    
    private void PlayPlayerDeathAnimation()
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        if(playerAnimator != null)
        {
            playerAnimator.Play("PlayerDeath");
        }
    }

    public void EndBattle()
    {
        Debug.Log("전투 종료");
        if(player.currentHP <= 0)
        {
            Debug.Log($"{currentEnemy.enemyData.enemyName} 승리");
        }
        else if(currentEnemy.currentHP <= 0)
        {
            Debug.Log($"player 승리");
        }
        battleWindow.SetActive(false);
    }
}
~~~

Battle UI Manager
---
~~~C#
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public GameObject battleResultWindow;

    public void UpdateHP(int playerHP, int enemyHP)
    {  
        playerHPText.text = $"Player HP: {playerHP}";
        enemyHPText.text = $"Enemy HP: {enemyHP}";
    }

    public void ShowPlayerTurn()
    {
        Debug.Log("플레이어 턴 시작");
    }

    public void ShowEnemyTurn()
    {
        Debug.Log("적 턴 시작");
    }
    
    public void ShowBattleResult(bool isPlayerWin)
    {
        battleResultWindow.SetActive(true);
    }
}
~~~

Battle Action Manager
---
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionManager : MonoBehaviour
{
    private BattleStateManager stateManager;

    public IEnumerator ExecutePlayerAction()
    {
        yield return new WaitUntill( stateManager.IsPlayerActionCompleted);
    }

    public IEnumerator ExecuteEnemyAction()
    {
        yield return new WaitForSeconds(1f);
    }
}
~~~

Battle State Manager
---
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateManager : MonoBehaviour
{
    public int playerHP;
    public int enemyHP;
    public bool IsBattleOver => playerHP <= 0 || enemyHP <= 0;
    public bool IsPlayerWin => playerHP > 0;

    public void ApplyDamage(bool toPlayer, int damage)
    {
        if(toPlayer)
        {
            playerHP -= damage;
        }
        else
        {
            enemyHP -= damage;
        }
    }
}
~~~

기본적인 전투 흐름
---
~~~C#
1. 전투 시작 (`BattleManager.StartBattle`):
   - 플레이어와 적 데이터를 초기화.
   - 전투 UI 활성화.
   - `BattleRoutine`으로 진입.

2. 턴 진행 (`BattleRoutine`):
   - 플레이어 턴 시작:
     - 스킬 버튼 활성화, 행동 대기.
   - 적 턴 시작:
     - 스킬 선택 후 실행.
   - 전투 종료 조건 확인.

3. 전투 종료:
   - 승리/패배 결과 UI 표시.
   - 전투 종료 후 정리 작업 수행.

[전투 시작 대기] → [준비 모션(공격 시작, 카운터 준비 등)] → [슬로우 모션 상태(플레이어/적의 추가 입력 대기)] → [행동 결과 처리]
↓                                                                            ↑
[턴 종료 처리 후 다음 턴으로 전환]                        ←             [ActionSelection]

~~~

QTR 턴제 전투
-------------
1. 기본 개념<br>
준비모션 -> 슬로우 -> 공격(결과)

* 준비모션에서 넘어갈 때 중간 과정을 자세히 넣지 않고 준비모션에서 몇초간 기다렸다가 공격이 적중하는 모션으로 넘어간다
* 플레이어나 적은 그 기다리는 몇초동안 행동을 선택하고 그 선택에 따라 결과모션이 달라진다
~~~
1. 일반공격은 프레임orSpeed 변수 값으로 결과를 적용
2. 패링: 공격 모션에서 패링 모션 출력
3. 카운터: 공격모션에서 카운터 모션 출력
4. 회피: 공격모션에서 회피 모션 출력
5. 슈퍼아머: 공격을 맞는 모션 출력 후 공격 모션을 출력
~~~

* BattleScene에서 Player와 Enemy는 기본적으로 위치를 고정해놓고 UI를 턴제처럼 만들어서 전투를 진행

ex) 적이 공격을 하면 공격모션이 오는 n초의 시간동안 어떤 행동을 할 수 있고 기본적을 턴이 순차적으로 돌아가되 카운터, 스턴 등의 상황에선 일방적인 턴이 가능
     '강인도'에 따라 슈퍼아머가 적용, 회피를 사용하면 공격을 n%의 확률로 피하고 턴 종료
