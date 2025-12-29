using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 캐릭터 선택 UI를 관리하는 컴포넌트
/// </summary>
public class CharacterSelector : MonoBehaviour
{
    [Header("캐릭터 데이터")]
    [Tooltip("선택 가능한 캐릭터 목록 (순서대로 표시됨)")]
    [SerializeField] private List<CharacterData> characterList = new List<CharacterData>();
    
    [Header("UI 참조")]
    [Tooltip("캐릭터 초상화를 표시할 Image 컴포넌트")]
    [SerializeField] private Image characterPortraitImage;
    
    [Tooltip("캐릭터 이름을 표시할 TextMeshProUGUI")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    
    [Tooltip("캐릭터 설명을 표시할 TextMeshProUGUI (OPTION 영역)")]
    [SerializeField] private TextMeshProUGUI characterDescriptionText;
    
    [Header("네비게이션 버튼")]
    [Tooltip("왼쪽 화살표 버튼 (이전 캐릭터)")]
    [SerializeField] private Button leftButton;
    
    [Tooltip("오른쪽 화살표 버튼 (다음 캐릭터)")]
    [SerializeField] private Button rightButton;
    
    [Tooltip("캐릭터 초상화 버튼 (클릭 시 캐릭터 선택 확인)")]
    [SerializeField] private Button characterSelectButton;
    
    [Header("게임 시작")]
    [Tooltip("게임 시작 씬 이름")]
    [SerializeField] private string gameStartSceneName = "Main Scenes";
    
    [Header("디버그")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // 현재 선택된 캐릭터 인덱스
    private int currentCharacterIndex = 0;
    
    // 이 캐릭터 선택 창이 연결된 슬롯 번호
    private int associatedSlotNumber = 0;
    
    // 현재 선택된 캐릭터 데이터
    public CharacterData CurrentCharacter 
    { 
        get 
        { 
            if (characterList == null || characterList.Count == 0) 
                return null;
            return characterList[currentCharacterIndex]; 
        } 
    }
    
    private void Awake()
    {
        ValidateReferences();
    }
    
    private void Start()
    {
        InitializeButtons();
        UpdateCharacterDisplay();
    }
    
    /// <summary>
    /// 참조가 올바르게 설정되었는지 확인
    /// </summary>
    private void ValidateReferences()
    {
        if (characterList == null || characterList.Count == 0)
        {
            Debug.LogWarning("[CharacterSelector] characterList가 비어있습니다. 인스펙터에서 캐릭터 데이터를 추가해주세요.");
        }
        
        if (characterPortraitImage == null)
            Debug.LogError("[CharacterSelector] characterPortraitImage가 설정되지 않았습니다.");
        
        if (characterNameText == null)
            Debug.LogError("[CharacterSelector] characterNameText가 설정되지 않았습니다.");
        
        if (characterDescriptionText == null)
            Debug.LogError("[CharacterSelector] characterDescriptionText가 설정되지 않았습니다.");
        
        if (leftButton == null)
            Debug.LogError("[CharacterSelector] leftButton이 설정되지 않았습니다.");
        
        if (rightButton == null)
            Debug.LogError("[CharacterSelector] rightButton이 설정되지 않았습니다.");
    }
    
    /// <summary>
    /// 버튼 이벤트 초기화
    /// </summary>
    private void InitializeButtons()
    {
        if (leftButton != null)
        {
            leftButton.onClick.RemoveAllListeners();
            leftButton.onClick.AddListener(SelectPreviousCharacter);
        }
        
        if (rightButton != null)
        {
            rightButton.onClick.RemoveAllListeners();
            rightButton.onClick.AddListener(SelectNextCharacter);
        }
        
        // 캐릭터 초상화 버튼 (CHARACTER 이미지에 Button 컴포넌트가 있는 경우)
        if (characterSelectButton != null)
        {
            characterSelectButton.onClick.RemoveAllListeners();
            characterSelectButton.onClick.AddListener(OnCharacterSelected);
        }
    }
    
    /// <summary>
    /// 다음 캐릭터 선택 (순환)
    /// </summary>
    public void SelectNextCharacter()
    {
        if (characterList == null || characterList.Count == 0)
        {
            LogDebug("캐릭터 목록이 비어있어 전환할 수 없습니다.");
            return;
        }
        
        currentCharacterIndex = (currentCharacterIndex + 1) % characterList.Count;
        UpdateCharacterDisplay();
        LogDebug($"다음 캐릭터 선택: {CurrentCharacter?.characterName} (인덱스: {currentCharacterIndex})");
    }
    
    /// <summary>
    /// 이전 캐릭터 선택 (순환)
    /// </summary>
    public void SelectPreviousCharacter()
    {
        if (characterList == null || characterList.Count == 0)
        {
            LogDebug("캐릭터 목록이 비어있어 전환할 수 없습니다.");
            return;
        }
        
        // 음수 방지하고 순환 처리
        currentCharacterIndex = (currentCharacterIndex - 1 + characterList.Count) % characterList.Count;
        UpdateCharacterDisplay();
        LogDebug($"이전 캐릭터 선택: {CurrentCharacter?.characterName} (인덱스: {currentCharacterIndex})");
    }
    
    /// <summary>
    /// 특정 인덱스의 캐릭터 선택
    /// </summary>
    public void SelectCharacter(int index)
    {
        if (characterList == null || characterList.Count == 0)
        {
            LogDebug("캐릭터 목록이 비어있습니다.");
            return;
        }
        
        if (index < 0 || index >= characterList.Count)
        {
            Debug.LogWarning($"[CharacterSelector] 잘못된 인덱스: {index}. 범위: 0 ~ {characterList.Count - 1}");
            return;
        }
        
        currentCharacterIndex = index;
        UpdateCharacterDisplay();
        LogDebug($"캐릭터 선택: {CurrentCharacter?.characterName} (인덱스: {currentCharacterIndex})");
    }
    
    /// <summary>
    /// UI 업데이트
    /// </summary>
    private void UpdateCharacterDisplay()
    {
        if (CurrentCharacter == null)
        {
            LogDebug("현재 캐릭터 데이터가 null입니다.");
            return;
        }
        
        LogDebug($"캐릭터 표시 업데이트: {CurrentCharacter.characterName} (인덱스: {currentCharacterIndex})");
        
        // 초상화 업데이트
        if (characterPortraitImage != null)
        {
            Sprite portrait = CurrentCharacter.GetPortrait();
            if (portrait != null)
            {
                characterPortraitImage.sprite = portrait;
                LogDebug($"초상화 업데이트 완료: {CurrentCharacter.characterName}");
            }
            else
            {
                Debug.LogWarning($"[CharacterSelector] {CurrentCharacter.characterName}의 초상화를 로드할 수 없습니다. (경로: UI/{CurrentCharacter.portraitSpriteName})");
            }
        }
        else
        {
            Debug.LogError("[CharacterSelector] characterPortraitImage가 null입니다.");
        }
        
        // 이름 업데이트
        if (characterNameText != null)
        {
            characterNameText.text = CurrentCharacter.characterName;
            LogDebug($"이름 업데이트: {CurrentCharacter.characterName}");
        }
        
        // 설명 업데이트
        if (characterDescriptionText != null)
        {
            characterDescriptionText.text = CurrentCharacter.description;
            LogDebug($"설명 업데이트 완료");
        }
    }
    
    /// <summary>
    /// 캐릭터 선택 창 열기
    /// </summary>
    public void OpenCharacterSelect()
    {
        gameObject.SetActive(true);
        currentCharacterIndex = 0; // 첫 번째 캐릭터로 초기화
        UpdateCharacterDisplay();
        LogDebug("캐릭터 선택 창이 열렸습니다.");
    }
    
    /// <summary>
    /// 캐릭터 선택 창 닫기
    /// </summary>
    public void CloseCharacterSelect()
    {
        gameObject.SetActive(false);
        LogDebug("캐릭터 선택 창이 닫혔습니다.");
    }
    
    /// <summary>
    /// 캐릭터 선택 확인 (CHARACTER 이미지 클릭 시 호출)
    /// </summary>
    private void OnCharacterSelected()
    {
        if (CurrentCharacter == null)
        {
            LogDebug("선택할 캐릭터가 없습니다.");
            return;
        }
        
        LogDebug($"캐릭터 선택 확인: {CurrentCharacter.characterName} (슬롯: {associatedSlotNumber})");
        StartNewGame();
    }
    
    /// <summary>
    /// 새 게임 시작
    /// </summary>
    public void StartNewGame()
    {
        if (CurrentCharacter == null)
        {
            Debug.LogError("[CharacterSelector] 캐릭터가 선택되지 않았습니다.");
            return;
        }
        
        LogDebug($"새 게임 시작: {CurrentCharacter.characterName} (슬롯: {associatedSlotNumber})");
        
        // 선택된 캐릭터의 시나리오 정보를 저장 (메인 씬에서 사용)
        SaveCharacterScenarioData();
        
        // 선택된 캐릭터 데이터를 저장 시스템에 전달 (나중에 SaveLoadManager와 연동)
        // SaveLoadManager.Instance.CreateNewGame(associatedSlotNumber, CurrentCharacter);
        
        // TODO: 씬 전환 (나중에 SceneTransitionManager와 연동)
        // SceneTransitionManager.Instance.LoadSceneWithFade(gameStartSceneName);
        
        // 씬 전환
        SceneManager.LoadScene(gameStartSceneName);
    }
    
    /// <summary>
    /// 선택된 캐릭터의 시나리오 정보를 저장 (메인 씬에서 사용)
    /// </summary>
    private void SaveCharacterScenarioData()
    {
        if (CurrentCharacter == null)
        {
            return;
        }
        
        // PlayerPrefs에 캐릭터 정보 저장 (메인 씬에서 읽어서 사용)
        PlayerPrefs.SetString("SelectedCharacterName", CurrentCharacter.characterName);
        PlayerPrefs.SetInt("SelectedSlotNumber", associatedSlotNumber);
        
        // Yarn 시나리오 정보 저장
        if (!string.IsNullOrEmpty(CurrentCharacter.yarnScriptName))
        {
            PlayerPrefs.SetString("YarnScriptName", CurrentCharacter.yarnScriptName);
        }
        
        if (!string.IsNullOrEmpty(CurrentCharacter.startNodeName))
        {
            PlayerPrefs.SetString("YarnStartNode", CurrentCharacter.startNodeName);
        }
        
        PlayerPrefs.Save();
        
        // 상세 디버그 로그
        Debug.Log($"[CharacterSelector] ===== 캐릭터 시나리오 정보 저장 =====");
        Debug.Log($"[CharacterSelector] 캐릭터 이름: {CurrentCharacter.characterName}");
        Debug.Log($"[CharacterSelector] 현재 인덱스: {currentCharacterIndex}");
        Debug.Log($"[CharacterSelector] 슬롯 번호: {associatedSlotNumber}");
        Debug.Log($"[CharacterSelector] Yarn 스크립트: {CurrentCharacter.yarnScriptName}");
        Debug.Log($"[CharacterSelector] 시작 노드: {CurrentCharacter.startNodeName}");
        Debug.Log($"[CharacterSelector] PlayerPrefs 저장 완료");
        
        // 저장된 값 확인
        string savedName = PlayerPrefs.GetString("SelectedCharacterName", "");
        string savedScript = PlayerPrefs.GetString("YarnScriptName", "");
        string savedNode = PlayerPrefs.GetString("YarnStartNode", "");
        Debug.Log($"[CharacterSelector] 저장 확인 - 이름: {savedName}, 스크립트: {savedScript}, 노드: {savedNode}");
    }
    
    /// <summary>
    /// 슬롯 번호 설정 (LoadFileSlot에서 호출)
    /// </summary>
    public void SetSlotNumber(int slotNumber)
    {
        associatedSlotNumber = slotNumber;
        LogDebug($"슬롯 번호 설정: {slotNumber}");
    }
    
    /// <summary>
    /// 디버그 로그 출력
    /// </summary>
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[CharacterSelector] {message}");
        }
    }
}

