Player
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set;} //싱글톤 패턴

    [Header("Character Info")]
    public string characterName = "레이븐 드레이크"; //플레이어 이름
    public Sprite baseCharacterSprite; //기본 캐릭터 이미지

    [Header("Character Staytus")]
    public int maxHP = 100; //최대 체력
    public int currentHP; //현재 체력
    public Image playerHPBar;
    public int level = 1; //플레이어 레벨
    public int experience = 0; // 경험치
    public  int experienceToNextLevel = 100; //레벨업까지 필요 경험치
    public event Action OnPlayerDeath; //플레이어가 죽었을 때 이벤트

    [Header("Weapon")]
    public Item equippedWeapon; //장착된 무기
    public Item equippedTop; //상의
    public Item equippedBottom; //하의
    public List<Skill> skillSlots = new List<Skill>(4);

    public Inventory inventory;
    public event System.Action OnCharacterUpdated;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {   
        inventory = Inventory.Instance;
        if(Inventory.Instance != null)
        {
            Inventory.Instance.OnInventoryUpdated += UpdateFromInventory;
        }
        else
        {
            Debug.LogError("Inventory.Instance가 null입니다");
        }
        //초기 동기화
        UpdateFromInventory();

        currentHP = maxHP; //초기 체력 설정
        
        //스킬 슬롯 초기화
        for(int i = 0; i < 4; i++)
        {
            skillSlots.Add(null);
        }
    }

    public Sprite GetCompositeCharacterImage()
    {
        return baseCharacterSprite;
    }

    public void UpdateCharacterState(Item weapon, Item top, Item bottom)
    {
        equippedWeapon = weapon;
        equippedTop = top;
        equippedBottom = bottom;

        OnCharacterUpdated?.Invoke();
    }

    public void UpdateFromInventory()
    {
        equippedWeapon = Inventory.Instance.equippedWeapon;
        skillSlots = Inventory.Instance.skillSlots;
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0); // HP 감소, 최소값은 0
        Debug.Log($"레이븐이 {damage}의 데미지를 받았습니다");

        UpdateHPBar();

        if(currentHP <= 0)
        {
            Die();
        }
    }

    private void UpdateHPBar()
    {
        if (playerHPBar == null)
        {
            Debug.LogError("Player HPBar가 연결되지 않았습니다.");
            return;
        }

        float hpRatio = Mathf.Clamp01((float)currentHP / maxHP); // HP 비율 계산
        playerHPBar.fillAmount = hpRatio; // HPBar 길이 설정
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        Debug.Log("경험치 획득: {amount}. 현재 경험치: {experience}/{experienceToNextLevel}");

        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        experience -= experienceToNextLevel;
        level++;
        maxHP += 10; //최대 체력 증가
        currentHP = maxHP; //체력 회복
        experienceToNextLevel += 50; //레벨업마다 필요 경험치 증가

        Debug.Log($"레벨업! 현재 레벨: {level}, 최대 체력: {maxHP}");
    }

    private void Die()
    {
        Debug.Log("YOU DIE");
        OnPlayerDeath?.Invoke();
    }

    public List<Skill> GetBattleSkills()
    {
        return Inventory.Instance.GetBattleSkills();
    }
}
~~~

