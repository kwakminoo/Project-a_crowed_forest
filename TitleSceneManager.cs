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
    
    [Tooltip("LOAD_GAME 창의 LOAD_FILE 버튼들 (최대 3개)")]
    [SerializeField] private Button[] loadFileButtons = new Button[3];
    
    [Header("캐릭터 선택")]
    [Tooltip("CharacterSelector 컴포넌트 (CH CHOOSE 오브젝트에 연결)")]
    [SerializeField] private CharacterSelector characterSelector;
    
    [Header("디버그")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private void Start()
    {
        InitializeButtons();
        InitializeWindows();
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
        
        // LOAD_FILE 버튼들
        for (int i = 0; i < loadFileButtons.Length; i++)
        {
            int slotIndex = i; // 클로저를 위한 로컬 변수
            if (loadFileButtons[i] != null)
            {
                loadFileButtons[i].onClick.RemoveAllListeners();
                loadFileButtons[i].onClick.AddListener(() => OnLoadFileClicked(slotIndex));
            }
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
        
        // 캐릭터 선택 창도 처음에 비활성화
        if (characterSelector != null)
        {
            characterSelector.gameObject.SetActive(false);
        }
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
    /// LOAD_FILE 버튼 클릭 처리
    /// </summary>
    private void OnLoadFileClicked(int slotIndex)
    {
        LogDebug($"LOAD_FILE {slotIndex + 1} 버튼 클릭됨");
        
        // LOAD_GAME 창 닫기
        if (loadGameWindow != null)
        {
            loadGameWindow.SetActive(false);
        }
        
        // 캐릭터 선택 창 열기
        if (characterSelector != null)
        {
            characterSelector.OpenCharacterSelect();
        }
        else
        {
            Debug.LogError("[TitleSceneManager] characterSelector가 설정되지 않았습니다.");
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

