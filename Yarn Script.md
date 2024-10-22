Custom Line View
-
~~~C#
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;
using TMPro;
using System;

public class CustomLineView : DialogueViewBase
{
    // UI 요소들: 텍스트, 이미지, 선택지 버튼 등이 배치될 부모 오브젝트
    public Transform contentParent;  // 스크롤 뷰 안에 들어갈 컨텐츠(텍스트, 이미지, 선택지)
    public GameObject textPrefab;    // 텍스트를 담을 프리팹
    public GameObject optionButtonPrefab;  // 선택지 버튼 프리팹
    public ScrollRect scrollRect;    // 스크롤 가능한 영역
    private GameObject activeTextObject;  // 기존 텍스트 오브젝트를 저장할 변수



    // 대사 출력
    public override void RunLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        if (textPrefab == null)
        {
            Debug.LogError("textPrefab이 설정되지 않았습니다!");
            return;
        }

        if (contentParent == null)
        {
            Debug.LogError("contentParent가 설정되지 않았습니다!");
            return;
        }

        // 기존 텍스트 오브젝트가 있는지 contentParent의 자식 오브젝트에서 확인
        TextMeshProUGUI textComponent = contentParent.GetComponentInChildren<TextMeshProUGUI>();

        // 텍스트 오브젝트가 없으면 새로 생성
        if (textComponent == null)
        {
            GameObject textObject = Instantiate(textPrefab, contentParent);
            textComponent = textObject.GetComponent<TextMeshProUGUI>();
        }

        if (textComponent == null)
        {
            Debug.LogError("Text 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        // 텍스트 업데이트
        textComponent.text = line.TextWithoutCharacterName.Text;

        // 대사 출력 완료 후 콜백 호출
        onDialogueLineFinished();

        // 스크롤 위치 갱신
        UpdateScrollPosition();
    }


    // 스크롤 업데이트 함수
    private void UpdateScrollPosition()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;  // 스크롤이 아래로 이동
    }

    // 3. 선택지 출력
    public override void RunOptions(DialogueOption[] options, System.Action<int> onOptionSelected)
{
    Debug.Log($"RunOptions 호출됨. 선택지 개수: {options.Length}");

    foreach (Transform child in contentParent)
    {
        if (child.GetComponent<Button>())
        {
            Destroy(child.gameObject);
        }
    }

    for (int i = 0; i < options.Length; i++)
    {
        Debug.Log($"선택지 {i + 1}: {options[i].Line.TextWithoutCharacterName.Text}");

        int optionIndex = i;
        GameObject button = Instantiate(optionButtonPrefab, contentParent);

        // 버튼이 비활성화된 경우 강제 활성화
        button.SetActive(true);

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = options[i].Line.TextWithoutCharacterName.Text;
            Debug.Log($"선택지 버튼 생성됨: {buttonText.text}");
        }
        else
        {
            Debug.LogError("Button에 TextMeshProUGUI 컴포넌트가 없습니다.");
        }

        button.GetComponent<Button>().onClick.AddListener(() => {
            onOptionSelected(optionIndex);
            ClearOptions();
        });
    }

    scrollRect.verticalNormalizedPosition = 1f;
}




    // 선택지 제거
    private void ClearOptions()
    {
        foreach (Transform child in contentParent)
        {
            if (child.GetComponent<Button>())
            {
                Destroy(child.gameObject);
            }
        }
    }

    // 4. 스크롤 기능 활성화
    private void Update()
    {
        // 텍스트나 이미지가 추가되면 스크롤 영역을 다시 갱신
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;  // 스크롤이 아래로 자동으로 내려가도록 설정
    }
}
~~~

~~~C#
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;
using TMPro;
using System;

public class CustomLineView : DialogueViewBase
{
    // UI 요소들: 텍스트, 이미지, 선택지 버튼 등이 배치될 부모 오브젝트
    public Transform contentParent;  // 스크롤 뷰 안에 들어갈 컨텐츠(텍스트, 이미지, 선택지)
    public GameObject textPrefab;    // 텍스트를 담을 프리팹
    public GameObject imagePrefab;   // 이미지를 담을 프리펩
    public GameObject optionButtonPrefab;  // 선택지 버튼 프리팹
    public ScrollRect scrollRect;    // 스크롤 가능한 영역
    private GameObject activeTextObject;  // 기존 텍스트 오브젝트를 저장할 변수

    private Queue<GameObject> buttonPool = new Queue<GameObject>();

    // 대사 출력
    public override void RunLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        ClearContent();

        if (textPrefab == null)
        {
            Debug.LogError("textPrefab이 설정되지 않았습니다!");
            return;
        }

        if (contentParent == null)
        {
            Debug.LogError("contentParent가 설정되지 않았습니다!");
            return;
        }

        GameObject textObject = InstantiateOrReuse(textPrefab, contentParent);
        TextMeshProUGUI storyText = textObject.GetComponent<TextMeshProUGUI>();
        storyText.text = line.TextWithoutCharacterName.Text;

        onDialogueLineFinished();
        ScrollBottom();
    }

    [YarnCommand("show_image")]
    public void ShowImage(string imageName)
    {
        GameObject imageObject = Instantiate(imagePrefab, contentParent);
        Image imageComponent = imageObject.GetComponent<Image>();
        Sprite image = Resources.Load<Sprite>($"Images/{imageName}");
        if(image != null)
        {
            imageComponent.sprite = image;
        }
        else
        {
            Debug.LogError($"이미지 '{imageName}을 찾을 수 없습니다 경로를 확인하세요");
        }

    }

    [YarnCommand("hide_image")]
    public void HideImage()
    {
        imagePrefab.SetActive(false);
    }
    // 3. 선택지 출력
    public override void RunOptions(DialogueOption[] options, System.Action<int> onOptionSelected)
    {
        Debug.Log($"RunOptions 호출됨. 선택지 개수: {options.Length}");

        ClearContent();

        if(!contentParent.gameObject.activeInHierarchy)
        {
            contentParent.gameObject.SetActive(true);
        }

        for (int i = 0; i < options.Length; i++)
        {
            Debug.Log($"선택지 {i + 1}: {options[i].Line.TextWithoutCharacterName.Text}");

            int optionIndex = i;

            GameObject button = GetOrCreateButton();
            button.transform.SetParent(contentParent, false);
            button.SetActive(true);

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = options[i].Line.TextWithoutCharacterName.Text;

            button.GetComponent<Button>().onClick.AddListener(() => {
                onOptionSelected(optionIndex);
                ClearContent();
            });
        }
        ScrollBottom();
    }

    private GameObject GetOrCreateButton()
    {
        if(buttonPool.Count > 0)
        {
            return buttonPool.Dequeue();
        }
        else
        {
            return Instantiate(optionButtonPrefab);
        }
    }

    private void ClearContent()
    {
        foreach(Transform child in contentParent)
        {
            if(child.gameObject.CompareTag("Button"))
            {
                child.gameObject.SetActive(false);
                buttonPool.Enqueue(child.gameObject);
            }
            Destroy(child.gameObject);
        }
    }

    // 4. 스크롤 기능 활성화
    private void ScrollBottom()
    {
        // 텍스트나 이미지가 추가되면 스크롤 영역을 다시 갱신
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;  // 스크롤이 아래로 자동으로 내려가도록 설정
    }

    private GameObject InstantiateOrReuse(GameObject prefabs, Transform parent)
    {
        GameObject instance = Instantiate(prefabs, parent);
        instance.SetActive(true);
        return instance;
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
