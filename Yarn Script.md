Custom Line View
-
~~~C#
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using Yarn;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class CustomLineView : DialogueViewBase
{
    public Transform contentParent; // Scroll View의 Content 객체
    public GameObject textPrefab;   // 텍스트 프리팹
    public GameObject imagePrefab;  // 이미지 프리팹
    public GameObject buttonPrefab; // 선택지 버튼 프리팹
    public ScrollRect scrollRect;   // 스크롤 가능한 영역
    private System.Action onDialogueLineFinishedCallBack;

    public float typingSpeed = 0.075f; //텍스트 출력 속도
    private bool isTyping = false; //텍스트 출력 중 여부
    private string fullText; //출력할 전체 텍스트
    private string currentNodeName  = ""; // 현재 타이틀 이름 저장
    private DialogueRunner dialogueRunner; // DialogueRunner 참조

    public AudioSource audioSource;  // ✅ 효과음 재생기 추가
    public List<AudioClip> soundEffects;  // ✅ 효과음 목록
    private Queue<(string sound, string triggerWord)> queuedSoundCommands = new Queue<(string, string)>(); // 🔹 효과음 + 트리거 단어 저장

    public AudioSource bgmSource;  // ✅ BGM을 재생할 AudioSource
    public string defaultBGM = "main_theme";  // ✅ 기본 BGM 이름 (게임이 시작될 때 실행될 BGM)
    private string previousBGM;  // ✅ 전투 전 BGM 저장

    void Start()
    {
        var dialogueRunner = FindObjectOfType<DialogueRunner>();
        if(dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("show_image", ShowImage);
            dialogueRunner.AddCommandHandler<string, string, string, string>("start_Battle", StartBattleCommand);
            dialogueRunner.AddCommandHandler<string, string>("play_sfx", PlaySFX);  // ✅ 효과음 명령 추가
            dialogueRunner.AddCommandHandler<string>("change_bgm", ChangeBGM);  // ✅ BGM 변경 명령 추가
            dialogueRunner.AddCommandHandler<string>("play_bgm", PlayBGM);  // ✅ BGM 실행 명령 추가
            dialogueRunner.AddCommandHandler("stop_bgm", StopBGM);
            dialogueRunner.AddCommandHandler<string, string>("give_item", GiveItemCommand);
        }
        else
        {
            Debug.LogError("다이얼로그 러너를 찾을 수 없습니다");
        }

        dialogueRunner.onNodeStart.AddListener(OnNodeStart); // 노드 변경 이벤트 연결
        // ✅ 게임 시작 시 기본 BGM 실행
        PlayBGM(defaultBGM);
    }

    public override void RunLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        onDialogueLineFinishedCallBack = onDialogueLineFinished;

        // 클릭 시 텍스트 즉시 출력 처리
        scrollRect.GetComponent<Button>().onClick.RemoveAllListeners();
        scrollRect.GetComponent<Button>().onClick.AddListener(() => CompleteTyping());

        string processedText = line.TextWithoutCharacterName.Text.Trim();

        Debug.Log($"📢 RunLine 실행됨: {processedText}");  // ✅ RunLine 실행 확인

        // 🔹 Yarn 명령어 감지 (예: `<<play_sfx "crow_caw" "까마귀">>`)
        System.Text.RegularExpressions.Regex commandRegex = new System.Text.RegularExpressions.Regex(@"<<play_sfx\s+""(.+?)""\s+""(.+?)"">>");
        var match = commandRegex.Match(processedText);

        while (match.Success)
        {
            string soundName = match.Groups[1].Value;  // 효과음 파일명
            string triggerWord = match.Groups[2].Value; // 트리거 단어

            Debug.Log($"✅ 효과음 명령 추가됨: {soundName}, 트리거 단어: {triggerWord}");
            queuedSoundCommands.Enqueue((soundName, triggerWord)); // ✅ 큐에 저장

            // 🔹 Yarn 명령어 제거 후 텍스트 업데이트
            processedText = processedText.Replace(match.Value, "").Trim();
            match = commandRegex.Match(processedText);
        }

        // 💡 텍스트를 정상적으로 출력하도록 RunLine을 실행
        StartCoroutine(TypeLine(line, onDialogueLineFinished));
    }

    public void PlayBGM(string bgmName)
    {
        if (string.IsNullOrEmpty(bgmName))
        {
            Debug.LogWarning("⚠ BGM 이름이 비어 있습니다.");
            return;
        }

        // ✅ Resources 폴더의 Audio/BGM/ 경로에서 오디오 파일 불러오기
        AudioClip bgmClip = Resources.Load<AudioClip>($"Audio/BGM/{bgmName}");
        
        if (bgmClip == null)
        {
            Debug.LogError($"⚠ BGM '{bgmName}'을(를) 찾을 수 없습니다. Resources/Audio/BGM/ 폴더를 확인하세요.");
            return;
        }

        // ✅ 같은 BGM이 이미 재생 중이면 변경하지 않음
        if (bgmSource.clip == bgmClip && bgmSource.isPlaying)
        {
            return;
        }

        // ✅ 새 BGM으로 변경 후 재생
        bgmSource.clip = bgmClip;
        bgmSource.loop = true; // ✅ BGM이 반복 재생되도록 설정
        bgmSource.Play();

        Debug.Log($"🎵 BGM 변경: {bgmName}");
    }
    
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            Debug.Log("🎵 BGM 정지됨");
        }
    }

    public void ChangeBGM(string bgmName)
    {
        PlayBGM(bgmName);
    }

    // ✅ 전투 종료 후 원래 BGM 복원
    public void RestorePreviousBGM()
    {
        PlayBGM(previousBGM);
    }

    private void OnNodeStart(string nodeName)
    {
        if (nodeName != currentNodeName)
        {
            currentNodeName = nodeName; // 새로운 노드 이름 저장
            ClearContent(); // 노드가 변경될 때 콘텐츠 초기화
        }
    }  

    private void ClearContent()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject); // 모든 자식 오브젝트 삭제
        }
        Debug.Log("콘텐츠 초기화 완료");
    }
    
    // 1. 스토리 텍스트 출력
    private IEnumerator TypeLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        isTyping = true; // 타이핑 상태 플래그 설정
        fullText = line.TextWithoutCharacterName.Text;

        // 새로운 텍스트 객체 생성
        GameObject newTextObject = Instantiate(textPrefab, contentParent);
        TextMeshProUGUI storyText = newTextObject.GetComponent<TextMeshProUGUI>();
        storyText.text = "";

        string currentText = ""; // 현재 출력 중인 텍스트

        // 텍스트 한 글자씩 출력
        for (int i = 0; i < fullText.Length; i++)
        {
            if (!isTyping) break;

            // <br> 태그 감지 및 줄바꿈 처리
            if (fullText[i] == '<' && i + 3 < fullText.Length && fullText.Substring(i, 4) == "<br>")
            {
                currentText += "\n"; // 줄바꿈 추가
                storyText.text = currentText; // 텍스트 업데이트
                i += 3; // "<br>" 건너뛰기 (4글자 점프)
                continue;
            }

            // 한 글자씩 추가
            currentText += fullText[i];
            storyText.text = currentText;

            // 🔹 특정 단어 등장 시 효과음 실행
            foreach (var (soundName, triggerWord) in queuedSoundCommands.ToList())
            {
                if (currentText.Contains(triggerWord)) // ✅ 트리거 단어가 현재 출력된 텍스트에 포함되었을 때 실행
                {
                    Debug.Log($"🎯 트리거 단어 '{triggerWord}' 감지 → '{soundName}' 효과음 재생");
                    PlaySFX(soundName, triggerWord);

                    // ✅ 사용한 효과음 명령을 큐에서 제거
                    queuedSoundCommands = new Queue<(string, string)>(queuedSoundCommands.Where(cmd => cmd.triggerWord != triggerWord));
                }
            }           

            // 스크롤을 맨 아래로 이동
            ScrollToBottom();

            yield return new WaitForSeconds(typingSpeed);
        }

        // 텍스트 출력 완료
        isTyping = false; // 타이핑 상태 플래그 해제
        storyText.text = fullText; // 최종 텍스트 설정

        // 사용자 입력 대기
        yield return StartCoroutine(WaitForUserInput());

        // 콜백 호출
        onDialogueLineFinishedCallBack?.Invoke();

        // 선택지를 다시 활성화
        foreach (Transform child in contentParent)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                child.gameObject.SetActive(true); // 버튼 다시 활성화
            }
        }

        // 스크롤을 맨 아래로 이동
        ScrollToBottom();
    }

    private IEnumerator WaitForUserInput()
    {
        bool inputReceived = false;

        // 스크롤 영역 클릭 리스너 설정
        scrollRect.GetComponent<Button>().onClick.RemoveAllListeners();
        scrollRect.GetComponent<Button>().onClick.AddListener(() => inputReceived = true);

        // 사용자가 클릭할 때까지 대기
        yield return new WaitUntil(() => inputReceived);
    }

    private void CompleteTyping()
    {
        // 텍스트 객체 가져오기
        TextMeshProUGUI storyText = null;
        if (contentParent.childCount > 0)
        {
            Transform lastChild = contentParent.GetChild(contentParent.childCount - 1);
            storyText = lastChild.GetComponent<TextMeshProUGUI>();
        }

        // 텍스트 객체가 없으면 새로 생성
        if (storyText == null)
        {
            GameObject newTextObject = Instantiate(textPrefab, contentParent);
            storyText = newTextObject.GetComponent<TextMeshProUGUI>();
            storyText.text = ""; // 초기화
        }

        if (isTyping)
        {
            // 텍스트 출력 중인 경우 즉시 완료
            isTyping = false;
            storyText.text = fullText;
        }
        else
        {
            // 텍스트 출력이 완료되었을 경우 다음 동작 수행
            onDialogueLineFinishedCallBack?.Invoke();
        }
        ScrollToBottom();
    }

    // 2. 이미지 출력 명령어 처리
    public void ShowImage(string imageName)
    {
        // Resources 폴더에서 이미지 로드
        Sprite image = Resources.Load<Sprite>($"Images/{imageName}");

        if(image == null)
        {
            Debug.LogError("{imageName}을 찾을 수 없습니다");
            return;
        }

        GameObject imageObject = Instantiate(imagePrefab, contentParent);
        Image imageComponent = imageObject.GetComponent<Image>();
        if(imageComponent == null)
        {
            Debug.LogError("이미지 컴포넌트를 찾을 수 없습니다");
            return;
        }
        imageComponent.sprite = image;
        imagePrefab.SetActive(true);
        imageObject.transform.SetAsFirstSibling();
    }

    public void StartBattleCommand(string enemyDataName, string backGroundName, string battleBGM, string nextYarnNode)
    {
        EnemyData enemyData = Resources.Load<EnemyData>($"Character/{enemyDataName}");
        Debug.Log($"로드 시도: Resources/Character/{enemyDataName}");
        if(enemyData != null)
        {
            Debug.Log($"로그 성공: {enemyData.enemyName}");
        }
        if(enemyData == null)
        {
            Debug.LogError($"Resources/Character/{enemyDataName}.asset의 데이터를 찾을 수 없습니다");
            return;
        }

        var BattleManager = FindObjectOfType<BattleManager>();
        if(BattleManager != null)
        {
            previousBGM = bgmSource.clip?.name;  // ✅ 기존 BGM 저장

            if (!string.IsNullOrEmpty(battleBGM)) // 🔹 사용자가 BGM을 입력한 경우
            {
                PlayBGM(battleBGM);  // ✅ 입력한 BGM 사용
            }
            else
            {
                PlayBGM("battle_theme");  // ✅ 기본 전투 BGM 사용
            }
            //적 데이터 전달
            BattleManager.StartBattle(enemyData, backGroundName, nextYarnNode);
        }
        else
        {
            Debug.LogError("BattleManager을찾을 수 없습니다");
        }
    }

    // 3. 선택지 버튼 생성 및 출력
    public override void RunOptions(DialogueOption[] options, System.Action<int> onOptionSelected)
    {

        Debug.Log($"선택지 출력 시작, 총 {options.Length}개의 선택지");

        // ClearContent 함수로 버튼만 삭제 (이미지나 텍스트는 삭제하지 않음)
        foreach (Transform child in contentParent)
        {
            if (child.GetComponent<Button>() != null)
            {
                Destroy(child.gameObject);  // 기존 버튼 삭제
            }
        }

        //새로운 선택지 버튼 생성
        for (int i = 0; i < options.Length; i++)
        {
            int optionIndex = i;
            GameObject buttonObject = Instantiate(buttonPrefab, contentParent); // 선택지 버튼 생성
            buttonObject.SetActive(true); // 비활성화된 경우 활성화

            RectTransform rectTransfrom = buttonObject.GetComponent<RectTransform>();
            rectTransfrom.anchoredPosition3D = Vector3.zero;
            rectTransfrom.localScale = Vector3.one;

            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();

            TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = options[i].Line.TextWithoutCharacterName.Text;
            }
            else
            {
                Debug.LogError("버튼에 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }

            Button button = buttonObject.GetComponent<Button>();
            button.interactable = true; // 버튼 활성화
            button.onClick.AddListener(() => 
            {
                Debug.Log($"버튼 {optionIndex} 클릭됨");
                button.interactable = false;  // 클릭 후 다시 선택되지 않도록 비활성화

                // 선택지 버튼 비활성화
                DisableAllButtons();

                // 선택지와 연결된 스토리 텍스트 추가
                string connectedStoryText = GetConnectedStoryText(options[optionIndex]);
                if (!string.IsNullOrEmpty(connectedStoryText))
                {
                    AddNewTextObject(connectedStoryText); // 새 텍스트 객체 생성
                }

                onOptionSelected(optionIndex); //선택지 처리    
            });

            // 버튼을 텍스트 출력 아래로 이동
            buttonObject.transform.SetAsLastSibling();
        }
        ScrollToBottom();
    }

    private void DisableAllButtons()
    {
        foreach (Transform child in contentParent)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false; // 버튼 비활성화
                Destroy(button.gameObject); // 버튼 UI를 아예 삭제 (필요 시 주석 처리 가능)
            }
        }
    }

    private string GetConnectedStoryText(DialogueOption option)
    {
        // Line.TextWithoutCharacterName가 유효한지 확인하고 텍스트를 반환합니다.
        if (option.Line?.TextWithoutCharacterName != null && 
            !string.IsNullOrEmpty(option.Line.TextWithoutCharacterName.Text))
        {
            return option.Line.TextWithoutCharacterName.Text;
        }
        return string.Empty;
    }

    private void AddNewTextObject(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("출력할 텍스트가 비어 있습니다.");
            return;
        }

        GameObject newTextObject = Instantiate(textPrefab, contentParent);
        TextMeshProUGUI newText = newTextObject.GetComponent<TextMeshProUGUI>();

        if (newText == null)
        {
            Debug.LogError("텍스트 프리팹에 TextMeshProUGUI 컴포넌트가 없습니다.");
            return;
        }

        newText.text = text;

        newTextObject.transform.SetAsLastSibling();
        DisableAllButtons();

        ScrollToBottom();
    }

    // 스크롤을 하단으로 이동시키는 함수
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();  // 강제로 UI 업데이트
        scrollRect.verticalNormalizedPosition = 0f;  // 스크롤을 맨 아래로 이동
    }

    public void PlaySFX(string soundName, string triggerWord)
    {
        Debug.Log($"🔍 PlaySFX 호출됨: {soundName}, 트리거 단어: {triggerWord}");  // ✅ 함수 호출 확인

        // Resources 폴더에서 효과음 로드
        AudioClip clip = Resources.Load<AudioClip>($"Audio/Sound Effects/{soundName}");

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            Debug.Log($"🔊 효과음 재생 중: {soundName}");  // ✅ 실제 재생까지 호출되는지 확인
        }
        else
        {
            Debug.LogError($"❌ 효과음 로드 실패: {soundName} 🔎 확인할 것: Resources/Audio/Sound Effects/{soundName}.wav 또는 .mp3 파일 존재 여부");
        }
    }

    public void GiveItemCommand(string itemName, string itemType)
    {
        Debug.Log($"🎁 아이템 획득: {itemName} | 타입: {itemType}");

        // ✅ 아이템 타입 변환
        if (!System.Enum.TryParse(itemType, out ItemType type))
        {
            Debug.LogError($"❌ 잘못된 아이템 타입: {itemType}");
            return;
        }

        // ✅ 아이템 추가
        Inventory.Instance.AddItemByName(itemName, type);

    }

}
~~~