Enemy Sprite
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public EnemyData enemyData;
    public Image enemyImage;
    public int currentHP; //현재 체력
    public int maxHP;
    public Image enemyHPBar;
    private Animator animator;
    
    private void Awake()
    {
        if(enemyData != null)
        {
            InitializeEnemy(enemyData);
        }
    }

    //적 데이터 초기화
    public void InitializeEnemy(EnemyData data)
    {
        enemyData = data;
        if (enemyData.skills != null)
        {
            // 방어적 복사
            enemyData.skills = new List<Skill>(enemyData.skills);

            // 유효하지 않은 스킬 제거
            enemyData.skills.RemoveAll(skill =>
            {
                if (skill == null || string.IsNullOrEmpty(skill.skillName) || skill.successRate <= 0 || skill.damage <= 0 || skill.skillSprite == null)
                {
                    Debug.LogError($"유효하지 않은 스킬 제거: 이름={(skill?.skillName ?? "null")}, 성공률={(skill?.successRate ?? 0)}, 데미지={(skill?.damage ?? 0)}, 스프라이트={(skill?.skillSprite == null ? "null" : "존재")}");
                    return true; // 제거
                }
                return false; // 유지
            });
        }
        else
        {
            Debug.LogError("EnemyData.skills가 null입니다.");
        }

        enemyData = data;
        maxHP = enemyData.maxHP;
        currentHP = maxHP;
        Debug.Log($"{enemyData.enemyName} 초기화 완료: 체력 {currentHP}/{enemyData.maxHP}");
    }


    public void UseSkill(Player target)
    {
        if(enemyData == null || enemyData.skills == null || enemyData.skills.Count == 0)
        {
            Debug.LogError("적 스킬이 설정 되지 않았습니다");
            return;
        }

        SkillRuntimeData selectedSkill = ChooseSkill();

        if(selectedSkill == null)
        {
            Debug.LogError($"{enemyData.enemyName}이(가) 사용할 수 있는 스킬이 없습니다");
            return;
        }

        BattleManager battleManager = FindObjectOfType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.HandleSkillCameraTransition(target, selectedSkill);
        }

        if (enemyImage != null)
        {
            StartCoroutine(ChangeEnemyImage(selectedSkill.skillSprite, 1.0f));
        }
        ExecuteSkillRuntime(selectedSkill, target); // 런타임 데이터 기반 실행
    }

    private void ExecuteSkillRuntime(SkillRuntimeData skill, Player target)
    {
        if (string.IsNullOrEmpty(skill.skillName) || skill.successRate <= 0 || skill.damage <= 0 || skill.skillSprite == null)
        {
            Debug.LogError($"스킬 데이터가 유효하지 않습니다. 이름: {skill.skillName}, 성공률: {skill.successRate}, 데미지: {skill.damage}, 스프라이트: {skill.skillSprite}");
            return;
        }

        Debug.Log($"{enemyData.enemyName}이(가) {skill.skillName} 실행 - 설정된 데미지: {skill.damage}");

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll > skill.successRate)
        {
            Debug.Log($"{skill.skillName}이(가) 실패했습니다. (Roll: {roll}, Success Rate: {skill.successRate})");
            return;
        }

        target.TakeDamage(skill.damage);
    }

    public IEnumerator ChangeEnemyImage(Sprite newSprite, float duration)
    {
        if (enemyImage != null)
        {
            // 기존 이미지 저장
            Sprite originalSprite = enemyImage.sprite;

            // 스킬 이미지로 변경
            enemyImage.sprite = newSprite;

            yield return new WaitForSeconds(duration);

            // 기존 이미지로 복원
            if (originalSprite != null)
            {
                enemyImage.sprite = originalSprite;
            }
            else
            {
                Debug.LogWarning("originalSprite가 null입니다. 기본 이미지를 설정할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("enemyImage가 null 상태입니다. 이미지 변경이 불가능합니다.");
        }
    }

    public SkillRuntimeData ChooseSkill()
    {
        if (enemyData.skills == null || enemyData.skills.Count == 0)
        {
            Debug.LogError($"{enemyData.enemyName}에게 할당된 스킬이 없습니다");
            return null;
        }

        List<Skill> validSkills = enemyData.skills.FindAll(skill =>
        {
            if (skill == null || string.IsNullOrEmpty(skill.skillName) || skill.successRate <= 0 || skill.damage <= 0 || skill.skillSprite == null)
            {
                Debug.LogWarning($"유효하지 않은 스킬이 필터링됨: 이름={(skill?.skillName ?? "null")}, 성공률={(skill?.successRate ?? 0)}, 데미지={(skill?.damage ?? 0)}, 스프라이트={(skill?.skillSprite == null ? "null" : "존재")}");
                return false;
            }
            return true;
        });

        if (validSkills.Count == 0)
        {
            Debug.LogError($"{enemyData.enemyName}에게 유효한 스킬이 없습니다");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, validSkills.Count);
        Skill selectedSkill = validSkills[randomIndex];

        return new SkillRuntimeData(selectedSkill.Clone());
    }

    //체력 변경 메소드
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0); // HP 감소, 최소값은 0
        Debug.Log($"{enemyData.enemyName}이(가) {damage}의 데미지를 받았습니다");
        if (enemyHPBar == null)
        {
            Debug.LogError("Enemy HPBar가 null입니다. 연결 상태를 확인하세요.");
        }

        UpdateHPBar();

        if(currentHP <= 0)
        {
            Die();
        }
    }

    private void UpdateHPBar()
    {
        if (enemyHPBar == null)
        {
            Debug.LogError("Enemy HPBar가 연결되지 않았습니다.");
            return;
        }

        float hpRatio = Mathf.Clamp01((float)currentHP / maxHP); // HP 비율 계산
        enemyHPBar.fillAmount = hpRatio; // HPBar 길이 설정
    }

    public void Die()
    {
        if(enemyData.deathAnimation != null)
        {
            Debug.Log($"{enemyData.enemyName}이(가) 쓰러졌습니다");
            Animator animator = GetComponent<Animator>();
            if(animator != null)
            {
                animator.Play(enemyData.deathAnimation.name);
            }
        }

    }

    //스킬 가져오기
    public List<Skill> GetSkills()
    {
        return enemyData.skills;
    }
}
~~~

enemy Data
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy System/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName; //적 이름
    public Sprite enemySprite; //적 이미지
    public int maxHP; //적 최대 체력
    public List<Skill> skills; //적이 사용할 스킬
}
~~~

레이븐 드레이크
-
![레이븐 드레이크](https://github.com/user-attachments/assets/fad16a47-38d0-4271-bf97-af7534e1c8d2)
* 드레이크가의 장남, 황색의 신도들에게 가족과 가문을 잃었다. 드레이크가는 가문지하에 잊혀진 옛것으로 보이는 순백의 나무를 이용해 두 아들을 만들었다

![레이븐 회피 모션](https://github.com/user-attachments/assets/1957e8be-38be-4283-b2b6-0b3d18254b79)
![레이븐 공격 모션](https://github.com/user-attachments/assets/1993016d-d481-48c1-b483-84f1d5183657)
![레이븐 공격준비](https://github.com/user-attachments/assets/81a65537-d5f0-4b86-bc7c-6ecb5c2d5beb)
![레이븐 걷기](https://github.com/user-attachments/assets/285cf75a-8efb-4aa6-b5c9-2038821e4dde)

![Raven_TEST-Sheet-horizontal-sprites](https://github.com/user-attachments/assets/1ae22452-2e1e-4215-8771-9d0d5c8cdecd)

황색 옷의 왕
-
![file](https://github.com/user-attachments/assets/21497e98-8c92-4e0c-ab7c-301741d1eac3)


황색의 신도
-
![file (1)](https://github.com/user-attachments/assets/80ec92c0-3e22-41cd-975b-281e5c9379e9)

![적 공격 준비 모션](https://github.com/user-attachments/assets/34e31bfe-4e54-4696-85a1-ce08d7afcaff)
![적 기본 모션](https://github.com/user-attachments/assets/c201ce8d-2326-413b-94b8-3de54ed5c602)
![적 공격 모션](https://github.com/user-attachments/assets/04e8bc0c-56f2-4a72-aa82-d613b410779d)
