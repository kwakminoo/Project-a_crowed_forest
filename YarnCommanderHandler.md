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
