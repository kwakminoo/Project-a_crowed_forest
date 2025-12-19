using UnityEngine;

[CreateAssetMenu(fileName = "BattleConfig", menuName = "Battle/Battle Config")]
public class BattleConfig : ScriptableObject
{
    [Header("Timing Settings")]
    [Tooltip("턴 간 대기 시간")]
    public float turnDelay = 1f;
    
    [Tooltip("스킬 실행 대기 시간")]
    public float skillExecutionDelay = 2f;
    
    [Tooltip("카메라 전환 지속 시간")]
    public float cameraTransitionDuration = 1f;
    
    [Tooltip("스킬 이펙트 지속 시간")]
    public float effectDuration = 1f;
    
    [Tooltip("플레이어 턴 대기 시간")]
    public float playerTurnDelay = 1f;

    [Header("UI Settings")]
    [Tooltip("HP바 업데이트 속도")]
    public float hpBarUpdateSpeed = 0.5f;
    
    [Tooltip("플레이어 HP바 색상")]
    public Color playerHPColor = Color.green;
    
    [Tooltip("적 HP바 색상")]
    public Color enemyHPColor = Color.red;
    
    [Tooltip("HP바 애니메이션 커브")]
    public AnimationCurve hpBarAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Camera Settings")]
    [Tooltip("카메라 전환 애니메이션 커브")]
    public AnimationCurve cameraTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Tooltip("배틀 카메라 오프셋")]
    public Vector3 battleCameraOffset = Vector3.zero;
    
    [Tooltip("카메라 전환 시 부드러운 이동 사용")]
    public bool useSmoothCameraTransition = true;

    [Header("Debug Settings")]
    [Tooltip("디버그 로그 출력")]
    public bool enableDebugLogs = true;
    
    [Tooltip("전투 상태 변경 로그")]
    public bool logBattleStateChanges = true;
    
    [Tooltip("스킬 실행 로그")]
    public bool logSkillExecution = true;
}

