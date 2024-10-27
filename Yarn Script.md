Custom Line View
-
~~~C#
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using Yarn;
using System.Collections;

public class CustomLineView : DialogueViewBase
{
    public Transform contentParent; // Scroll View의 Content 객체
    public GameObject textPrefab;   // 텍스트 프리팹
    public GameObject imagePrefab;  // 이미지 프리팹
    public GameObject buttonPrefab; // 선택지 버튼 프리팹
    public ScrollRect scrollRect;   // 스크롤 가능한 영역
    private System.Action onDialogueLineFinishedCallBack;

    public float typingSpeed = 0.05f;
    private bool isTyping = false;
    private string fullText;

    void Start()
    {
        var dialogueRunner = FindObjectOfType<DialogueRunner>();
        if(dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("show_image", ShowImage);
            dialogueRunner.AddCommandHandler<string, string, string>("start_Battle", StartBattleCommand);
        }
        else
        {
            Debug.LogError("다이얼로그 러너를 찾을 수 없습니다");
        }
    }

    public override void RunLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {
        onDialogueLineFinishedCallBack = onDialogueLineFinished;

        scrollRect.GetComponent<Button>().onClick.RemoveAllListeners();
        scrollRect.GetComponent<Button>().onClick.AddListener(() => CompleteTyping());


        //CreateCountinueButton(line, onDialogueLineFinished);
        StartCoroutine(TypeLine(line, onDialogueLineFinished));
    }
    
    // 1. 스토리 텍스트 출력
    private IEnumerator TypeLine(LocalizedLine line, System.Action onDialogueLineFinished)
    {

        isTyping = true;
        fullText = line.TextWithoutCharacterName.Text;
        // ClearContent 호출을 하지 않음 -> 텍스트와 이미지를 지우지 않음
        TextMeshProUGUI storyText = contentParent.GetComponentInChildren<TextMeshProUGUI>();

        foreach (Transform child in contentParent)
        {
            Button button = child.GetComponent<Button>();
            if(button != null)
            {
                child.gameObject.SetActive(false);
            }
        }

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
        storyText.text = "";

        foreach(char latter in fullText.ToCharArray())
        {
            if(!isTyping) break;
            storyText.text += latter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        storyText.text = fullText;
        // 콜백 호출
        onDialogueLineFinishedCallBack?.Invoke();

        foreach(Transform child in contentParent)
        {
            Button button = child.GetComponent<Button>();
            if(button != null)
            {
                child.gameObject.SetActive(true);
            }
        }

        ScrollToBottom();  // 스크롤을 하단으로 이동
    }

    private void CompleteTyping()
    {
        if (isTyping)
        {
            isTyping = false;
            TextMeshProUGUI storyText = contentParent.GetComponentInChildren<TextMeshProUGUI>();
            storyText.text = fullText;
        }
        else
        {
            onDialogueLineFinishedCallBack?.Invoke();
        }
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

    public void StartBattleCommand(string enemyName, string enemySpriteName, string backGroundName)
    {
        var BattleManager = FindObjectOfType<BattleManager>();
        if(BattleManager != null)
        {
            BattleManager.StartBattle(enemyName, enemySpriteName, backGroundName);
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

        for (int i = 0; i < options.Length; i++)
        {
            int optionIndex = i;
            GameObject buttonObject = Instantiate(buttonPrefab, contentParent); // 선택지 버튼 생성
            buttonObject.SetActive(true); // 비활성화된 경우 활성화

            RectTransform rectTransfrom = buttonObject.GetComponent<RectTransform>();
            rectTransfrom.anchoredPosition3D = Vector3.zero;
            rectTransfrom.localScale = Vector3.one;

            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            Debug.Log($"버튼 생성됨 {buttonObject.name}, 위치: {rectTransform.anchoredPosition}");

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
            button.onClick.AddListener(() => {
                Debug.Log($"버튼 {optionIndex} 클릭됨");
                button.interactable = false;  // 클릭 후 다시 선택되지 않도록 비활성화

                foreach(Transform child in contentParent)
                {
                    if(child.GetComponent<Image>() != null)
                    {
                        Destroy(child.gameObject);
                        Debug.Log("이미지 삭제");
                    }
                }
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


전투창 출력
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class BattleManager : MonoBehaviour
{
    public GameObject battleWindow;
    public Image enemyImage;
    public TextMeshProUGUI enemyNameText;
    public Image backGroundImage;

    public void StartBattle(string enemyName, string enemySpriteName, string backGroundName)
    {
        battleWindow.SetActive(true);

        enemyNameText.text = enemyName;

        Sprite enemySprite = Resources.Load<Sprite>($"Character/{enemySpriteName}");
        if(enemySprite != null)
        {
            enemyImage.sprite = enemySprite;
        }
        else
        {
            Debug.LogError($"{enemySpriteName}을 찾을 수 없습니다");
        }

        Sprite backGroundSprite = Resources.Load<Sprite>($"Battle Background/{backGroundName}");
        if(backGroundSprite != null)
        {
            backGroundImage.sprite = backGroundSprite;
        }
        else
        {
            Debug.LogError($"{backGroundName}을 찾을 수 없습니다");
        }
    }

    public void EndBattle()
    {
        battleWindow.SetActive(false);
    }
}

~~~
