Custom Line View
-
~~~C#
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class CustemLineView : DialogueViewBase
{
    public RectTransform contentParent; // Content 오브젝트 참조
    public Image imagePrefab;           // 이미지 프리팹 참조 (Inspector에서 설정)
    public TextMeshProUGUI textPrefab;  // 텍스트 프리팹 참조 (Inspector에서 설정)
    public Button buttonPrefab;         // 버튼 프리팹 참조 (Inspector에서 설정)

    private List<Button> activeButtons = new List<Button>();  // 현재 활성화된 버튼 목록

    void Start()
    {
        // 계층에 있는 오브젝트들을 초기화할 때 비활성화 상태로 설정
        foreach (Transform child in contentParent)
        {
            if (child.GetComponent<Button>())
            {
                child.gameObject.SetActive(false);  // 버튼 비활성화
            }
        }
    }

    // 다이얼로그 텍스트 출력
    public override void RunLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        // 텍스트 출력
        DisplayStoryText(line.TextWithoutCharacterName.Text);
        onDialogueLineFinished?.Invoke(); // 라인 출력 완료 시 콜백 호출
    }

    // 선택지 출력
    public override void RunOptions(DialogueOption[] options, System.Action<int> onOptionSelected)
    {
        // 선택지 버튼 출력
        DisplayOptions(options, onOptionSelected);
    }

    // 이미지, 텍스트, 선택지 버튼을 동시에 출력
    public void DisplayContent(string imagePath, string storyText, DialogueOption[] options, System.Action<int> onOptionSelected)
    {
        // 이미지 출력
        DisplayImage(imagePath);

        // 스토리 텍스트 출력
        DisplayStoryText(storyText);

        // 선택지 버튼 출력
        DisplayOptions(options, onOptionSelected);
    }

    // 이미지 출력 함수
    private void DisplayImage(string imagePath)
    {
        Image imageObject = GetOrCreateImage(); // 이미지 오브젝트 생성 또는 가져오기
        Sprite newSprite = Resources.Load<Sprite>(imagePath); // Resources에서 이미지 로드

        if (newSprite != null)
        {
            imageObject.sprite = newSprite;
            imageObject.gameObject.SetActive(true);  // 이미지 활성화
        }
        else
        {
            Debug.LogError($"이미지 '{imagePath}'를 찾을 수 없습니다.");
        }
    }

    // 스토리 텍스트 출력 함수
    private void DisplayStoryText(string storyText)
    {
        TextMeshProUGUI textObject = GetOrCreateText(); // 텍스트 오브젝트 생성 또는 가져오기
        textObject.text = storyText;  // 텍스트 설정
        textObject.gameObject.SetActive(true);  // 텍스트 활성화
    }

    // 선택지 버튼 출력 함수
    private void DisplayOptions(DialogueOption[] options, System.Action<int> onOptionSelected)
    {
        // 기존 버튼 비활성화
        foreach (Button button in activeButtons)
        {
            button.gameObject.SetActive(false);
        }

        // 필요한 수의 버튼 생성 또는 재사용
        for (int i = 0; i < options.Length; i++)
        {
            Button button = GetOrCreateButton(i); // 버튼 생성 또는 재사용
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = options[i].Line.TextWithoutCharacterName.Text;  // 버튼 텍스트 설정

            int optionIndex = i;  // 캡처 문제 방지
            button.onClick.RemoveAllListeners();  // 기존 리스너 제거
            button.onClick.AddListener(() => {
                onOptionSelected(optionIndex);  // 선택 시 콜백 호출
                button.gameObject.SetActive(false);  // 선택한 버튼 비활성화
            });

            button.gameObject.SetActive(true);  // 버튼 활성화
        }

        // 스크롤을 상단으로 초기화
        contentParent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    // 이미지 오브젝트 생성 또는 가져오기
    private Image GetOrCreateImage()
    {
        // 이미 있는 이미지 오브젝트 사용
        foreach (Transform child in contentParent)
        {
            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                return image;
            }
        }

        // 없으면 새로 생성
        Image newImage = Instantiate(imagePrefab, contentParent);
        return newImage;
    }

    // 텍스트 오브젝트 생성 또는 가져오기
    private TextMeshProUGUI GetOrCreateText()
    {
        // 이미 있는 텍스트 오브젝트 사용
        foreach (Transform child in contentParent)
        {
            TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                return text;
            }
        }

        // 없으면 새로 생성
        TextMeshProUGUI newText = Instantiate(textPrefab, contentParent);
        return newText;
    }

    // 버튼 오브젝트 생성 또는 가져오기
    private Button GetOrCreateButton(int index)
    {
        if (index < activeButtons.Count)
        {
            return activeButtons[index];  // 이미 있는 버튼 반환
        }
        else
        {
            // 새로운 버튼 생성
            Button newButton = Instantiate(buttonPrefab, contentParent);
            activeButtons.Add(newButton);
            return newButton;
        }
    }
}
~~~


