using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 창을 닫는 X 버튼용 컴포넌트
/// 이 컴포넌트를 X 버튼에 추가하면 클릭 시 지정된 창을 비활성화합니다.
/// </summary>
public class WindowCloseButton : MonoBehaviour
{
    [Header("닫을 창 설정")]
    [Tooltip("이 버튼을 클릭하면 비활성화될 창 GameObject")]
    [SerializeField] private GameObject targetWindow;
    
    [Tooltip("버튼 컴포넌트 (자동으로 찾음)")]
    [SerializeField] private Button closeButton;
    
    [Header("추가 옵션")]
    [Tooltip("창을 닫을 때 부모 창도 함께 닫을지")]
    [SerializeField] private bool closeParentWindow = false;
    
    [Tooltip("부모 창 GameObject (closeParentWindow가 true일 때 사용)")]
    [SerializeField] private GameObject parentWindow;
    
    [Header("디버그")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private void Awake()
    {
        // Button 컴포넌트 자동 찾기
        if (closeButton == null)
        {
            closeButton = GetComponent<Button>();
        }
    }
    
    private void Start()
    {
        InitializeButton();
    }
    
    /// <summary>
    /// 버튼 이벤트 초기화
    /// </summary>
    private void InitializeButton()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        else
        {
            Debug.LogError("[WindowCloseButton] Button 컴포넌트를 찾을 수 없습니다.");
        }
    }
    
    /// <summary>
    /// 닫기 버튼 클릭 처리
    /// </summary>
    private void OnCloseButtonClicked()
    {
        LogDebug("닫기 버튼 클릭됨");
        
        // 부모 창 닫기
        if (closeParentWindow && parentWindow != null)
        {
            parentWindow.SetActive(false);
            LogDebug($"부모 창 비활성화: {parentWindow.name}");
        }
        
        // 타겟 창 닫기
        if (targetWindow != null)
        {
            targetWindow.SetActive(false);
            LogDebug($"창 비활성화: {targetWindow.name}");
        }
        else
        {
            Debug.LogWarning("[WindowCloseButton] targetWindow가 설정되지 않았습니다.");
        }
    }
    
    /// <summary>
    /// 타겟 창 설정
    /// </summary>
    public void SetTargetWindow(GameObject window)
    {
        targetWindow = window;
    }
    
    /// <summary>
    /// 부모 창 설정
    /// </summary>
    public void SetParentWindow(GameObject window)
    {
        parentWindow = window;
    }
    
    /// <summary>
    /// 디버그 로그 출력
    /// </summary>
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[WindowCloseButton] {message}");
        }
    }
}

