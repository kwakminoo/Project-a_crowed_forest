이미지
-
~~~C#
 public TextMeshProUGUI dialogueText;  // Yarn 텍스트를 출력할 UI
    public Image characterImage;          // 캐릭터나 배경 이미지를 출력할 UI

    private DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        // Yarn 커스텀 명령 추가 (string[] 매개변수를 받도록 수정)
        dialogueRunner.AddCommandHandler("show_image", ShowImage);
        dialogueRunner.AddCommandHandler("hide_image", HideImage);

        // 이미지 초기 비활성화
        characterImage.enabled = false;
    }

    // Yarn 명령: show_image
    public void ShowImage(string[] parameters)
    {
        if (parameters.Length > 0)
        {
            string imageName = parameters[0];  // 첫 번째 매개변수를 이미지 이름으로 사용
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
        else
        {
            Debug.LogError("No image name provided for show_image command");
        }
    }

    // Yarn 명령: hide_image
    public void HideImage(string[] parameters)
    {
        characterImage.enabled = false;
    }
~~~
