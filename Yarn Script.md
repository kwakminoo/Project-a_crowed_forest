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
