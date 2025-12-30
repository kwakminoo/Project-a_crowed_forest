using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 메인 씬의 설정창을 관리하는 매니저
/// 타이틀로 나가기, 종료하기 버튼에 자동 저장 기능 포함
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("설정 창 UI")]
    [Tooltip("설정 창 GameObject")]
    [SerializeField] private GameObject settingsWindow;
    
    [Header("설정 버튼들")]
    [Tooltip("타이틀로 나가기 버튼")]
    [SerializeField] private Button returnToTitleButton;
    
    [Tooltip("종료하기 버튼")]
    [SerializeField] private Button quitButton;
    
    [Tooltip("설정 창 닫기 버튼")]
    [SerializeField] private Button closeButton;
    
    [Header("사운드 설정")]
    [Tooltip("사운드 아이콘 버튼 (토글용)")]
    [SerializeField] private Button soundToggleButton;
    
    [Tooltip("사운드 아이콘 Image 컴포넌트")]
    [SerializeField] private Image soundIconImage;
    
    [Tooltip("사운드 ON 아이콘 스프라이트")]
    [SerializeField] private Sprite soundOnIcon;
    
    [Tooltip("사운드 OFF 아이콘 스프라이트")]
    [SerializeField] private Sprite soundOffIcon;
    
    [Tooltip("사운드 바 슬라이더")]
    [SerializeField] private Slider soundVolumeSlider;
    
    [Tooltip("사운드 바 배경 Image (검은색으로 변경용)")]
    [SerializeField] private Image soundBarBackground;
    
    [Tooltip("사운드 바 Fill Image")]
    [SerializeField] private Image soundBarFill;
    
    [Header("디버그")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // 사운드 상태
    private bool isSoundOn = true;
    private float savedVolume = 1.0f; // 사운드 off 전 볼륨 저장
    private Color soundBarNormalColor; // 사운드 바 원래 색상
    
    private void Start()
    {
        InitializeButtons();
        InitializeWindow();
        InitializeSoundSettings();
    }
    
    /// <summary>
    /// 버튼 이벤트 초기화
    /// </summary>
    private void InitializeButtons()
    {
        // 타이틀로 나가기 버튼
        if (returnToTitleButton != null)
        {
            returnToTitleButton.onClick.RemoveAllListeners();
            returnToTitleButton.onClick.AddListener(OnReturnToTitleClicked);
        }
        
        // 종료하기 버튼
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        // 설정 창 닫기 버튼
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        // 사운드 토글 버튼
        if (soundToggleButton != null)
        {
            soundToggleButton.onClick.RemoveAllListeners();
            soundToggleButton.onClick.AddListener(OnSoundToggleClicked);
        }
        
        // 사운드 볼륨 슬라이더
        if (soundVolumeSlider != null)
        {
            soundVolumeSlider.onValueChanged.RemoveAllListeners();
            soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            
            // 저장된 볼륨 불러오기
            float savedVol = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
            soundVolumeSlider.value = savedVol;
            ApplyVolume(savedVol);
        }
    }
    
    /// <summary>
    /// 설정 창 초기화
    /// </summary>
    private void InitializeWindow()
    {
        // 설정 창은 처음에 비활성화
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }
    }
    
    /// <summary>
    /// 사운드 설정 초기화
    /// </summary>
    private void InitializeSoundSettings()
    {
        // 사운드 바 원래 색상 저장
        if (soundBarFill != null)
        {
            soundBarNormalColor = soundBarFill.color;
        }
        
        // 저장된 사운드 상태 불러오기
        isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        UpdateSoundIcon();
        
        // 사운드가 꺼져있으면 볼륨 0으로 설정
        if (!isSoundOn)
        {
            ApplyVolume(0f);
            if (soundVolumeSlider != null)
            {
                soundVolumeSlider.value = 0f;
            }
        }
    }
    
    /// <summary>
    /// 설정 창 열기
    /// </summary>
    public void OpenSettings()
    {
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(true);
            LogDebug("설정 창이 열렸습니다.");
        }
    }
    
    /// <summary>
    /// 설정 창 닫기
    /// </summary>
    public void CloseSettings()
    {
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
            LogDebug("설정 창이 닫혔습니다.");
        }
    }
    
    /// <summary>
    /// 타이틀로 나가기 버튼 클릭 처리
    /// </summary>
    private void OnReturnToTitleClicked()
    {
        LogDebug("타이틀로 나가기 버튼 클릭됨");
        
        // 자동 저장 실행
        AutoSave();
        
        // 타이틀 씬으로 이동
        SceneManager.LoadScene("Title");
    }
    
    /// <summary>
    /// 종료하기 버튼 클릭 처리
    /// </summary>
    private void OnQuitClicked()
    {
        LogDebug("종료하기 버튼 클릭됨");
        
        // 자동 저장 실행
        AutoSave();
        
        // 게임 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    /// <summary>
    /// 설정 창 닫기 버튼 클릭 처리
    /// </summary>
    private void OnCloseClicked()
    {
        LogDebug("설정 창 닫기 버튼 클릭됨");
        CloseSettings();
    }
    
    /// <summary>
    /// 자동 저장 실행
    /// </summary>
    private void AutoSave()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogWarning("[SettingsManager] SaveSystem.Instance가 null입니다. 자동 저장을 건너뜁니다.");
            return;
        }
        
        // 현재 슬롯 번호 가져오기 (없으면 1번 슬롯 사용)
        int currentSlot = SaveSystem.Instance.GetCurrentSlotNumber();
        if (currentSlot == 0)
        {
            currentSlot = 1; // 기본값
        }
        
        bool success = SaveSystem.Instance.SaveGame(currentSlot);
        if (success)
        {
            LogDebug($"자동 저장 완료: 슬롯 {currentSlot}");
        }
        else
        {
            Debug.LogWarning("[SettingsManager] 자동 저장 실패");
        }
    }
    
    /// <summary>
    /// 사운드 토글 버튼 클릭 처리
    /// </summary>
    private void OnSoundToggleClicked()
    {
        isSoundOn = !isSoundOn;
        UpdateSoundIcon();
        ApplySoundState();
        
        LogDebug($"사운드 {(isSoundOn ? "ON" : "OFF")}");
    }
    
    /// <summary>
    /// 사운드 아이콘 업데이트
    /// </summary>
    private void UpdateSoundIcon()
    {
        if (soundIconImage == null)
        {
            return;
        }
        
        if (isSoundOn)
        {
            // 사운드 ON 아이콘으로 변경
            if (soundOnIcon != null)
            {
                soundIconImage.sprite = soundOnIcon;
            }
        }
        else
        {
            // 사운드 OFF 아이콘으로 변경
            if (soundOffIcon != null)
            {
                soundIconImage.sprite = soundOffIcon;
            }
        }
    }
    
    /// <summary>
    /// 사운드 상태 적용
    /// </summary>
    private void ApplySoundState()
    {
        if (isSoundOn)
        {
            // 사운드 켜기: 저장된 볼륨 복원
            if (soundVolumeSlider != null)
            {
                soundVolumeSlider.value = savedVolume;
                ApplyVolume(savedVolume);
            }
            
            // 사운드 바 색상 복원
            if (soundBarFill != null)
            {
                soundBarFill.color = soundBarNormalColor;
            }
            
            if (soundBarBackground != null)
            {
                soundBarBackground.color = Color.white; // 또는 원래 색상
            }
        }
        else
        {
            // 사운드 끄기: 현재 볼륨 저장 후 0으로 설정
            if (soundVolumeSlider != null)
            {
                savedVolume = soundVolumeSlider.value;
                soundVolumeSlider.value = 0f;
            }
            ApplyVolume(0f);
            
            // 사운드 바 검은색으로 변경
            if (soundBarFill != null)
            {
                soundBarFill.color = Color.black;
            }
            
            if (soundBarBackground != null)
            {
                soundBarBackground.color = Color.black;
            }
        }
        
        // 상태 저장
        PlayerPrefs.SetInt("SoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 사운드 볼륨 변경 처리
    /// </summary>
    private void OnSoundVolumeChanged(float volume)
    {
        ApplyVolume(volume);
        
        // 볼륨이 0보다 크면 사운드 ON으로 간주
        if (volume > 0f && !isSoundOn)
        {
            isSoundOn = true;
            UpdateSoundIcon();
            
            // 사운드 바 색상 복원
            if (soundBarFill != null)
            {
                soundBarFill.color = soundBarNormalColor;
            }
            
            if (soundBarBackground != null)
            {
                soundBarBackground.color = Color.white; // 또는 원래 색상
            }
        }
        else if (volume == 0f && isSoundOn)
        {
            isSoundOn = false;
            UpdateSoundIcon();
            
            // 사운드 바 검은색으로 변경
            if (soundBarFill != null)
            {
                soundBarFill.color = Color.black;
            }
            
            if (soundBarBackground != null)
            {
                soundBarBackground.color = Color.black;
            }
        }
        
        // 볼륨 저장
        PlayerPrefs.SetFloat("SoundVolume", volume);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 볼륨을 모든 AudioSource에 적용
    /// </summary>
    private void ApplyVolume(float volume)
    {
        // 모든 AudioSource 찾기
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }
        
        // 전역 볼륨 설정 (선택사항)
        AudioListener.volume = volume;
        
        LogDebug($"볼륨 설정: {volume * 100:F0}%");
    }
    
    /// <summary>
    /// 디버그 로그 출력
    /// </summary>
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[SettingsManager] {message}");
        }
    }
}

