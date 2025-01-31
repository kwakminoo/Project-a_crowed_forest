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
using Unity.VisualScripting;

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
    public Image playerHPBar;
    public TextMeshProUGUI enemyHPText;
    public Image enemyHPBar;

    private Canvas mainCanvas;
    public Camera mainCamera;
    public Camera battleCamera;

    private bool isBattleActive = true;

    public DialogueRunner dialogueRunner;
    private string nextNode;

    //데이터 초기화
    private void Start()
    {
        mainCanvas = FindObjectOfType<Canvas>();
        if(mainCanvas != null)
        {
            mainCanvas.worldCamera = mainCamera;
            Debug.Log("Canvas의 Event Camera가 Main Camera로 초기 설정되었습니다.");
        }

        inventory = Inventory.Instance ?? throw new NullReferenceException("Inventory.Instance가 null입니다.");
        player = playerObject.GetComponent<Player>() ?? Player.Instance;
        Player.Instance.OnCharacterUpdated += UpdateBattleCharacterImage;
        currentEnemy = enemyObject.GetComponent<EnemyScript>();
        if (battleCharacterImage != null)
        {
            battleCharacterImage.sprite = player.baseCharacterSprite;
        }

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

    public void StartBattle(EnemyData enemyData, string backGroundName, string nextYarnNode)
    {
        if (dialogueRunner != null)
        {
            if (dialogueRunner.IsDialogueRunning) // 실행 중인지 확인
            {
                dialogueRunner.Stop(); // 실행 중이면 대화 중단
                Debug.Log("DialogueRunner: 스토리 출력 중단");
            }

            nextNode = nextYarnNode; // 다음 노드 저장
        }

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

   //전투 흐름
    private IEnumerator BattleRoutine()
    {
        while(isBattleActive)
        {
            //적 턴
            yield return StartCoroutine(EnemyTurn());

            if(!isBattleActive) break;

            //플레이어 턴
            yield return StartCoroutine(PlayerTurn());
        }

        EndBattle();
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log($"{currentEnemy.enemyData.enemyName}의 턴 시작");

        yield return new WaitForSeconds(1f);

        // 카메라 전환: Battle Camera 활성화
        SwitchToBattleCamera();
        yield return new WaitForSeconds(1.0f);

        currentEnemy.UseSkill(player);

        yield return new WaitForSeconds(2.0f);
        SwitchToMainCamera();
        yield return new WaitForSeconds(0.5f); // 카메라 복귀 대기

        UpdateBattleState();

        Debug.Log($"{currentEnemy.enemyData.enemyName}의 턴 종료");

        if(player.currentHP <= 0)
        {
            isBattleActive = false;
            EndBattle();
        }
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
    
    public void EndBattle()
    {
        SwitchToMainCamera();
        Debug.Log("전투 종료");
            if(player.currentHP <= 0)
        {
            Debug.Log($"{currentEnemy.enemyData.enemyName} 승리");
        }
        else if(currentEnemy.currentHP <= 0)
        {
            Debug.Log($"player 승리");
        }

        // ✅ 기존 BGM으로 복귀
        var lineView = FindObjectOfType<CustomLineView>();
        if (lineView != null)
        {
            lineView.RestorePreviousBGM(); // ✅ 전투 종료 후 원래 BGM 복귀
        }
        
        battleWindow.SetActive(false);

        // 스토리 출력 재개
        if (dialogueRunner != null)
        {
            if (!dialogueRunner.IsDialogueRunning && !string.IsNullOrEmpty(nextNode))
            {
                dialogueRunner.StartDialogue(nextNode); // 저장된 노드에서 대화 시작
                Debug.Log($"DialogueRunner: 대화 시작. 노드: {nextNode}");
            }
            else
            {
                Debug.LogError("DialogueRunner가 실행 중이거나 노드가 설정되지 않았습니다.");
            }
        }
    }

    //스킬 사용
    public void UseSkill(int skillIndex)
    {
        List<Skill> battleSkills = player.GetBattleSkills();
        if(skillIndex < 0 || skillIndex >= battleSkills.Count || battleSkills[skillIndex] == null)
        {
            Debug.LogError("유효하지 않은 스킬 슬롯");
            return;
        }

        Skill skill = battleSkills[skillIndex];
            
        skill.ExecuteSkill(player.gameObject, currentEnemy.gameObject, this);

        StartCoroutine(ShowSkillEffect(skill.skillSprite));

        // 카메라 전환: Battle Camera 활성화
        SwitchToBattleCamera();

        // 스킬 실행
        StartCoroutine(ExecuteSkillWithCamera(skill));
        
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

    private IEnumerator ShowSkillEffect(Sprite skillSprite)
    {
        if(battleCharacterImage == null || skillSprite == null)
        {
            Debug.LogError("배틀 캐릭터 이미지 또는 스킬 이미지가 설정되지 않았습니다.");
            yield break;
        }

        battleCharacterImage.enabled = false;
        Sprite originalImage = battleCharacterImage.sprite;
        battleCharacterImage.sprite = skillSprite;
        battleCharacterImage.enabled = true;

        yield return new WaitForSeconds(1.0f);

        battleCharacterImage.sprite = originalImage;
        battleCharacterImage.enabled = true;
    }

    //UI업데이트
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

    public void UpdateBattleState()
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

        UpdateHPBar(player.currentHP, player.maxHP, playerHPBar);
        UpdateHPBar(currentEnemy.currentHP, currentEnemy.maxHP, enemyHPBar);
    }

    private void UpdateHPBar(int currentHP, int maxHP, Image hpBar)
    {
        if (hpBar == null)
        {
            Debug.LogError("HP Bar가 연결되지 않았습니다. 현재 업데이트하려는 HP Bar가 null입니다.");
            return;
        }

        float hpRatio = Mathf.Clamp01((float)currentHP / maxHP);
        hpBar.fillAmount = hpRatio;
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
    
    //카메라 제어
    public void SwitchToBattleCamera()
    {
        if (battleCamera != null)
        {
            battleCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Battle Camera가 참조되지 않았습니다.");
        }

        // Canvas의 Event Camera 설정
        if (mainCanvas != null)
        {
            mainCanvas.worldCamera = battleCamera;
        }

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }

    public void SwitchToMainCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }
        // Canvas의 Event Camera 설정
        if (mainCanvas != null)
        {
            mainCanvas.worldCamera = mainCamera;
        }
        if (battleCamera != null)
        {
            battleCamera.gameObject.SetActive(false);
        }
    }   

    // 일정 시간 후 카메라 전환
    public IEnumerator SwitchBackToMainCameraAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchToMainCamera();
    }

    // 스킬 사용 시 카메라 전환
    public void HandleSkillCameraTransition(Player player, SkillRuntimeData skill)
    {
        if (player == null || skill == null) return;

        // 배틀 카메라 활성화
        SwitchToBattleCamera();

        // 스킬 연출 후 일정 시간 뒤 메인 카메라로 복귀
        StartCoroutine(SwitchBackToMainCameraAfterDelay(2.0f));
    }

    private IEnumerator ExecuteSkillWithCamera(Skill skill)
    {
        if (skill == null)
        {
            Debug.LogError("스킬 데이터가 null입니다.");
            yield break;
        }

        // 카메라 전환 효과 및 스킬 연출
        Debug.Log($"스킬 연출 시작: {skill.skillName}");
        yield return new WaitForSeconds(2.0f); // 연출 대기 (예시)

        // 연출 종료 후 카메라 복귀
        yield return SwitchBackToMainCameraAfterDelay(1.0f);
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
