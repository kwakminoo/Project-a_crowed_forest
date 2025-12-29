using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 로드 파일 슬롯 프리팹용 컴포넌트
/// 각 슬롯을 클릭하면 저장 데이터가 없을 경우 CH CHOOSE 프리팹을 인스턴스화합니다.
/// </summary>
public class LoadFileSlot : MonoBehaviour
{
    [Header("슬롯 정보")]
    [Tooltip("슬롯 번호 (1, 2, 3)")]
    [SerializeField] private int slotNumber = 1;
    
    [Header("UI 참조")]
    [Tooltip("슬롯 버튼 (이 컴포넌트가 붙은 GameObject에 Button이 있어야 함)")]
    [SerializeField] private Button slotButton;
    
    [Tooltip("슬롯 번호를 표시할 텍스트 (선택사항)")]
    [SerializeField] private TextMeshProUGUI slotNumberText;
    
    [Tooltip("저장 데이터가 있는지 표시할 텍스트 (선택사항)")]
    [SerializeField] private TextMeshProUGUI saveDataText;
    
    [Header("프리팹 및 연결")]
    [Tooltip("CH CHOOSE 프리팹 (저장 데이터가 없을 때 인스턴스화됨)")]
    [SerializeField] private GameObject chChoosePrefab;
    
    [Tooltip("CH CHOOSE를 생성할 부모 Transform (보통 Canvas)")]
    [SerializeField] private Transform chChooseParent;
    
    [Tooltip("LOAD_GAME 창 (이 슬롯이 속한 부모 창)")]
    [SerializeField] private GameObject loadGameWindow;
    
    [Header("저장 데이터 확인")]
    [Tooltip("이 슬롯에 저장된 데이터가 있는지 (나중에 SaveLoadManager와 연동)")]
    [SerializeField] private bool hasSaveData = false;
    
    [Header("디버그")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // 현재 생성된 CH CHOOSE 인스턴스
    private GameObject currentChChooseInstance;
    
    private void Awake()
    {
        // Button 컴포넌트 자동 찾기
        if (slotButton == null)
        {
            slotButton = GetComponent<Button>();
        }
        
        // 슬롯 번호 텍스트 자동 설정
        if (slotNumberText != null)
        {
            slotNumberText.text = $"FILE {slotNumber}";
        }
        
        // 부모가 설정되지 않았으면 Canvas 찾기
        if (chChooseParent == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                chChooseParent = canvas.transform;
            }
        }
    }
    
    private void Start()
    {
        InitializeButton();
        CheckSaveData(); // 저장 데이터 확인
        UpdateSaveDataDisplay();
    }
    
