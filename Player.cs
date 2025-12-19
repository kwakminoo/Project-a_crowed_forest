using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using System;

public enum MadnessState
{
    None,                   // 정상 상태
    IncomprehensibleFear,  // 이해할 수 없는 공포
    WrathOfThatDay,        // 그날의 분노
    FesteringMadness,      // 좀먹는 광기
    Utopia,                // 이상향
   UnkillablePain          // 날 죽이지 못하는 고통은 날 더 강하게 만들 뿐이다
}

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } //싱글톤 패턴

    [Header("Madness Sprites")]
    public Sprite fearSprite;
    public Sprite rageSprite;
    public Sprite maniaSprite;
    public Sprite utopiaSprite;
    public Sprite painSprite;
    public Sprite defaultSprite;
    public SpriteRenderer spriteRenderer; // 스프라이트 렌더러 연결
    public Animator animator; // 애니메이터 연결


    [Header("Character Info")]
    public string characterName = "레이븐 드레이크"; //플레이어 이름
    public Sprite baseCharacterSprite; //기본 캐릭터 이미지

    [Header("Stats")]
    public int strength = 5;     // 근력 - 무기 물리 공격력 보정
    public int dexterity = 5;    // 기량 - 치명타 확률/속도 또는 명중률
    public int vitality = 5;     // 체력 - MaxHP 증가
    public int willpower = 5;    // 정신력 - 신비 공격력 보정
    public int luck = 5;         // 운 - 회피율, 아이템 드랍률 등

    [Header("Character Staytus")]
    public int maxHP = 100; //최대 체력
    public int currentHP; //현재 체력
    public int maxSanity = 100;
    public int currentSanity;
    public Image playerHPBar; //HP바
    public Image playerSanityBar; //SAN바
    public int level = 1; //플레이어 레벨
    public int experience = 0; // 경험치
    public int experienceToNextLevel = 100; //레벨업까지 필요 경험치
    public event Action OnPlayerDeath; //플레이어가 죽었을 때 이벤트
    public MadnessState currentMadnessState = MadnessState.None; //현재 정신붕괴 상태에 있는지를 나타내는 상태값
    private bool madnessEffectApplied = false;

    [Header("Weapon")]
    public Item equippedWeapon; //장착된 무기
    public Item equippedTop; //상의
    public Item equippedBottom; //하의
    public List<Skill> skillSlots = new List<Skill>(4);

    public Inventory inventory;
    public event System.Action OnCharacterUpdated;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad는 "루트 GameObject"에만 적용 가능
            var rootGo = transform.root != null ? transform.root.gameObject : gameObject;
            DontDestroyOnLoad(rootGo);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventory = Inventory.Instance;
        if (Inventory.Instance != null)
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
        currentSanity = maxSanity;

        //스킬 슬롯 초기화
        for (int i = 0; i < 4; i++)
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

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void TakeSanityDamage(int amount)
    {
        currentSanity = Mathf.Max(currentSanity - amount, 0);
        Debug.Log($"정신력이 {amount} 감소했습니다. 현재 정신력: {currentSanity}/{maxSanity}");
        UpdateSanityBar();
        CheckSanity();
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

    private void UpdateSanityBar()
    {
        if (playerSanityBar == null)
        {
            Debug.LogError("Player SanityBar가 연결되지 않았습니다.");
            return;
        }

        float sanityRatio = Mathf.Clamp01((float)currentSanity / maxSanity);
        playerSanityBar.fillAmount = sanityRatio;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        Debug.Log($"경험치 획득: {amount}. 현재 경험치: {experience}/{experienceToNextLevel}");

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

    private void CheckSanity()
    {
        if (currentSanity <= 50 && currentMadnessState == MadnessState.None)
        {
            EnterRandomMadnessState();
        }
        else if (currentSanity > 50 && currentMadnessState != MadnessState.None)
        {
            RecoverFromMadness(); // 자동 회복
        }
    }

    public bool IsMad()
    {
        return currentMadnessState != MadnessState.None;
    }

    private void EnterRandomMadnessState()
    {
        MadnessState[] madnessOptions = {
            MadnessState.IncomprehensibleFear,
            MadnessState.WrathOfThatDay,
            MadnessState.FesteringMadness,
            MadnessState.Utopia,
            MadnessState.UnkillablePain
        };

        MadnessState randomState = madnessOptions[UnityEngine.Random.Range(0, madnessOptions.Length)];
        ApplyMadnessVisual(randomState);
    }

    public void ApplyMadnessVisual(MadnessState state)
    {
        currentMadnessState = state;
        SpriteRenderer renderer = spriteRenderer != null ? spriteRenderer : GetComponent<SpriteRenderer>();

        switch (state)
        {
            case MadnessState.IncomprehensibleFear:
                renderer.sprite = fearSprite;
                animator.SetTrigger("FearPose");
                PlayEffect("FearAura");
                break;
            case MadnessState.WrathOfThatDay:
                renderer.sprite = rageSprite;
                animator.SetTrigger("RagePose");
                PlayEffect("RageAura");
                break;
            case MadnessState.FesteringMadness:
                renderer.sprite = maniaSprite;
                animator.SetTrigger("ManiaPose");
                PlayEffect("MadnessPulse");
                break;
            case MadnessState.Utopia:
                renderer.sprite = utopiaSprite;
                animator.SetTrigger("UtopiaPose");
                PlayEffect("UtopiaGlow");
                break;
            case MadnessState.UnkillablePain:
                renderer.sprite = painSprite;
                animator.SetTrigger("PainOverdrive");
                PlayEffect("PainFlare");
                break;
            case MadnessState.None:
            default:
                renderer.sprite = defaultSprite;
                animator.SetTrigger("Idle");
                break;
        }

        Debug.Log($"정신붕괴 상태 시각 적용: {state}");
    }

    private void PlayEffect(string effectName)
    {
        Debug.Log($"이펙트 재생: {effectName}");
        // Instantiate(Resources.Load<GameObject>($"Effects/{effectName}"), transform.position, Quaternion.identity);
    }

    // 💥 광기 상태별 전투 효과 계산용 메서드
    public void ApplyMadnessCombatEffect()
    {
        if (madnessEffectApplied) return;

        switch (currentMadnessState)
        {
            case MadnessState.IncomprehensibleFear:
                // 공격력 감소, 회피율 상승
                strength -= 2;
                luck += 2;
                break;
            case MadnessState.WrathOfThatDay:
                // 공격력 상승, 스킬 사용 시 체력 소모
                strength += 4;
                break;
            case MadnessState.FesteringMadness:
                // 무작위로 자해 또는 추가공격 처리 BattleManager에서 결정 가능
                break;
            case MadnessState.Utopia:
                // 무작위 스킬 강제 or 거부는 BattleManager에서 실행 시 결정
                break;
            case MadnessState.UnkillablePain:
                // 공격력 상승, 흡혈, 정신력 회복, 회피율 증가
                strength += 3;
                luck += 3;
                break;
        }

        madnessEffectApplied = true;
    }

    public void RecoverFromMadness()
    {
        Debug.Log($"정신붕괴에서 회복됨: {currentMadnessState}");

        currentMadnessState = MadnessState.None;

        if (spriteRenderer != null)
            spriteRenderer.sprite = defaultSprite;

        if (animator != null)
            animator.SetTrigger("Idle");

        madnessEffectApplied = false; // 전투 보정 초기화
    }

    public void RecoverSanity(int amount)
    {
        currentSanity = Mathf.Min(currentSanity + amount, maxSanity);
        UpdateSanityBar();
        CheckSanity(); // 회복 후 체크
    }



}
