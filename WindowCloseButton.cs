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
    
    [Header("닫기 방식 설정")]
    [Tooltip("타겟 창을 닫을 때 삭제할지 비활성화할지 (true: 삭제, false: 비활성화)")]
    [SerializeField] private bool destroyTargetWindow = true;
    
    [Tooltip("부모 창을 닫을 때 삭제할지 비활성화할지 (true: 삭제, false: 비활성화)")]
    [SerializeField] private bool destroyParentWindow = false;
    
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
            CloseWindow(parentWindow, destroyParentWindow, "부모 창");
        }
        
        // 타겟 창 닫기
        if (targetWindow != null)
        {
            CloseWindow(targetWindow, destroyTargetWindow, "타겟 창");
        }
        else
        {
            Debug.LogWarning("[WindowCloseButton] targetWindow가 설정되지 않았습니다.");
        }
    }
    
    /// <summary>
    /// 창을 닫는 공통 메서드 (삭제 또는 비활성화)
    /// </summary>
    /// <param name="window">닫을 창 GameObject</param>
    /// <param name="destroy">true면 삭제, false면 비활성화</param>
    /// <param name="windowType">창 타입 (로그용)</param>
    private void CloseWindow(GameObject window, bool destroy, string windowType)
    {
        if (window == null)
        {
            return;
        }
        
        if (destroy)
        {
            Destroy(window);
            LogDebug($"{windowType} 삭제: {window.name}");
        }
        else
        {
            window.SetActive(false);
            LogDebug($"{windowType} 비활성화: {window.name}");
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



