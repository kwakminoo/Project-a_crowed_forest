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
    [Tooltip("기본 최대 체력 (vitality 보정 전)")]
    public int baseMaxHP = 100;
    public int maxHP = 100; //최대 체력 (자동 계산됨)
    public int currentHP; //현재 체력
    [Tooltip("기본 최대 정신력")]
    public int baseMaxSanity = 100;
    public int maxSanity = 100;
    public int currentSanity;
    public Image playerHPBar; //HP바
    public Image playerSanityBar; //SAN바
    public int level = 1; //플레이어 레벨
    public int experience = 0; // 경험치
    public int experienceToNextLevel = 100; //레벨업까지 필요 경험치
    public event Action OnPlayerDeath; //플레이어가 죽었을 때 이벤트
    public MadnessState currentMadnessState = MadnessState.None; //현재 정신붕괴 상태에 있는지를 나타내는 상태값

    [Header("Weapon")]
    public Item equippedWeapon; //장착된 무기
    public Item equippedTop; //상의
    public Item equippedBottom; //하의
    public Item equippedAccessory; //장신구
    public List<Skill> skillSlots = new List<Skill>(4);
    
    [Header("Traits")]
    [Tooltip("현재 장착된 특성 (최대 3개) - 슬롯 0: 무기, 슬롯 1: 장신구, 슬롯 2: 방어구")]
    public List<Trait> equippedTraits = new List<Trait>(3);
    
    [Tooltip("획득한 모든 특성 (일반 특성만, 귀속 특성은 equippedTraits에만 존재)")]
    public List<Trait> ownedTraits = new List<Trait>();
    
    [Tooltip("최대 특성 보유 개수")]
    public int maxTraitSlots = 3;

    [Header("Weapon Proficiency")]
    [Tooltip("무기 종류별 숙련도 경험치 (레벨별로 필요한 경험치가 다름)")]
    public Dictionary<WeaponCategory, int> weaponProficiencyExp = new Dictionary<WeaponCategory, int>();
    
    [Tooltip("무기 종류별 최대 달성 레벨 (7레벨까지는 자동, 그 이상은 특수 조건 필요)")]
    public Dictionary<WeaponCategory, int> weaponProficiencyMaxLevel = new Dictionary<WeaponCategory, int>();
    
    [Tooltip("전투에서 무기 사용 시 획득하는 숙련도 경험치 (기본값)")]
    public int proficiencyExpPerUse = 1;
    
    [Tooltip("레벨 0→1에 필요한 경험치")]
    public int baseExpForLevel1 = 10;
    
    [Tooltip("레벨당 경험치 증가 곡선 배수 (높을수록 어려워짐)")]
    public float expCurveMultiplier = 1.5f;
    
    [Tooltip("자동 레벨업 가능한 최대 레벨 (이 레벨까지는 스킬 사용으로만 올릴 수 있음)")]
    public int maxAutoLevel = 7;

    public Inventory inventory;
    public event System.Action OnCharacterUpdated;
    public event Action<WeaponCategory, int> OnProficiencyLevelUp; // 숙련도 레벨업 이벤트
    public event Action OnTraitsUpdated; // 특성 변경 이벤트
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // DontDestroyOnLoad는 루트 오브젝트에서만 작동하므로, 부모가 있으면 씬 루트로 이동
            if (transform.parent != null)
            {
                Debug.LogWarning($"⚠ Player가 루트 오브젝트가 아닙니다. 씬 루트로 이동합니다. (부모: {transform.parent.name})");
                transform.SetParent(null); // 씬 루트로 이동
            }
            
            // 이제 루트 오브젝트이므로 DontDestroyOnLoad 적용
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

        // 파생 스테이터스 재계산
        RecalculateDerivedStats();
        
        currentHP = maxHP; //초기 체력 설정
        currentSanity = maxSanity;

        //스킬 슬롯 초기화
        for (int i = 0; i < 4; i++)
        {
            skillSlots.Add(null);
        }
        
        // 무기 숙련도 초기화
        InitializeWeaponProficiency();
        
        // 특성 시스템 초기화
        InitializeTraits();
        
        // 숙련도 레벨업 이벤트 구독 (디버그용)
        OnProficiencyLevelUp += (category, level) =>
        {
            Debug.Log($"🎉 {category} 숙련도 레벨업! 레벨 {level}");
        };
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
        equippedTop = Inventory.Instance.equippedTop;
        equippedBottom = Inventory.Instance.equippedBottom;
        equippedAccessory = Inventory.Instance.equippedAccessory;
        skillSlots = Inventory.Instance.skillSlots;
        
        // 장비 장착 시 특성 업데이트
        UpdateTraitsFromEquipment();
        
        // 장비 변경 시 파생 스테이터스 재계산
        OnEquipmentChanged();
    }

    public void TakeDamage(int damage)
    {
        // 방어력 계산 (vitality 기반 + 장비 보정)
        int defense = CalculateDefense();
        int finalDamage = Mathf.Max(1, damage - defense); // 최소 1 데미지
        
        currentHP = Mathf.Max(currentHP - finalDamage, 0);
        Debug.Log($"레이븐이 {damage} 데미지를 받았습니다. (방어력: {defense}, 최종 데미지: {finalDamage})");

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
        
        // 레벨업 시 기본 스테이터스 증가 (선택사항 - 나중에 스테이터스 포인트 분배로 변경 가능)
        // strength += 1;
        // dexterity += 1;
        // vitality += 1;
        // willpower += 1;
        // luck += 1;
        
        // 파생 스테이터스 재계산
        RecalculateDerivedStats();
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
        // 정신력이 50 이하로 떨어지고 정신붕괴 상태가 없으면 랜덤 정신붕괴 진입
        if (currentSanity <= 50 && currentMadnessState == MadnessState.None)
        {
            EnterRandomMadnessState();
        }
        // 정신력이 70 이상으로 회복되면 정신붕괴 상태 해제 (다키스트 던전 스타일)
        else if (currentSanity >= 70 && currentMadnessState != MadnessState.None)
        {
            RecoverFromMadness(); // 자동 회복
        }
        // 정신력이 50~70 사이면 상태 유지 (자동 해제 안 됨)
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
    // 주의: 이 메서드는 더 이상 스테이터스를 직접 변경하지 않습니다.
    // 정신붕괴 보정은 GetFinalStrength(), GetFinalLuck() 등에서 자동 계산됩니다.
    public void ApplyMadnessCombatEffect()
    {
        // 정신붕괴 효과는 최종 스테이터스 계산 메서드에서 자동 반영됨
        // 여기서는 전투 시작 시 정신붕괴 상태 확인만 수행
        if (currentMadnessState != MadnessState.None)
        {
            Debug.Log($"전투 시작: 정신붕괴 상태 {currentMadnessState} 적용됨");
        }
    }

    public void RecoverFromMadness()
    {
        // 정신붕괴 보정은 GetFinalStrength() 등에서 자동으로 계산되므로
        // 여기서는 상태만 해제하면 됨 (스테이터스 직접 변경 불필요)
        
        Debug.Log($"정신붕괴에서 회복됨: {currentMadnessState}");

        currentMadnessState = MadnessState.None;

        if (spriteRenderer != null)
            spriteRenderer.sprite = defaultSprite;

        if (animator != null)
            animator.SetTrigger("Idle");
        
        // 파생 스테이터스 재계산 (정신붕괴 해제 시)
        RecalculateDerivedStats();
    }

    /// <summary>
    /// 잠자기 - 정신붕괴 상태를 해제합니다 (다키스트 던전 스타일)
    /// </summary>
    public void Rest()
    {
        if (currentMadnessState != MadnessState.None)
        {
            RecoverFromMadness();
            Debug.Log("잠을 자서 정신붕괴 상태가 해제되었습니다.");
        }
        
        // 잠자기 시 체력과 정신력 회복 (선택사항)
        // currentHP = maxHP;
        // currentSanity = maxSanity;
    }

    public void RecoverSanity(int amount)
    {
        currentSanity = Mathf.Min(currentSanity + amount, maxSanity);
        UpdateSanityBar();
        CheckSanity(); // 회복 후 체크 (정신력 70 이상이면 자동 해제)
    }

    // ========== 무기 숙련도 시스템 ==========

    /// <summary>
    /// 무기 숙련도 초기화 - 모든 무기 종류를 0으로 설정
    /// </summary>
    private void InitializeWeaponProficiency()
    {
        foreach (WeaponCategory category in System.Enum.GetValues(typeof(WeaponCategory)))
        {
            if (!weaponProficiencyExp.ContainsKey(category))
            {
                weaponProficiencyExp[category] = 0;
            }
            if (!weaponProficiencyMaxLevel.ContainsKey(category))
            {
                weaponProficiencyMaxLevel[category] = maxAutoLevel; // 초기값은 7레벨까지 자동 가능
            }
        }
        Debug.Log("무기 숙련도 시스템 초기화 완료");
    }
    
    /// <summary>
    /// 특정 레벨에 도달하기 위해 필요한 총 경험치 계산 (레벨별 경험치 곡선)
    /// </summary>
    public int GetRequiredExpForLevel(int targetLevel)
    {
        if (targetLevel <= 0) return 0;
        if (targetLevel > 10) targetLevel = 10;
        
        int totalExp = 0;
        for (int level = 1; level <= targetLevel; level++)
        {
            // 레벨별 필요 경험치: baseExp * (expCurveMultiplier ^ (level - 1))
            int expForThisLevel = Mathf.RoundToInt(baseExpForLevel1 * Mathf.Pow(expCurveMultiplier, level - 1));
            totalExp += expForThisLevel;
        }
        
        return totalExp;
    }
    
    /// <summary>
    /// 현재 경험치로 도달 가능한 레벨 계산
    /// </summary>
    public int GetLevelFromExp(int exp)
    {
        int level = 0;
        int accumulatedExp = 0;
        
        for (int l = 1; l <= 10; l++)
        {
            int expForLevel = Mathf.RoundToInt(baseExpForLevel1 * Mathf.Pow(expCurveMultiplier, l - 1));
            if (accumulatedExp + expForLevel <= exp)
            {
                accumulatedExp += expForLevel;
                level = l;
            }
            else
            {
                break;
            }
        }
        
        return level;
    }

    /// <summary>
    /// 무기 사용 시 숙련도 경험치 획득 (7레벨까지 자동 레벨업 가능)
    /// </summary>
    /// <param name="weapon">사용한 무기</param>
    public void GainWeaponProficiency(Item weapon)
    {
        if (weapon == null || !weapon.IsWeapon())
        {
            return;
        }

        WeaponCategory category = weapon.weaponCategory;
        int currentLevel = GetProficiencyLevel(category);
        int maxAllowedLevel = GetMaxAllowedLevel(category);
        
        // 현재 레벨이 최대 허용 레벨에 도달했으면 더 이상 경험치 획득 불가
        if (currentLevel >= maxAllowedLevel)
        {
            return;
        }
        
        // 경험치 증가
        if (!weaponProficiencyExp.ContainsKey(category))
        {
            weaponProficiencyExp[category] = 0;
        }
        
        weaponProficiencyExp[category] += proficiencyExpPerUse;
        
        // 새로운 레벨 계산
        int newLevel = GetLevelFromExp(weaponProficiencyExp[category]);
        
        // 최대 허용 레벨을 초과하지 않도록 제한
        if (newLevel > maxAllowedLevel)
        {
            newLevel = maxAllowedLevel;
            // 경험치도 최대 레벨에 맞게 조정
            weaponProficiencyExp[category] = GetRequiredExpForLevel(maxAllowedLevel);
        }
        
        // 레벨업 체크
        if (newLevel > currentLevel)
        {
            OnProficiencyLevelUp?.Invoke(category, newLevel);
            Debug.Log($"{category} 숙련도 레벨업! 레벨 {currentLevel} → {newLevel} (경험치: {weaponProficiencyExp[category]})");
        }
    }
    
    /// <summary>
    /// 특정 무기의 최대 허용 레벨 조회 (7레벨까지 자동, 그 이상은 특수 조건 필요)
    /// </summary>
    public int GetMaxAllowedLevel(WeaponCategory category)
    {
        if (!weaponProficiencyMaxLevel.ContainsKey(category))
        {
            return maxAutoLevel; // 기본값은 7레벨
        }
        return weaponProficiencyMaxLevel[category];
    }
    
    /// <summary>
    /// 특수 조건으로 숙련도 레벨업 (7레벨 이상용)
    /// </summary>
    /// <param name="category">무기 종류</param>
    /// <param name="reason">레벨업 이유 (퀘스트 완료, 칭호 획득 등)</param>
    /// <returns>레벨업 성공 여부</returns>
    public bool LevelUpProficiencyBySpecialCondition(WeaponCategory category, string reason = "")
    {
        int currentLevel = GetProficiencyLevel(category);
        int maxAllowedLevel = GetMaxAllowedLevel(category);
        
        // 이미 최대 레벨이면 실패
        if (currentLevel >= 10)
        {
            Debug.LogWarning($"{category} 숙련도가 이미 최대 레벨(10)입니다.");
            return false;
        }
        
        // 최대 허용 레벨 증가
        if (!weaponProficiencyMaxLevel.ContainsKey(category))
        {
            weaponProficiencyMaxLevel[category] = maxAutoLevel;
        }
        
        weaponProficiencyMaxLevel[category] = Mathf.Min(weaponProficiencyMaxLevel[category] + 1, 10);
        
        // 경험치를 새 레벨에 맞게 설정
        int newMaxLevel = weaponProficiencyMaxLevel[category];
        if (!weaponProficiencyExp.ContainsKey(category))
        {
            weaponProficiencyExp[category] = 0;
        }
        
        // 새 레벨에 필요한 경험치로 설정
        weaponProficiencyExp[category] = GetRequiredExpForLevel(newMaxLevel);
        
        Debug.Log($"{category} 숙련도 특수 레벨업! 레벨 {currentLevel} → {newMaxLevel} (이유: {reason})");
        OnProficiencyLevelUp?.Invoke(category, newMaxLevel);
        
        return true;
    }
    
    /// <summary>
    /// 퀘스트 완료로 특정 무기 숙련도 레벨업
    /// </summary>
    public bool LevelUpProficiencyByQuest(WeaponCategory category, string questName)
    {
        return LevelUpProficiencyBySpecialCondition(category, $"퀘스트 완료: {questName}");
    }
    
    /// <summary>
    /// 칭호 획득으로 특정 무기 숙련도 레벨업
    /// </summary>
    public bool LevelUpProficiencyByTitle(WeaponCategory category, string titleName)
    {
        return LevelUpProficiencyBySpecialCondition(category, $"칭호 획득: {titleName}");
    }
    
    /// <summary>
    /// 모든 무기 숙련도 레벨업 (칭호 효과 등)
    /// </summary>
    public void LevelUpAllProficiencyBySpecialCondition(string reason = "")
    {
        foreach (WeaponCategory category in System.Enum.GetValues(typeof(WeaponCategory)))
        {
            if (category == WeaponCategory.Unarmed) continue; // 맨손은 제외
            
            int currentLevel = GetProficiencyLevel(category);
            if (currentLevel < 10)
            {
                LevelUpProficiencyBySpecialCondition(category, reason);
            }
        }
    }

    /// <summary>
    /// 현재 무기의 숙련도 경험치 획득 (장착된 무기 기준)
    /// </summary>
    public void GainCurrentWeaponProficiency()
    {
        if (equippedWeapon != null && equippedWeapon.IsWeapon())
        {
            GainWeaponProficiency(equippedWeapon);
        }
    }

    /// <summary>
    /// 무기 종류별 숙련도 레벨 조회 (0~10)
    /// </summary>
    public int GetProficiencyLevel(WeaponCategory category)
    {
        if (!weaponProficiencyExp.ContainsKey(category))
        {
            return 0;
        }
        return GetLevelFromExp(weaponProficiencyExp[category]);
    }

    /// <summary>
    /// 무기 종류별 숙련도 경험치 조회
    /// </summary>
    public int GetProficiencyExp(WeaponCategory category)
    {
        if (!weaponProficiencyExp.ContainsKey(category))
        {
            return 0;
        }
        return weaponProficiencyExp[category];
    }
    
    /// <summary>
    /// 다음 레벨까지 필요한 경험치 조회
    /// </summary>
    public int GetExpToNextLevel(WeaponCategory category)
    {
        int currentLevel = GetProficiencyLevel(category);
        int maxAllowedLevel = GetMaxAllowedLevel(category);
        
        // 이미 최대 레벨이면 0 반환
        if (currentLevel >= maxAllowedLevel)
        {
            return 0;
        }
        
        int currentExp = GetProficiencyExp(category);
        int requiredExpForNextLevel = GetRequiredExpForLevel(currentLevel + 1);
        
        return requiredExpForNextLevel - currentExp;
    }

    /// <summary>
    /// 현재 장착된 무기의 숙련도 레벨 조회
    /// </summary>
    public int GetCurrentWeaponProficiencyLevel()
    {
        if (equippedWeapon == null || !equippedWeapon.IsWeapon())
        {
            return 0;
        }
        return GetProficiencyLevel(equippedWeapon.weaponCategory);
    }

    /// <summary>
    /// 숙련도 기반 공격력 보너스 계산
    /// </summary>
    public int GetProficiencyDamageBonus(WeaponCategory category)
    {
        int level = GetProficiencyLevel(category);
        // 레벨당 +2 데미지 보너스 (최대 +20)
        return level * 2;
    }

    /// <summary>
    /// 숙련도 기반 방어력 보너스 계산
    /// </summary>
    public int GetProficiencyDefenseBonus(WeaponCategory category)
    {
        int level = GetProficiencyLevel(category);
        // 레벨당 +1 방어력 보너스 (최대 +10)
        return level * 1;
    }

    /// <summary>
    /// 숙련도 기반 패링/회피 성공률 보너스 계산
    /// </summary>
    public float GetProficiencyParryBonus(WeaponCategory category)
    {
        int level = GetProficiencyLevel(category);
        // 레벨당 +2% 성공률 보너스 (최대 +20%)
        return level * 0.02f;
    }

    /// <summary>
    /// 현재 장착된 무기의 숙련도 보너스 적용된 데미지 계산
    /// </summary>
    public int GetCurrentWeaponDamageBonus()
    {
        if (equippedWeapon == null || !equippedWeapon.IsWeapon())
        {
            return 0;
        }
        return GetProficiencyDamageBonus(equippedWeapon.weaponCategory);
    }

    /// <summary>
    /// 스킬 사용 가능 여부 확인 (필요한 숙련도 레벨 체크)
    /// </summary>
    public bool CanUseSkill(Skill skill, WeaponCategory requiredCategory, int requiredLevel)
    {
        if (skill == null)
        {
            return false;
        }
        
        int currentLevel = GetProficiencyLevel(requiredCategory);
        return currentLevel >= requiredLevel;
    }

    /// <summary>
    /// 스킬이 현재 장착된 무기로 사용 가능한지 확인
    /// </summary>
    public bool CanUseSkillWithCurrentWeapon(Skill skill, int requiredLevel)
    {
        if (equippedWeapon == null || !equippedWeapon.IsWeapon())
        {
            return false;
        }
        
        return CanUseSkill(skill, equippedWeapon.weaponCategory, requiredLevel);
    }

    // ========== 특성 시스템 ==========

    /// <summary>
    /// 특성 시스템 초기화
    /// </summary>
    private void InitializeTraits()
    {
        // 특성 슬롯 초기화 (최대 3개)
        while (equippedTraits.Count < maxTraitSlots)
        {
            equippedTraits.Add(null);
        }
        
        // 슬롯 개수 제한
        if (equippedTraits.Count > maxTraitSlots)
        {
            equippedTraits = equippedTraits.GetRange(0, maxTraitSlots);
        }
        
        Debug.Log("특성 시스템 초기화 완료");
    }

    /// <summary>
    /// 장비 장착 시 특성 자동 적용
    /// </summary>
    private void UpdateTraitsFromEquipment()
    {
        // 기존 장비 특성 효과 제거
        RemoveAllEquipmentTraitEffects();
        
        // 장비별 특성 적용 (우선순위: 무기 → 장신구 → 방어구)
        ApplyEquipmentTrait(equippedWeapon, 0);      // 무기: 슬롯 0
        ApplyEquipmentTrait(equippedAccessory, 1);   // 장신구: 슬롯 1
        ApplyEquipmentTrait(equippedTop, 2);         // 방어구: 슬롯 2
        ApplyEquipmentTrait(equippedBottom, 2);      // 방어구: 슬롯 2 (하의가 있으면 상의 대체)
        
        // 특성 효과 적용
        ApplyAllTraitEffects();
        
        // 파생 스테이터스 재계산 (특성 변경 시)
        RecalculateDerivedStats();
        
        OnTraitsUpdated?.Invoke();
    }

    /// <summary>
    /// 장비의 특성을 해당 슬롯에 적용
    /// </summary>
    private void ApplyEquipmentTrait(Item equipment, int slotIndex)
    {
        if (equipment == null || !equipment.HasTrait() || slotIndex < 0 || slotIndex >= maxTraitSlots)
        {
            return;
        }
        
        // 귀속 특성이 해당 슬롯에 있으면 교체하지 않음
        if (equippedTraits[slotIndex] != null && equippedTraits[slotIndex].IsBound())
        {
            Debug.Log($"슬롯 {slotIndex}에 귀속 특성이 있어 {equipment.itemName}의 특성이 적용되지 않습니다.");
            return;
        }
        
        // 슬롯에 특성 적용
        equippedTraits[slotIndex] = equipment.attachedTrait;
        Debug.Log($"{equipment.itemName}의 특성 '{equipment.attachedTrait.traitName}'이 슬롯 {slotIndex}에 적용되었습니다.");
    }

    /// <summary>
    /// 모든 장비 특성 효과 제거
    /// </summary>
    private void RemoveAllEquipmentTraitEffects()
    {
        for (int i = 0; i < equippedTraits.Count; i++)
        {
            if (equippedTraits[i] != null)
            {
                // 장비에서 온 특성인지 확인 (일반적으로 장비 특성은 제거 가능)
                // 여기서는 모든 특성 효과를 제거하고 다시 적용하는 방식 사용
                equippedTraits[i].RemoveEffects(this);
            }
        }
    }

    /// <summary>
    /// 모든 특성 효과 적용
    /// </summary>
    private void ApplyAllTraitEffects()
    {
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                trait.ApplyEffects(this);
            }
        }
    }

    /// <summary>
    /// 특성 획득 (퀘스트 완료, 전투 보상 등)
    /// </summary>
    public bool AcquireTrait(Trait trait, string source = "")
    {
        if (trait == null)
        {
            Debug.LogError("획득하려는 특성이 null입니다.");
            return false;
        }
        
        // 이미 보유한 특성인지 확인 (귀속 특성은 중복 획득 방지)
        if (trait.IsBound() && HasTrait(trait))
        {
            Debug.LogWarning($"이미 보유한 귀속 특성입니다: {trait.traitName}");
            return false;
        }
        
        // 일반 특성은 ownedTraits에 추가
        if (!trait.IsBound())
        {
            if (!ownedTraits.Contains(trait))
            {
                ownedTraits.Add(trait);
                Debug.Log($"특성 획득: {trait.traitName} (출처: {source})");
            }
        }
        else
        {
            // 귀속 특성은 즉시 장착 (빈 슬롯에)
            int emptySlot = FindEmptyTraitSlot();
            if (emptySlot >= 0)
            {
                equippedTraits[emptySlot] = trait;
                trait.ApplyEffects(this);
                Debug.Log($"귀속 특성 획득 및 장착: {trait.traitName} (슬롯 {emptySlot}, 출처: {source})");
                OnTraitsUpdated?.Invoke();
                return true;
            }
            else
            {
                Debug.LogWarning($"특성 슬롯이 모두 찼습니다. {trait.traitName}을 장착할 수 없습니다.");
                return false;
            }
        }
        
        OnTraitsUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// 특성 장착 (일반 특성만, 상태창에서 사용)
    /// </summary>
    public bool EquipTrait(Trait trait, int slotIndex)
    {
        if (trait == null)
        {
            Debug.LogError("장착하려는 특성이 null입니다.");
            return false;
        }
        
        if (slotIndex < 0 || slotIndex >= maxTraitSlots)
        {
            Debug.LogError($"잘못된 슬롯 인덱스: {slotIndex}");
            return false;
        }
        
        // 귀속 특성은 제거 불가능하므로 교체 불가
        if (equippedTraits[slotIndex] != null && equippedTraits[slotIndex].IsBound())
        {
            Debug.LogWarning($"슬롯 {slotIndex}에 귀속 특성이 있어 교체할 수 없습니다.");
            return false;
        }
        
        // 일반 특성만 장착 가능
        if (trait.IsBound())
        {
            Debug.LogWarning("귀속 특성은 이 방법으로 장착할 수 없습니다. AcquireTrait을 사용하세요.");
            return false;
        }
        
        // 보유한 특성인지 확인
        if (!ownedTraits.Contains(trait))
        {
            Debug.LogWarning($"보유하지 않은 특성입니다: {trait.traitName}");
            return false;
        }
        
        // 기존 특성 효과 제거
        if (equippedTraits[slotIndex] != null)
        {
            equippedTraits[slotIndex].RemoveEffects(this);
        }
        
        // 새 특성 장착
        equippedTraits[slotIndex] = trait;
        trait.ApplyEffects(this);
        
        Debug.Log($"특성 장착: {trait.traitName} → 슬롯 {slotIndex}");
        OnTraitsUpdated?.Invoke();
        
        return true;
    }

    /// <summary>
    /// 특성 해제 (일반 특성만, 상태창에서 사용)
    /// </summary>
    public bool UnequipTrait(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxTraitSlots)
        {
            Debug.LogError($"잘못된 슬롯 인덱스: {slotIndex}");
            return false;
        }
        
        if (equippedTraits[slotIndex] == null)
        {
            Debug.LogWarning($"슬롯 {slotIndex}에 특성이 없습니다.");
            return false;
        }
        
        // 귀속 특성은 제거 불가능
        if (equippedTraits[slotIndex].IsBound())
        {
            Debug.LogWarning($"귀속 특성은 제거할 수 없습니다: {equippedTraits[slotIndex].traitName}");
            return false;
        }
        
        // 특성 효과 제거
        equippedTraits[slotIndex].RemoveEffects(this);
        equippedTraits[slotIndex] = null;
        
        Debug.Log($"특성 해제: 슬롯 {slotIndex}");
        OnTraitsUpdated?.Invoke();
        
        return true;
    }

    /// <summary>
    /// 특성 보유 여부 확인
    /// </summary>
    public bool HasTrait(Trait trait)
    {
        if (trait == null) return false;
        
        // 장착된 특성 중 확인
        foreach (var equippedTrait in equippedTraits)
        {
            if (equippedTrait == trait)
            {
                return true;
            }
        }
        
        // 보유한 특성 중 확인 (일반 특성)
        return ownedTraits.Contains(trait);
    }

    /// <summary>
    /// 빈 특성 슬롯 찾기
    /// </summary>
    private int FindEmptyTraitSlot()
    {
        for (int i = 0; i < equippedTraits.Count; i++)
        {
            if (equippedTraits[i] == null)
            {
                return i;
            }
        }
        return -1; // 빈 슬롯 없음
    }

    /// <summary>
    /// 특정 슬롯의 특성 조회
    /// </summary>
    public Trait GetTraitAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedTraits.Count)
        {
            return null;
        }
        return equippedTraits[slotIndex];
    }

    /// <summary>
    /// 현재 장착된 특성 개수 조회
    /// </summary>
    public int GetEquippedTraitCount()
    {
        int count = 0;
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 퀘스트 완료로 특성 획득
    /// </summary>
    public bool AcquireTraitFromQuest(Trait trait, string questName)
    {
        return AcquireTrait(trait, $"퀘스트: {questName}");
    }

    /// <summary>
    /// 전투 보상으로 특성 획득
    /// </summary>
    public bool AcquireTraitFromBattle(Trait trait, string enemyName = "")
    {
        return AcquireTrait(trait, $"전투 보상: {enemyName}");
    }

    // ========== 최종 스테이터스 계산 시스템 ==========

    /// <summary>
    /// 최종 근력 계산 (기본 + 장비 + 특성 + 정신붕괴)
    /// </summary>
    public int GetFinalStrength()
    {
        int total = strength;
        
        // 장비 보정
        if (equippedWeapon != null) total += equippedWeapon.strengthBonus;
        if (equippedTop != null) total += equippedTop.strengthBonus;
        if (equippedBottom != null) total += equippedBottom.strengthBonus;
        if (equippedAccessory != null) total += equippedAccessory.strengthBonus;
        
        // 특성 보정
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                total += trait.GetStatBonus(StatType.Strength);
            }
        }
        
        // 정신붕괴 보정
        if (currentMadnessState == MadnessState.WrathOfThatDay)
            total += 4;
        else if (currentMadnessState == MadnessState.UnkillablePain)
            total += 3;
        else if (currentMadnessState == MadnessState.IncomprehensibleFear)
            total -= 2;
        
        return total;
    }

    /// <summary>
    /// 최종 기량 계산
    /// </summary>
    public int GetFinalDexterity()
    {
        int total = dexterity;
        
        if (equippedWeapon != null) total += equippedWeapon.dexterityBonus;
        if (equippedTop != null) total += equippedTop.dexterityBonus;
        if (equippedBottom != null) total += equippedBottom.dexterityBonus;
        if (equippedAccessory != null) total += equippedAccessory.dexterityBonus;
        
        // 특성 보정
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                total += trait.GetStatBonus(StatType.Dexterity);
            }
        }
        
        return total;
    }

    /// <summary>
    /// 최종 체력 계산
    /// </summary>
    public int GetFinalVitality()
    {
        int total = vitality;
        
        if (equippedWeapon != null) total += equippedWeapon.vitalityBonus;
        if (equippedTop != null) total += equippedTop.vitalityBonus;
        if (equippedBottom != null) total += equippedBottom.vitalityBonus;
        if (equippedAccessory != null) total += equippedAccessory.vitalityBonus;
        
        // 특성 보정
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                total += trait.GetStatBonus(StatType.Vitality);
            }
        }
        
        return total;
    }

    /// <summary>
    /// 최종 정신력 계산
    /// </summary>
    public int GetFinalWillpower()
    {
        int total = willpower;
        
        if (equippedWeapon != null) total += equippedWeapon.willpowerBonus;
        if (equippedTop != null) total += equippedTop.willpowerBonus;
        if (equippedBottom != null) total += equippedBottom.willpowerBonus;
        if (equippedAccessory != null) total += equippedAccessory.willpowerBonus;
        
        // 특성 보정
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                total += trait.GetStatBonus(StatType.Willpower);
            }
        }
        
        return total;
    }

    /// <summary>
    /// 최종 운 계산
    /// </summary>
    public int GetFinalLuck()
    {
        int total = luck;
        
        if (equippedWeapon != null) total += equippedWeapon.luckBonus;
        if (equippedTop != null) total += equippedTop.luckBonus;
        if (equippedBottom != null) total += equippedBottom.luckBonus;
        if (equippedAccessory != null) total += equippedAccessory.luckBonus;
        
        // 특성 보정
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                total += trait.GetStatBonus(StatType.Luck);
            }
        }
        
        // 정신붕괴 보정
        if (currentMadnessState == MadnessState.IncomprehensibleFear)
            total += 2;
        else if (currentMadnessState == MadnessState.UnkillablePain)
            total += 3;
        
        return total;
    }

    /// <summary>
    /// 최종 공격력 계산 (strength 기반 + 무기 공격력 + 숙련도 + 특성)
    /// </summary>
    public int GetFinalAttackPower()
    {
        int baseAttack = 10; // 기본 공격력
        int strengthBonus = GetFinalStrength() * 2; // 근력 보너스
        
        // 무기 공격력
        int weaponAttack = 0;
        if (equippedWeapon != null)
        {
            weaponAttack = equippedWeapon.attackPower;
        }
        
        // 숙련도 보너스
        int proficiencyBonus = 0;
        if (equippedWeapon != null && equippedWeapon.IsWeapon())
        {
            proficiencyBonus = GetProficiencyDamageBonus(equippedWeapon.weaponCategory);
        }
        
        // 특성 데미지 보너스
        int traitDamageBonus = 0;
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                traitDamageBonus += trait.GetDamageBonus();
            }
        }
        
        return baseAttack + strengthBonus + weaponAttack + proficiencyBonus + traitDamageBonus;
    }

    /// <summary>
    /// 최종 방어력 계산 (vitality 기반 + 장비 방어력 + 숙련도 + 특성)
    /// </summary>
    public int CalculateDefense()
    {
        int baseDefense = 0;
        int vitalityBonus = GetFinalVitality() * 1; // 체력 보너스
        
        // 장비 방어력
        int equipmentDefense = 0;
        if (equippedWeapon != null) equipmentDefense += equippedWeapon.defensePower;
        if (equippedTop != null) equipmentDefense += equippedTop.defensePower;
        if (equippedBottom != null) equipmentDefense += equippedBottom.defensePower;
        if (equippedAccessory != null) equipmentDefense += equippedAccessory.defensePower;
        
        // 숙련도 보너스
        int proficiencyBonus = 0;
        if (equippedWeapon != null && equippedWeapon.IsWeapon())
        {
            proficiencyBonus = GetProficiencyDefenseBonus(equippedWeapon.weaponCategory);
        }
        
        // 특성 방어력 보너스
        int traitDefenseBonus = 0;
        foreach (var trait in equippedTraits)
        {
            if (trait != null)
            {
                traitDefenseBonus += trait.GetDefenseBonus();
            }
        }
        
        return baseDefense + vitalityBonus + equipmentDefense + proficiencyBonus + traitDefenseBonus;
    }

    /// <summary>
    /// 치명타 확률 계산 (dexterity 기반)
    /// </summary>
    public float CalculateCriticalChance()
    {
        int finalDexterity = GetFinalDexterity();
        return Mathf.Clamp01(finalDexterity * 0.01f); // 기량당 1%
    }

    /// <summary>
    /// 명중률 계산 (dexterity 기반)
    /// </summary>
    public float CalculateHitRate()
    {
        int finalDexterity = GetFinalDexterity();
        return Mathf.Clamp01(0.8f + (finalDexterity * 0.02f)); // 기본 80% + 기량당 2%
    }

    /// <summary>
    /// 회피율 계산 (luck 기반)
    /// </summary>
    public float CalculateDodgeRate()
    {
        int finalLuck = GetFinalLuck();
        return Mathf.Clamp01(finalLuck * 0.01f); // 운당 1%
    }

    /// <summary>
    /// 파생 스테이터스 재계산 (maxHP 등)
    /// </summary>
    public void RecalculateDerivedStats()
    {
        // vitality → maxHP 계산
        int finalVitality = GetFinalVitality();
        maxHP = baseMaxHP + (finalVitality * 10); // 체력당 10 HP
        
        // 현재 HP가 최대 HP를 초과하지 않도록
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    /// <summary>
    /// 장비 변경 시 파생 스테이터스 재계산
    /// </summary>
    private void OnEquipmentChanged()
    {
        RecalculateDerivedStats();
        OnCharacterUpdated?.Invoke();
    }

}
