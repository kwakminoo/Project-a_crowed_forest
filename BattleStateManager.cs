using UnityEngine;
using System;

public enum BattleState
{
    Idle,           // 대기 상태
    Initializing,   // 전투 초기화 중
    PlayerTurn,     // 플레이어 턴
    EnemyTurn,      // 적 턴
    SkillExecuting, // 스킬 실행 중
    BattleEnding,   // 전투 종료 중
    BattleEnded     // 전투 완료
}

public class BattleStateManager : MonoBehaviour
{
    [Header("State Settings")]
    [SerializeField] private BattleState currentState = BattleState.Idle;
    [SerializeField] private BattleConfig config;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // 이벤트
    public event Action<BattleState, BattleState> OnStateChanged;
    public event Action<BattleState> OnStateEntered;
    public event Action<BattleState> OnStateExited;
    
    // 프로퍼티
    public BattleState CurrentState => currentState;
    public BattleConfig Config => config;
    
    private void Awake()
    {
        // 설정이 없으면 기본값 사용
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<BattleConfig>();
            LogDebug("BattleConfig가 설정되지 않아 기본값을 사용합니다.");
        }
    }
    
    /// <summary>
    /// 전투 상태를 변경합니다.
    /// </summary>
    /// <param name="newState">새로운 상태</param>
    /// <param name="force">강제 변경 여부</param>
    public void ChangeState(BattleState newState, bool force = false)
    {
        if (currentState == newState && !force)
        {
            LogDebug($"상태 변경 요청 무시: {newState} (이미 현재 상태)");
            return;
        }
        
        BattleState previousState = currentState;
        
        // 이전 상태 종료
        ExitState(previousState);
        
        // 상태 변경
        currentState = newState;
        
        // 새 상태 진입
        EnterState(newState);
        
        // 이벤트 발행
        OnStateChanged?.Invoke(previousState, newState);
        
        LogDebug($"전투 상태 변경: {previousState} → {newState}");
    }
    
    /// <summary>
    /// 상태 진입 처리
    /// </summary>
    private void EnterState(BattleState state)
    {
        OnStateEntered?.Invoke(state);
        
        switch (state)
        {
            case BattleState.Initializing:
                LogDebug("전투 초기화 시작");
                break;
                
            case BattleState.PlayerTurn:
                LogDebug("플레이어 턴 시작");
                break;
                
            case BattleState.EnemyTurn:
                LogDebug("적 턴 시작");
                break;
                
            case BattleState.SkillExecuting:
                LogDebug("스킬 실행 중");
                break;
                
            case BattleState.BattleEnding:
                LogDebug("전투 종료 중");
                break;
                
            case BattleState.BattleEnded:
                LogDebug("전투 완료");
                break;
        }
    }
    
    /// <summary>
    /// 상태 종료 처리
    /// </summary>
    private void ExitState(BattleState state)
    {
        OnStateExited?.Invoke(state);
        
        switch (state)
        {
            case BattleState.PlayerTurn:
                LogDebug("플레이어 턴 종료");
                break;
                
            case BattleState.EnemyTurn:
                LogDebug("적 턴 종료");
                break;
                
            case BattleState.SkillExecuting:
                LogDebug("스킬 실행 완료");
                break;
        }
    }
    
    /// <summary>
    /// 특정 상태인지 확인
    /// </summary>
    public bool IsInState(BattleState state)
    {
        return currentState == state;
    }
    
    /// <summary>
    /// 전투가 활성 상태인지 확인
    /// </summary>
    public bool IsBattleActive()
    {
        return currentState != BattleState.Idle && 
               currentState != BattleState.BattleEnded;
    }
    
    /// <summary>
    /// 전투가 종료되었는지 확인
    /// </summary>
    public bool IsBattleEnded()
    {
        return currentState == BattleState.BattleEnded;
    }
    
    /// <summary>
    /// 전투를 초기화합니다.
    /// </summary>
    public void InitializeBattle()
    {
        ChangeState(BattleState.Initializing);
    }
    
    /// <summary>
    /// 전투를 종료합니다.
    /// </summary>
    public void EndBattle()
    {
        ChangeState(BattleState.BattleEnding);
        
        // 잠시 후 완전 종료
        Invoke(nameof(CompleteBattleEnd), 0.5f);
    }
    
    private void CompleteBattleEnd()
    {
        ChangeState(BattleState.BattleEnded);
    }
    
    /// <summary>
    /// 디버그 로그 출력
    /// </summary>
    private void LogDebug(string message)
    {
        if (enableDebugLogs && config != null && config.enableDebugLogs)
        {
            Debug.Log($"[BattleStateManager] {message}");
        }
    }
    
    private void OnDestroy()
    {
        // 이벤트 정리
        OnStateChanged = null;
        OnStateEntered = null;
        OnStateExited = null;
    }
}
