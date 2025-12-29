using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 타이틀 씬의 전체 흐름을 관리하는 메인 매니저
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    [Header("메인 메뉴 UI")]
    [Tooltip("New Game 버튼")]
    [SerializeField] private Button newGameButton;
    
    [Tooltip("Continue 버튼")]
    [SerializeField] private Button continueButton;
    
    [Tooltip("Setting 버튼")]
    [SerializeField] private Button settingButton;
    
    [Tooltip("Exit 버튼")]
    [SerializeField] private Button exitButton;
    
    [Header("로드 게임 창")]
    [Tooltip("LOAD_GAME 창 GameObject (New Game 클릭 시 활성화)")]
    [SerializeField] private GameObject loadGameWindow;
    
    [Tooltip("LOAD_FILE 슬롯 프리팹들 (LoadFileSlot 컴포넌트가 있는 GameObject들)")]
    [SerializeField] private LoadFileSlot[] loadFileSlots = new LoadFileSlot[3];
    
    [Header("CH CHOOSE 프리팹")]
    [Tooltip("CH CHOOSE 프리팹 (LoadFileSlot에서 사용)")]
    [SerializeField] private GameObject chChoosePrefab;
    
    [Tooltip("CH CHOOSE를 생성할 부모 Transform (보통 Canvas)")]
    [SerializeField] private Transform chChooseParent;
    
    [Header("디버그")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private void Start()
    {
        InitializeButtons();
        InitializeWindows();
        InitializeLoadFileSlots();
        CheckContinueButton();
    }
    
    /// <summary>
    /// 버튼 이벤트 초기화
    /// </summary>
    private void InitializeButtons()
    {
        // New Game 버튼
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(OnNewGameClicked);
        }
        else
        {
            Debug.LogError("[TitleSceneManager] newGameButton이 설정되지 않았습니다.");
        }
        
        // Continue 버튼
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        // Setting 버튼
        if (settingButton != null)
        {
            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(OnSettingClicked);
        }
        
        // Exit 버튼
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnExitClicked);
        }
    }
    
    /// <summary>
    /// 창 초기화
    /// </summary>
    private void InitializeWindows()
    {
        // LOAD_GAME 창은 처음에 비활성화
        if (loadGameWindow != null)
        {
            loadGameWindow.SetActive(false);
        }
    }
    
    /// <summary>
    /// LOAD_FILE 슬롯들 초기화
    /// </summary>
    private void InitializeLoadFileSlots()
    {
        // 부모가 설정되지 않았으면 Canvas 찾기
        if (chChooseParent == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                chChooseParent = canvas.transform;
            }
        }
        
        // 각 슬롯에 필요한 정보 설정
        for (int i = 0; i < loadFileSlots.Length; i++)
        {
            if (loadFileSlots[i] != null)
            {
                loadFileSlots[i].SetSlotNumber(i + 1);
                
                if (chChoosePrefab != null)
                {
                    loadFileSlots[i].SetChChoosePrefab(chChoosePrefab);
                }
                
                if (chChooseParent != null)
                {
                    loadFileSlots[i].SetChChooseParent(chChooseParent);
                }
                
                if (loadGameWindow != null)
                {
                    loadFileSlots[i].SetLoadGameWindow(loadGameWindow);
                }
            }
        }
    }
    
    /// <summary>
    /// Continue 버튼 활성화/비활성화 확인
    /// </summary>
    private void CheckContinueButton()
    {
        if (continueButton == null)
        {
            return;
        }
        
        // 저장된 게임 데이터가 있는지 확인
        bool hasSaveData = HasAnySaveData();
        
        // 저장 데이터가 없으면 버튼 GameObject 자체를 비활성화
        continueButton.gameObject.SetActive(hasSaveData);
        
        if (hasSaveData)
        {
            LogDebug("저장된 게임 데이터가 있어 Continue 버튼을 활성화했습니다.");
        }
        else
        {
            LogDebug("저장된 게임 데이터가 없어 Continue 버튼을 비활성화했습니다.");
        }
    }
    
    /// <summary>
    /// 저장된 게임 데이터가 있는지 확인
    /// </summary>
    private bool HasAnySaveData()
    {
        // TODO: 나중에 SaveLoadManager와 연동
        // 현재는 PlayerPrefs로 간단하게 확인
        
        // 슬롯 1, 2, 3 중 하나라도 저장 데이터가 있으면 true
        for (int i = 1; i <= 3; i++)
        {
            if (HasSaveDataInSlot(i))
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 특정 슬롯에 저장 데이터가 있는지 확인
    /// </summary>
    private bool HasSaveDataInSlot(int slotNumber)
    {
        // PlayerPrefs를 사용한 간단한 확인
        // 키 형식: "SaveData_Slot{번호}_Exists"
        string key = $"SaveData_Slot{slotNumber}_Exists";
        
        // PlayerPrefs에 키가 있고 값이 1이면 저장 데이터 있음
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key, 0) == 1;
        }
        
        // 또는 더 구체적인 저장 데이터 확인
        // 예: "SaveData_Slot{번호}_CharacterName" 등
        string characterKey = $"SaveData_Slot{slotNumber}_CharacterName";
        if (PlayerPrefs.HasKey(characterKey))
        {
            string characterName = PlayerPrefs.GetString(characterKey, "");
            return !string.IsNullOrEmpty(characterName);
        }
        
        return false;
    }
    
    /// <summary>
    /// New Game 버튼 클릭 처리
    /// </summary>
    private void OnNewGameClicked()
    {
        LogDebug("New Game 버튼 클릭됨");
        
        if (loadGameWindow != null)
        {
            loadGameWindow.SetActive(true);
            LogDebug("LOAD_GAME 창이 열렸습니다.");
        }
        else
        {
            Debug.LogError("[TitleSceneManager] loadGameWindow가 설정되지 않았습니다.");
        }
    }
    
    /// <summary>
    /// Continue 버튼 클릭 처리
    /// </summary>
    private void OnContinueClicked()
    {
        LogDebug("Continue 버튼 클릭됨");
        // TODO: 저장 데이터 확인 및 로드
    }
    
    /// <summary>
    /// Setting 버튼 클릭 처리
    /// </summary>
    private void OnSettingClicked()
    {
        LogDebug("Setting 버튼 클릭됨");
        // TODO: 설정 창 열기
    }
    
    /// <summary>
    /// Exit 버튼 클릭 처리
    /// </summary>
    private void OnExitClicked()
    {
        LogDebug("Exit 버튼 클릭됨");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    /// <summary>
    /// 디버그 로그 출력
    /// </summary>
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[TitleSceneManager] {message}");
        }
    }
}



