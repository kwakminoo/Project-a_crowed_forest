using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using System;
using Unity.VisualScripting;
using Cinemachine;

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
    private Camera mainCamera;

    [Header("Cinemachine Camera System")]
    [SerializeField] private BattleConfig battleConfig;
    private CinemachineBrain cinemachineBrain;
    private CinemachineVirtualCamera defaultVCam;
    private CinemachineVirtualCamera playerVCam;
    private CinemachineVirtualCamera enemyVCam;
    private CinemachineVirtualCamera skillFocusVCam;
    private Transform playerCameraTarget;
    private Transform enemyCameraTarget;

    private bool isBattleActive = true;

    public DialogueRunner dialogueRunner;
    private string nextNode;

    //데이터 초기화
    private void Start()
    {
        InitializeComponents();
        InitializeBattleSystem();
    }

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void InitializeComponents()
    {
        mainCanvas = FindFirstObjectByType<Canvas>();
        mainCamera = Camera.main;
        
        if(mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        if(mainCanvas != null && mainCamera != null)
        {
            mainCanvas.worldCamera = mainCamera;
            Debug.Log("Canvas의 Event Camera가 Main Camera로 초기 설정되었습니다.");
        }

        // BattleConfig 로드
        if (battleConfig == null)
        {
            battleConfig = Resources.Load<BattleConfig>("BattleConfig");
            if (battleConfig == null)
            {
                battleConfig = ScriptableObject.CreateInstance<BattleConfig>();
                Debug.LogWarning("BattleConfig를 찾을 수 없습니다. 기본 설정을 사용합니다.");
            }
        }

        // 기존 컴포넌트 초기화
        inventory = Inventory.Instance ?? throw new NullReferenceException("Inventory.Instance가 null입니다.");
        player = playerObject.GetComponent<Player>() ?? Player.Instance;
        Player.Instance.OnCharacterUpdated += UpdateBattleCharacterImage;
        currentEnemy = enemyObject.GetComponent<EnemyScript>();

        // Cinemachine 시스템 초기화
        InitializeCinemachineSystem();

        if(player == null)
        {
            Debug.LogError("player 오브젝트를 찾을 수 없습니다");
        }

        if(currentEnemy == null)
        {
            Debug.LogError("enemyObject를 찾을 수 없습니다");
        }

        Debug.Log("Inventory 인스턴스 상태: " + (inventory != null ? "정상" : "null"));
    }

    /// <summary>
    /// Cinemachine 카메라 시스템 초기화
    /// </summary>
    private void InitializeCinemachineSystem()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다. Cinemachine 시스템을 초기화할 수 없습니다.");
            return;
        }

        // 메인 카메라 설정 자동화
        SetupMainCamera();

        // CinemachineBrain 추가 또는 찾기
        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        if (cinemachineBrain == null)
        {
            cinemachineBrain = mainCamera.gameObject.AddComponent<CinemachineBrain>();
            Debug.Log("CinemachineBrain이 Main Camera에 추가되었습니다.");
        }

        // CinemachineBrain 설정
        if (battleConfig != null)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = battleConfig.cameraBlendTime;
            // CinemachineBlendDefinition.Style enum 사용 (Styles 아님)
            cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        }

        // 카메라 타겟 생성 (플레이어/적 위치를 추적하기 위한 빈 오브젝트)
        CreateCameraTargets();

        // Virtual Camera 생성
        CreateVirtualCameras();

        Debug.Log("✅ Cinemachine 카메라 시스템 초기화 완료");
    }

    /// <summary>
    /// 메인 카메라 설정 자동화 (World Space Canvas 지원)
    /// </summary>
    private void SetupMainCamera()
    {
        if (mainCamera == null) return;

        // 1. Projection을 Orthographic으로 설정 (2D 게임)
        mainCamera.orthographic = true;
        
        // 2. Orthographic Size 설정 (기본값 유지 또는 적절한 값)
        if (mainCamera.orthographicSize <= 0)
        {
            mainCamera.orthographicSize = 5f; // 기본값
        }

        // 3. Depth 설정 (다른 카메라보다 높게)
        mainCamera.depth = 0;

        // 4. Culling Mask에 UI 레이어 포함 확인
        // UI 레이어가 없으면 기본 레이어만 사용 (Everything)
        // UI 레이어가 있으면 추가
        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer != -1)
        {
            mainCamera.cullingMask |= (1 << uiLayer);
        }
        // 기본 레이어도 포함
        mainCamera.cullingMask |= (1 << 0); // Default 레이어

        // 5. Canvas의 Event Camera 설정 (이미 InitializeComponents에서 설정됨)
        // 추가 확인
        if (mainCanvas != null)
        {
            mainCanvas.worldCamera = mainCamera;
        }

        Debug.Log($"✅ 메인 카메라 설정 완료: Orthographic={mainCamera.orthographic}, Size={mainCamera.orthographicSize}, Depth={mainCamera.depth}");
    }

    /// <summary>
    /// 카메라 타겟 생성 (플레이어/적 위치 추적용)
    /// </summary>
    private void CreateCameraTargets()
    {
        // 플레이어 카메라 타겟
        if (playerObject != null)
        {
            playerCameraTarget = new GameObject("PlayerCameraTarget").transform;
            playerCameraTarget.SetParent(playerObject.transform);
            playerCameraTarget.localPosition = Vector3.zero; // 플레이어 중심
        }

        // 적 카메라 타겟
        if (enemyObject != null)
        {
            enemyCameraTarget = new GameObject("EnemyCameraTarget").transform;
            enemyCameraTarget.SetParent(enemyObject.transform);
            enemyCameraTarget.localPosition = Vector3.zero; // 적 중심
        }
    }

    /// <summary>
    /// Virtual Camera 생성
    /// </summary>
    private void CreateVirtualCameras()
    {
        GameObject vcamContainer = new GameObject("CinemachineVirtualCameras");
        // BattleManager의 위치에 따라 달라지지 않도록 씬 루트에 배치
        vcamContainer.transform.SetParent(null);
        vcamContainer.transform.position = Vector3.zero;

        // 1. 기본 Virtual Camera (전체 전투 뷰)
        defaultVCam = CreateVCam("DefaultVCam", vcamContainer.transform, battleConfig?.defaultVCamPriority ?? 5);
        if (defaultVCam != null)
        {
            // 기본 카메라는 전체 전투 장면을 보여줌
            // Follow와 LookAt을 설정하지 않으면 고정된 뷰를 유지
            var composer = defaultVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (composer != null)
            {
                composer.m_DeadZoneWidth = 0.2f;
                composer.m_DeadZoneHeight = 0.2f;
            }
            
            // 기본 카메라는 메인 카메라의 위치를 유지
            if (mainCamera != null)
            {
                defaultVCam.transform.position = mainCamera.transform.position;
            }
        }

        // 2. 플레이어 Virtual Camera
        if (playerCameraTarget != null)
        {
            playerVCam = CreateVCam("PlayerVCam", vcamContainer.transform, battleConfig?.playerVCamPriority ?? 10);
            if (playerVCam != null)
            {
                playerVCam.Follow = playerCameraTarget;
                playerVCam.LookAt = playerCameraTarget;
            }
        }

        // 3. 적 Virtual Camera
        if (enemyCameraTarget != null)
        {
            enemyVCam = CreateVCam("EnemyVCam", vcamContainer.transform, battleConfig?.enemyVCamPriority ?? 11);
            if (enemyVCam != null)
            {
                enemyVCam.Follow = enemyCameraTarget;
                enemyVCam.LookAt = enemyCameraTarget;
            }
        }

        // 4. 스킬 포커스 Virtual Camera (동적으로 타겟 변경)
        skillFocusVCam = CreateVCam("SkillFocusVCam", vcamContainer.transform, battleConfig?.skillFocusVCamPriority ?? 20);
        if (skillFocusVCam != null)
        {
            // 스킬 포커스 카메라는 기본적으로 비활성화
            skillFocusVCam.Priority = 0;
        }

        // 기본 카메라 활성화
        if (defaultVCam != null)
        {
            defaultVCam.Priority = battleConfig?.defaultVCamPriority ?? 5;
        }
    }

    /// <summary>
    /// Virtual Camera 생성 헬퍼 메서드
    /// </summary>
    private CinemachineVirtualCamera CreateVCam(string name, Transform parent, int priority)
    {
        GameObject vcamObj = new GameObject(name);
        vcamObj.transform.SetParent(parent);

        CinemachineVirtualCamera vcam = vcamObj.AddComponent<CinemachineVirtualCamera>();
        vcam.Priority = priority;

        // 2D 게임이므로 Orthographic 설정
        // 메인 카메라의 Orthographic Size를 그대로 사용
        if (mainCamera != null)
        {
            vcam.m_Lens.Orthographic = true;
            vcam.m_Lens.OrthographicSize = mainCamera.orthographicSize;
        }
        else
        {
            // 메인 카메라가 없으면 기본값 사용
            vcam.m_Lens.Orthographic = true;
            vcam.m_Lens.OrthographicSize = 5f;
        }

        // Framing Transposer 추가 (2D 추적용)
        var transposer = vcam.AddCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            transposer.m_DeadZoneWidth = 0f;
            transposer.m_DeadZoneHeight = 0f;
            transposer.m_SoftZoneWidth = 0.8f;
            transposer.m_SoftZoneHeight = 0.8f;
        }

        return vcam;
    }

    /// <summary>
    /// 전투 시스템 초기화
    /// </summary>
    private void InitializeBattleSystem()
    {
        if (battleCharacterImage != null && player != null)
        {
            battleCharacterImage.sprite = player.baseCharacterSprite;
        }

        InitialzeSkillButtons();
        UpdateBattleState();

        //플레이어 죽는 이벤트 연결
        if (player != null)
        {
            player.OnPlayerDeath += HandlePlayerDeath;
        }
    }

    /// <summary>
    /// 전투 상태 변경 처리 (현재 비활성화)
    /// </summary>
    /*
    private void HandleBattleStateChanged(BattleState previousState, BattleState newState)
    {
        if (battleConfig != null && battleConfig.logBattleStateChanges)
        {
            Debug.Log($"[BattleManager] 상태 변경: {previousState} → {newState}");
        }

        // 상태별 추가 처리
        switch (newState)
        {
            case BattleState.BattleEnded:
                HandleBattleEnded();
                break;
        }
    }

    /// <summary>
    /// 전투 종료 처리
    /// </summary>
    private void HandleBattleEnded()
    {
        isBattleActive = false;
        // 기존 EndBattle 로직은 유지
    }
    */

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
        // 전투 상태 초기화
        isBattleActive = true;
        
        // 상태 관리자 초기화 (현재 비활성화)
        // if (stateManager != null)
        // {
        //     stateManager.InitializeBattle();
        // }
        
        // 대화 시스템 처리
        HandleDialogueSystem(nextYarnNode);
        
        // 적 초기화
        if (!InitializeEnemy(enemyData))
        {
            return;
        }
        
        // 배경 설정
        SetupBattleBackground(backGroundName);
        
        // UI 초기화
        InitializeBattleUI();
        
        // 전투 시작
        StartCoroutine(BattleRoutine());
    }

    /// <summary>
    /// 대화 시스템 처리
    /// </summary>
    private void HandleDialogueSystem(string nextYarnNode)
    {
        if (dialogueRunner != null)
        {
            if (dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.Stop();
                Debug.Log("DialogueRunner: 스토리 출력 중단");
            }
            nextNode = nextYarnNode;
        }
    }

    /// <summary>
    /// 적 초기화
    /// </summary>
    private bool InitializeEnemy(EnemyData enemyData)
    {
        currentEnemy = enemyObject.GetComponent<EnemyScript>();
        if (currentEnemy == null)
        {
            Debug.LogError("EnemyObject에 EnemyScript가 없습니다.");
            return false;
        }
        
        if (enemyData == null)
        {
            Debug.LogError("적 데이터가 null입니다. 초기화를 중단합니다.");
            return false;
        }

        currentEnemy.InitializeEnemy(enemyData);
        currentEnemy.currentHP = enemyData.maxHP;
        enemyNameText.text = enemyData.enemyName;
        enemyImage.sprite = enemyData.enemySprite;

        if (currentEnemy.currentHP <= 0)
        {
            Debug.LogError($"❌ {enemyData.enemyName}의 초기 체력이 0입니다. 전투 시작 불가능.");
            EndBattle();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 배경 설정
    /// </summary>
    private void SetupBattleBackground(string backGroundName)
    {
        Sprite backGroundSprite = Resources.Load<Sprite>($"Battle Background/{backGroundName}");
        if (backGroundSprite != null)
        {
            backGroundImage.sprite = backGroundSprite;
        }
        else
        {
            Debug.LogError($"{backGroundName}을 찾을 수 없습니다");
        }
    }

    /// <summary>
    /// 전투 UI 초기화
    /// </summary>
    private void InitializeBattleUI()
    {
        // 기본 카메라로 전환
        SwitchToDefaultCamera();
        
        battleWindow.SetActive(true);
        UpdateBattleCharacterImage();
        UpdateBattleState();
        InitialzeSkillButtons();
    }

    /// <summary>
    /// 전투 흐름 메인 루프
    /// </summary>
    private IEnumerator BattleRoutine()
    {
        Debug.Log("전투 루틴 시작");
        
        while (isBattleActive)
        {
            // 적 턴
            yield return StartCoroutine(EnemyTurn());

            if (!isBattleActive) break;

            // 플레이어 턴
            yield return StartCoroutine(PlayerTurn());
        }

        Debug.Log("전투 루틴 종료");
        EndBattle();
    }

    /// <summary>
    /// 적 턴 처리
    /// </summary>
    private IEnumerator EnemyTurn()
    {
        Debug.Log($"{currentEnemy.enemyData.enemyName}의 턴 시작");

        yield return new WaitForSeconds(1f);

        // 1. 적 카메라로 전환
        FocusOnTarget(enemyCameraTarget, battleConfig?.skillCameraBlendTime ?? 0.3f);
        yield return new WaitForSeconds(battleConfig?.skillCameraBlendTime ?? 0.3f);

        // 2. 적 스킬 사용 (스킬 이미지 변경 등)
        currentEnemy.UseSkill(player);

        // 3. 스킬 포커스 (줌 인)
        FocusOnTargetWithZoom(enemyCameraTarget, battleConfig?.skillCameraZoom ?? 2f, battleConfig?.skillCameraBlendTime ?? 0.3f);
        yield return new WaitForSeconds(battleConfig?.skillCameraFocusDuration ?? 1.5f);

        // 4. 기본 카메라로 복귀
        SwitchToDefaultCamera();
        yield return new WaitForSeconds(0.3f);

        UpdateBattleState();

        Debug.Log($"{currentEnemy.enemyData.enemyName}의 턴 종료");

        if(player.currentHP <= 0)
        {
            isBattleActive = false;
            EndBattle();
        }
    }

    /// <summary>
    /// 플레이어 턴 처리
    /// </summary>
    private IEnumerator PlayerTurn()
    {
        Debug.Log("player턴 시작");

        bool isActionCompleted = false;

        //스킬 버튼 활성화 및 클릭 대기
        List<Skill> battleSkills = player.GetBattleSkills() ?? new List<Skill>();
        
        for(int i = 0; i < skillButtons.Count; i++)
        {
            if (skillButtons[i] == null)
            {
                Debug.LogError($"스킬 버튼 {i}번이 null입니다.");
                continue;
            }

            int skillIndex = i;
            
            // 버튼 활성화 및 interactable 설정
            skillButtons[i].gameObject.SetActive(true);
            skillButtons[i].interactable = true;
            
            // 스킬이 있는 버튼만 클릭 가능하도록 설정
            if (i < battleSkills.Count && battleSkills[i] != null)
            {
                skillButtons[i].interactable = true;
                skillButtons[i].onClick.RemoveAllListeners();
                skillButtons[i].onClick.AddListener(() =>
                {
                    if (!isActionCompleted) // 중복 클릭 방지
                    {
                        Debug.Log($"스킬 버튼 {skillIndex} 클릭됨");
                        UseSkill(skillIndex);
                        isActionCompleted = true;
                    }
                });
            }
            else
            {
                // 스킬이 없는 버튼은 비활성화
                skillButtons[i].interactable = false;
            }
        }

        // Canvas의 Event Camera 확인
        if (mainCanvas != null && mainCanvas.worldCamera == null)
        {
            mainCanvas.worldCamera = mainCamera;
            Debug.LogWarning("Canvas의 Event Camera가 null이었습니다. Main Camera로 설정했습니다.");
        }

        yield return new WaitUntil(() => isActionCompleted);

        UpdateBattleState();

        if(currentEnemy.currentHP <= 0)
        {
            isBattleActive = false;
            EndBattle();
        }

        yield return new WaitForSeconds(1f); //플레이어 턴이 끝난 후 짧은 대기 
    }

    
    public void EndBattle()
    {
        SwitchToDefaultCamera();
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
        // var lineView = FindObjectOfType<CustomLineView>();
        // if (lineView != null)
        // {
        //     lineView.RestorePreviousBGM(); // ✅ 전투 종료 후 원래 BGM 복귀
        // }
        
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
        
        // 스킬 카메라 연출 시작
        StartCoroutine(ExecuteSkillWithCamera(skill, currentEnemy.gameObject));
    }

    /// <summary>
    /// 스킬 실행 및 카메라 연출
    /// </summary>
    private IEnumerator ExecuteSkillWithCamera(Skill skill, GameObject target)
    {
        if (skill == null || target == null)
        {
            Debug.LogError("스킬 또는 타겟이 null입니다.");
            yield break;
        }

        // 1. 플레이어 카메라로 전환 (스킬 준비)
        FocusOnTarget(playerCameraTarget, battleConfig?.skillCameraBlendTime ?? 0.3f);
        yield return new WaitForSeconds(battleConfig?.skillCameraBlendTime ?? 0.3f);

        // 2. 스킬 이펙트 표시
        StartCoroutine(ShowSkillEffect(skill.skillSprite));

        // 3. 타겟(적)으로 카메라 포커스 (줌 인)
        FocusOnTargetWithZoom(enemyCameraTarget, battleConfig?.skillCameraZoom ?? 2f, battleConfig?.skillCameraBlendTime ?? 0.3f);
        yield return new WaitForSeconds(battleConfig?.skillCameraBlendTime ?? 0.3f);

        // 4. 스킬 실행
        skill.ExecuteSkill(player.gameObject, target, this);
        
        // 5. 스킬 포커스 유지
        yield return new WaitForSeconds(battleConfig?.skillCameraFocusDuration ?? 1.5f);

        // 6. 기본 카메라로 복귀
        SwitchToDefaultCamera();
        yield return new WaitForSeconds(0.3f);
        
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
    
    // ========== Cinemachine 카메라 제어 메서드 ==========

    /// <summary>
    /// 기본 카메라로 전환 (전체 전투 뷰)
    /// </summary>
    public void SwitchToDefaultCamera()
    {
        if (defaultVCam != null)
        {
            defaultVCam.Priority = battleConfig?.defaultVCamPriority ?? 5;
            playerVCam.Priority = 0;
            enemyVCam.Priority = 0;
            skillFocusVCam.Priority = 0;
        }
    }

    /// <summary>
    /// 특정 타겟에 포커스 (플레이어 또는 적)
    /// </summary>
    public void FocusOnTarget(Transform target, float blendTime = 0.5f)
    {
        if (target == null)
        {
            Debug.LogError("카메라 타겟이 null입니다.");
            return;
        }

        if (cinemachineBrain != null && battleConfig != null)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = blendTime;
        }

        // 타겟에 따라 적절한 VCam 활성화
        if (target == playerCameraTarget && playerVCam != null)
        {
            playerVCam.Priority = battleConfig?.playerVCamPriority ?? 10;
            enemyVCam.Priority = 0;
            skillFocusVCam.Priority = 0;
            defaultVCam.Priority = 0;
        }
        else if (target == enemyCameraTarget && enemyVCam != null)
        {
            enemyVCam.Priority = battleConfig?.enemyVCamPriority ?? 11;
            playerVCam.Priority = 0;
            skillFocusVCam.Priority = 0;
            defaultVCam.Priority = 0;
        }
    }

    /// <summary>
    /// 특정 타겟에 줌 인하여 포커스 (스킬 사용 시)
    /// </summary>
    public void FocusOnTargetWithZoom(Transform target, float zoomLevel, float blendTime = 0.3f)
    {
        if (target == null || skillFocusVCam == null)
        {
            Debug.LogError("카메라 타겟 또는 SkillFocusVCam이 null입니다.");
            return;
        }

        if (cinemachineBrain != null && battleConfig != null)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = blendTime;
        }

        // 스킬 포커스 카메라 설정
        skillFocusVCam.Follow = target;
        skillFocusVCam.LookAt = target;
        skillFocusVCam.m_Lens.OrthographicSize = (mainCamera != null ? mainCamera.orthographicSize : 5f) - zoomLevel;
        skillFocusVCam.Priority = battleConfig?.skillFocusVCamPriority ?? 20;

        // 다른 카메라 비활성화
        playerVCam.Priority = 0;
        enemyVCam.Priority = 0;
        defaultVCam.Priority = 0;
    }

    /// <summary>
    /// 적 스킬 사용 시 카메라 전환 (EnemyScript에서 호출)
    /// </summary>
    public void HandleSkillCameraTransition(Player player, SkillRuntimeData skill)
    {
        if (player == null || skill == null) return;

        // 적 카메라로 전환 후 스킬 포커스
        StartCoroutine(EnemySkillCameraSequence());
    }

    /// <summary>
    /// 적 스킬 카메라 시퀀스
    /// </summary>
    private IEnumerator EnemySkillCameraSequence()
    {
        // 적 카메라로 전환
        FocusOnTarget(enemyCameraTarget, battleConfig?.skillCameraBlendTime ?? 0.3f);
        yield return new WaitForSeconds(battleConfig?.skillCameraBlendTime ?? 0.3f);

        // 스킬 포커스 (줌 인)
        FocusOnTargetWithZoom(enemyCameraTarget, battleConfig?.skillCameraZoom ?? 2f, battleConfig?.skillCameraBlendTime ?? 0.3f);
        yield return new WaitForSeconds(battleConfig?.skillCameraFocusDuration ?? 1.5f);

        // 기본 카메라로 복귀
        SwitchToDefaultCamera();
    }

    /// <summary>
    /// 컴포넌트 정리
    /// </summary>
    private void OnDestroy()
    {
        // 이벤트 정리
        if (player != null)
        {
            player.OnPlayerDeath -= HandlePlayerDeath;
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnCharacterUpdated -= UpdateBattleCharacterImage;
        }
    }
}