Custom Line View
-
~~~C#
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class CustomLineView : DialogueViewBase
{
    public Transform contentParent; // Scroll View의 Content 객체
    public GameObject textPrefab;   // 텍스트 프리팹
    public GameObject imagePrefab;  // 이미지 프리팹
    public GameObject buttonPrefab; // 선택지 버튼 프리팹
    public ScrollRect scrollRect;   // 스크롤 가능한 영역

    // 1. 스토리 텍스트 출력
    public override void RunLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        // ClearContent 호출을 하지 않음 -> 텍스트와 이미지를 지우지 않음
        TextMeshProUGUI storyText = contentParent.GetComponentInChildren<TextMeshProUGUI>();
        
        // 텍스트 오브젝트가 없으면 생성
        if (storyText == null)
        {
            Debug.Log("스토리 텍스트가 없으므로 새로 생성합니다.");
            GameObject textObject = Instantiate(textPrefab, contentParent);
            storyText = textObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.Log("기존 스토리 텍스트를 사용합니다.");
        }
        storyText.gameObject.SetActive(true);  // 비활성화된 경우 활성화
        storyText.text = line.TextWithoutCharacterName.Text;

        // 콜백 호출
        onDialogueLineFinished();
        ScrollToBottom();  // 스크롤을 하단으로 이동
    }

    // 2. 이미지 출력 명령어 처리
    [YarnCommand("show_image")]
    public void ShowImage(string imageName)
    {
        // ClearContent 호출을 하지 않음 -> 이미지와 텍스트를 지우지 않음
        Image imageComponent = contentParent.GetComponentInChildren<Image>();
        
        // 이미지 오브젝트가 없으면 생성
        if (imageComponent == null)
        {
            Debug.Log("이미지 오브젝트가 없으므로 새로 생성합니다.");
            GameObject imageObject = Instantiate(imagePrefab, contentParent);
            imageComponent = imageObject.GetComponent<Image>();
        }
        else
        {
            Debug.Log("기존 이미지 오브젝트를 사용합니다.");
        }
        imageComponent.gameObject.SetActive(true);  // 비활성화된 경우 활성화

        // Resources 폴더에서 이미지 로드
        Sprite image = Resources.Load<Sprite>($"Images/{imageName}");
        if (image != null)
        {
            imageComponent.sprite = image;
        }
        else
        {
            Debug.LogError($"이미지 '{imageName}'을(를) 찾을 수 없습니다.");
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

        for (int i = 0; i < options.Length; i++)
        {
            int optionIndex = i;
            GameObject buttonObject = Instantiate(buttonPrefab, contentParent); // 선택지 버튼 생성
            buttonObject.SetActive(true); // 비활성화된 경우 활성화

            TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = options[i].Line.TextWithoutCharacterName.Text;
            }
            else
            {
                Debug.LogError("버튼에 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }

            buttonObject.GetComponent<Button>().onClick.AddListener(() => {
                onOptionSelected(optionIndex);
            });
        }
        ScrollToBottom();  // 스크롤을 하단으로 이동
    }

    // 스크롤을 하단으로 이동시키는 함수
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();  // 강제로 UI 업데이트
        scrollRect.verticalNormalizedPosition = 0f;  // 스크롤을 맨 아래로 이동
    }
}

~~~

이미지 출력
-
~~~C#
    public GameObject imageObject; // 이미지가 표시될 UI 오브젝트

    [YarnCommand("show_image")]
    // 이미지를 보여주는 함수
    void ShowImage(string imageName)
    {
        // 이미지 불러오기 및 표시 로직 구현
        var image = Resources.Load<Sprite>(imageName);
        if (image != null)
        {
            imageObject.GetComponent<UnityEngine.UI.Image>().sprite = image;
            imageObject.SetActive(true); // 이미지 오브젝트 활성화

            textO

bject.transform.SetSiblingIndex(1); // 텍스트 박스를 이미지 아래로 이동
        }
    }
~~~

전투창 출력
-
~~~C#
public CombatManager combatManager; // 전투 관리 스크립트 참조
{
    void Start()
    {
        // DialogueRunner에서 커스텀 명령어를 추가
        var dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler<string, string>("start_combat", StartCombatHandler);
    }

    // 커스텀 명령어를 처리하는 핸들러
    private void StartCombatHandler(string location, string enemyType)
    {
        // CombatManager를 사용하여 전투 시작
        combatManager.StartCombat(location, enemyType);
    }

    public Image backgroundImage; // 배경을 표시할 UI Image 컴포넌트
    public Transform enemySpawnPoint; // 적을 소환할 위치 (적이 배치될 빈 오브젝트)
    
    private GameObject currentEnemy; // 현재 소환된 적을 추적하는 변수

    // 전투 시작 함수 (배경 이름과 적 이름을 받아서 처리)
    public void StartCombat(string location, string enemyType)
    {
        // 배경 스프라이트를 Resources 폴더에서 로드
        Sprite backgroundSprite = Resources.Load<Sprite>($"BackGround/{location}");
        if (backgroundSprite != null)
        {
            backgroundImage.sprite = backgroundSprite;
        }
        else
        {
            Debug.LogError($"배경 스프라이트 '{location}'을 찾을 수 없습니다.");
        }

        // 기존 적 제거 (이미 적이 있을 경우)
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }

        // 적 프리팹을 Resources 폴더에서 로드
        GameObject enemyPrefab = Resources.Load<GameObject>($"Prefabs/Enemies/{enemyType}");
        if (enemyPrefab != null)
        {
            // 적 소환
            currentEnemy = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);
            currentEnemy.transform.SetParent(enemySpawnPoint, false); // 적을 지정된 스폰 포인트에 자식으로 배치
        }
        else
        {
            Debug.LogError($"적 프리팹 '{enemyType}'을 찾을 수 없습니다.");
        }

        // 전투 UI 활성화 (자신의 게임 오브젝트를 활성화)
        gameObject.SetActive(true);
    }

    // 전투 종료 함수
    public void EndCombat()
    {
        // 전투 UI 비활성화
        gameObject.SetActive(false);

        // 현재 적 오브젝트 제거
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }
    }
}

~~~