    /// <summary>
    /// 버튼 이벤트 초기화
    /// </summary>
    private void InitializeButton()
    {
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }
        else
        {
            Debug.LogError($"[LoadFileSlot] Slot {slotNumber}: Button 컴포넌트를 찾을 수 없습니다.");
        }
    }
    
    /// <summary>
    /// 슬롯 클릭 처리
    /// </summary>
    private void OnSlotClicked()
    {
        LogDebug($"LOAD_FILE {slotNumber} 클릭됨");
        
        // 저장 데이터가 있으면 로드, 없으면 새 게임 시작
        if (hasSaveData)
        {
            // TODO: 저장 데이터 로드
            LogDebug($"Slot {slotNumber}: 저장된 데이터를 로드합니다.");
            // LoadGame();
        }
        else
        {
            // 저장 데이터가 없으면 CH CHOOSE 프리팹 인스턴스화
            OpenCharacterSelect();
        }
    }
    
    /// <summary>
    /// 캐릭터 선택 창 열기 (CH CHOOSE 프리팹 인스턴스화)
    /// </summary>
    private void OpenCharacterSelect()
    {
        if (chChoosePrefab == null)
        {
            Debug.LogError($"[LoadFileSlot] Slot {slotNumber}: chChoosePrefab이 설정되지 않았습니다.");
            return;
        }
        
        // 이미 인스턴스가 있으면 재사용
        if (currentChChooseInstance != null)
        {
            currentChChooseInstance.SetActive(true);
            
            // 위치를 (0, 0, 0)으로 재설정
            SetChChoosePosition(currentChChooseInstance);
            
            CharacterSelector selector = currentChChooseInstance.GetComponent<CharacterSelector>();
            if (selector != null)
            {
                selector.OpenCharacterSelect();
                selector.SetSlotNumber(slotNumber); // 슬롯 번호 전달
            }
            LogDebug($"Slot {slotNumber}: 기존 CH CHOOSE 인스턴스를 활성화하고 위치를 (0, 0, 0)으로 설정했습니다.");
            
            // LOAD_GAME 창은 비활성화하지 않음 (CH CHOOSE 창과 함께 표시)
            return;
        }
        
        // 새 인스턴스 생성
        if (chChooseParent == null)
        {
            Debug.LogError($"[LoadFileSlot] Slot {slotNumber}: chChooseParent가 설정되지 않았습니다.");
            return;
        }
        
        currentChChooseInstance = Instantiate(chChoosePrefab, chChooseParent);
        currentChChooseInstance.name = $"CH CHOOSE (Slot {slotNumber})";
        
        // 위치를 (0, 0, 0)으로 설정
        SetChChoosePosition(currentChChooseInstance);
        
        // CharacterSelector에 슬롯 번호 전달
        CharacterSelector characterSelector = currentChChooseInstance.GetComponent<CharacterSelector>();
        if (characterSelector != null)
        {
            characterSelector.SetSlotNumber(slotNumber);
            characterSelector.OpenCharacterSelect();
        }
        else
        {
            Debug.LogWarning($"[LoadFileSlot] Slot {slotNumber}: CH CHOOSE 프리팹에 CharacterSelector 컴포넌트가 없습니다.");
        }
        
        // LOAD_GAME 창은 비활성화하지 않음 (CH CHOOSE 창과 함께 표시)
        
        LogDebug($"Slot {slotNumber}: CH CHOOSE 프리팹을 인스턴스화했습니다.");
    }
    
    /// <summary>
    /// CH CHOOSE 인스턴스의 위치를 (0, 0, 0)으로 설정
    /// </summary>
    private void SetChChoosePosition(GameObject instance)
    {
        if (instance == null)
        {
            return;
        }
        
        RectTransform rectTransform = instance.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            LogDebug($"CH CHOOSE 위치를 (0, 0, 0)으로 설정했습니다.");
        }
        else
        {
            // RectTransform이 없으면 일반 Transform 사용
            instance.transform.localPosition = Vector3.zero;
            LogDebug($"CH CHOOSE 위치를 (0, 0, 0)으로 설정했습니다. (일반 Transform 사용)");
        }
    }
    
    /// <summary>
    /// 저장 데이터 확인 (나중에 SaveLoadManager와 연동)
    /// </summary>
    private void CheckSaveData()
    {
        // TODO: SaveLoadManager를 통해 실제 저장 데이터 확인
        // 예: hasSaveData = SaveLoadManager.Instance.HasSaveData(slotNumber);
        hasSaveData = false; // 임시로 항상 false
    }
    
    /// <summary>
    /// 저장 데이터 표시 업데이트
    /// </summary>
    private void UpdateSaveDataDisplay()
    {
        if (saveDataText != null)
        {
            if (hasSaveData)
            {
                saveDataText.text = "저장됨";
            }
            else
            {
                saveDataText.text = "비어있음";
            }
        }
    }
    
    /// <summary>
    /// 슬롯 번호 설정 (프리팹 인스턴스화 시 호출)
    /// </summary>
    public void SetSlotNumber(int number)
    {
        slotNumber = number;
        
        if (slotNumberText != null)
        {
            slotNumberText.text = $"FILE {slotNumber}";
        }
    }
    
    /// <summary>
    /// CH CHOOSE 프리팹 설정
    /// </summary>
    public void SetChChoosePrefab(GameObject prefab)
    {
        chChoosePrefab = prefab;
    }
    
    /// <summary>
    /// CH CHOOSE 부모 설정
    /// </summary>
    public void SetChChooseParent(Transform parent)
    {
        chChooseParent = parent;
    }
    
    /// <summary>
    /// LoadGameWindow 설정
    /// </summary>
    public void SetLoadGameWindow(GameObject window)
    {
        loadGameWindow = window;
    }
    
    /// <summary>
    /// 저장 데이터 상태 설정
    /// </summary>
    public void SetHasSaveData(bool hasData)
    {
        hasSaveData = hasData;
        UpdateSaveDataDisplay();
    }
    
    /// <summary>
    /// 디버그 로그 출력
    /// </summary>
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[LoadFileSlot Slot {slotNumber}] {message}");
        }
    }
}



