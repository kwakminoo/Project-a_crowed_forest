이미지
-
~~~C#
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

public class YarnTextAndImageManager : MonoBehaviour
{
    public Image dialogueImage;           // 텍스트와 함께 출력할 이미지 UI
    public Sprite[] sprites;             // 여러 스프라이트 이미지 배열

    private DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        // Yarn 커스텀 명령 추가
        dialogueRunner.AddCommandHandler("showImage", ShowImage);
        dialogueRunner.AddCommandHandler("hideImage", HideImage);
    }

    // Yarn 커스텀 명령: showImage
    void ShowImage(string[] parameters)
    {
        string imageName = parameters[0];

        // 이미지 이름에 맞는 스프라이트 찾기
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == imageName)
            {
                displayImage.sprite = sprite;        // 스프라이트를 UI에 할당
                displayImage.gameObject.SetActive(true); // 이미지 보이기
                break;
            }
        }
    }

    // Yarn 커스텀 명령: hideImage
    void HideImage(string[] parameters)
    {
        displayImage.gameObject.SetActive(false); // 이미지 숨기기
    }
}

~~~


~~~C#
public TextMeshProUGUI dialogueText;  // Yarn 텍스트를 출력할 UI
    public Image characterImage;          // 캐릭터나 배경 이미지를 출력할 UI

    private DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        // Yarn 커스텀 명령 추가
        dialogueRunner.AddCommandHandler("show_image", ShowImage);
        dialogueRunner.AddCommandHandler("hide_image", HideImage);

        // 이미지 초기 비활성화
        characterImage.enabled = false;
    }

    // Yarn 명령: show_image
    public void ShowImage(string imageName)
    {
        Sprite newSprite = Resources.Load<Sprite>("Images/" + imageName);
        if (newSprite != null)
        {
            characterImage.sprite = newSprite;
            characterImage.enabled = true;
        }
        else
        {
            Debug.LogError("Image not found: " + imageName);
        }
    }

    // Yarn 명령: hide_image
    public void HideImage()
    {
        characterImage.enabled = false;
    }
~~~
