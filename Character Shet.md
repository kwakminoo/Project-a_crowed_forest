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
            Debug.Log("Player가 Inventory 이벤트를 구독했습니다");
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
        Debug.Log("Player 데이터가 Inventory에서 자동으로 동기화됐습니다");
    }

    public void TakeDamage(int damge)
    {
        currentHP -= damge;
        Debug.Log($"레이븐이 {damge}의 데미지를 받았습니다");

        if(currentHP <= 0)
        {
            Die();
        }
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


황색 옷의 왕
-
![file](https://github.com/user-attachments/assets/21497e98-8c92-4e0c-ab7c-301741d1eac3)


황색의 신도
-
![file (1)](https://github.com/user-attachments/assets/80ec92c0-3e22-41cd-975b-281e5c9379e9)

![적 공격 준비 모션](https://github.com/user-attachments/assets/34e31bfe-4e54-4696-85a1-ce08d7afcaff)
![적 기본 모션](https://github.com/user-attachments/assets/c201ce8d-2326-413b-94b8-3de54ed5c602)
![적 공격 모션](https://github.com/user-attachments/assets/04e8bc0c-56f2-4a72-aa82-d613b410779d)