커스텀 라인뷰 수정
-
~~~C#
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using System.Collections;

public class CustomLineView : DialogueViewBase
{
    public Transform contentParent; // Scroll View의 Content 객체
    public GameObject textPrefab;   // 텍스트 프리팹
    public GameObject imagePrefab;  // 이미지 프리팹
    public GameObject buttonPrefab; // 선택지 버튼 프리팹
    public ScrollRect scrollRect;   // 스크롤 가능한 영역
    private System.Action onDialogueLineFinishedCallback;

    public float typingSpeed = 0.05f;
    private bool isTyping = false;
    private string fullText;

    private bool isBattleActive = false; // **수정: 배틀 활성 상태 플래그 추가**

    void Start()
    {
        var dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("show_image", ShowImage);
            dialogueRunner.AddCommandHandler<string, string>("start_Battle", StartBattleCommand); // **수정: 배틀 시작 명령어 추가**
        }
        else
        {
            Debug.LogError("다이얼로그 러너를 찾을 수 없습니다");
        }
    }

    public override void RunLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        if (isBattleActive) // **수정: 배틀 중일 경우 출력 중단**
        {
            onDialogueLineFinished?.Invoke();
            return;
        }

        onDialogueLineFinishedCallback = onDialogueLineFinished;

        scrollRect.GetComponent<Button>().onClick.RemoveAllListeners();
        scrollRect.GetComponent<Button>().onClick.AddListener(() => CompleteTyping());

        StartCoroutine(TypeLine(line, onDialogueLineFinished));
    }

    private IEnumerator TypeLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        isTyping = true;
        fullText = line.TextWithoutCharacterName.Text;

        // 기존 텍스트와 버튼 유지 (새로운 텍스트를 아래에 추가)
        GameObject textObject = Instantiate(textPrefab, contentParent);
        TextMeshProUGUI storyText = textObject.GetComponent<TextMeshProUGUI>();
        storyText.text = "";

        foreach (char letter in fullText.ToCharArray())
        {
            if (!isTyping) break;
            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        storyText.text = fullText;
        onDialogueLineFinished?.Invoke();

        ScrollToBottom();
    }

    private void CompleteTyping()
    {
        if (isTyping)
        {
            isTyping = false;
            var storyTexts = contentParent.GetComponentsInChildren<TextMeshProUGUI>();
            if (storyTexts.Length > 0)
            {
                storyTexts[storyTexts.Length - 1].text = fullText; // 마지막 텍스트 완성
            }
        }
        else
        {
            onDialogueLineFinishedCallback?.Invoke();
        }
    }

    public void ShowImage(string imageName)
    {
        Sprite image = Resources.Load<Sprite>($"Images/{imageName}");

        if (image == null)
        {
            Debug.LogError($"{imageName}을 찾을 수 없습니다");
            return;
        }

        GameObject imageObject = Instantiate(imagePrefab, contentParent);
        Image imageComponent = imageObject.GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("이미지 컴포넌트를 찾을 수 없습니다");
            return;
        }
        imageComponent.sprite = image;
        imageObject.SetActive(true);
        imageObject.transform.SetAsFirstSibling();
    }

    public void StartBattleCommand(string enemyDataName, string backGroundName)
    {
        isBattleActive = true; // **수정: 배틀 활성화 플래그 설정**

        EnemyData enemyData = Resources.Load<EnemyData>($"Character/{enemyDataName}");
        if (enemyData == null)
        {
            Debug.LogError($"적 데이터 {enemyDataName}을 찾을 수 없습니다.");
            return;
        }

        var battleManager = FindObjectOfType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.StartBattle(enemyData, backGroundName);

            // 배틀 종료 후 스토리 재개
            StartCoroutine(WaitForBattleEnd(battleManager));
        }
        else
        {
            Debug.LogError("BattleManager를 찾을 수 없습니다.");
        }
    }

    private IEnumerator WaitForBattleEnd(BattleManager battleManager)
    {
        while (battleManager.IsBattleActive()) // **수정: 배틀 진행 중 체크**
        {
            yield return null;
        }

        isBattleActive = false; // **수정: 배틀 종료 후 플래그 해제**
        Debug.Log("배틀 종료. 스토리 재개.");
    }

    public override void RunOptions(DialogueOption[] options, System.Action<int> onOptionSelected)
    {
        if (isBattleActive) return; // **수정: 배틀 중 선택지 출력 중단**

        foreach (var option in options)
        {
            GameObject buttonObject = Instantiate(buttonPrefab, contentParent);
            var buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = option.Line.TextWithoutCharacterName.Text;
            }

            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(() => onOptionSelected(option.ID));
        }

        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    // **추가: 타이틀 시작 시 텍스트/버튼 초기화**
    private void ClearContent()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}

~~~
