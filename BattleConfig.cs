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

    [Header("Cinemachine Camera Settings")]
    [Tooltip("카메라 블렌드 시간 (초)")]
    public float cameraBlendTime = 0.5f;
    
    [Tooltip("스킬 사용 시 카메라 블렌드 시간 (초)")]
    public float skillCameraBlendTime = 0.3f;
    
    [Tooltip("스킬 카메라 줌 레벨 (Orthographic Size 감소량)")]
    public float skillCameraZoom = 2f;
    
    [Tooltip("스킬 카메라 포커스 지속 시간 (초)")]
    public float skillCameraFocusDuration = 1.5f;
    
    [Tooltip("플레이어 Virtual Camera 우선순위")]
    public int playerVCamPriority = 10;
    
    [Tooltip("적 Virtual Camera 우선순위")]
    public int enemyVCamPriority = 11;
    
    [Tooltip("스킬 포커스 Virtual Camera 우선순위")]
    public int skillFocusVCamPriority = 20;
    
    [Tooltip("기본 Virtual Camera 우선순위")]
    public int defaultVCamPriority = 5;

    [Header("Debug Settings")]
    [Tooltip("디버그 로그 출력")]
    public bool enableDebugLogs = true;
    
    [Tooltip("전투 상태 변경 로그")]
    public bool logBattleStateChanges = true;
    
    [Tooltip("스킬 실행 로그")]
    public bool logSkillExecution = true;
}

