이미지
-
~~~C#
    public YarnProject yarnProject;
    public GameObject imageObject; // 이미지가 표시될 UI 오브젝트
    public GameObject textObject;  // 텍스트 박스 UI 오브젝트

    void Start()
    {
        // Yarn Spinner 명령어에 커스텀 명령을 추가
        var dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler<string>("show_image", ShowImage);
    }

    // 이미지를 보여주는 함수
    void ShowImage(string imageName)
    {
        // 이미지 불러오기 및 표시 로직 구현
        var image = Resources.Load<Sprite>(imageName);
        if (image != null)
        {
            imageObject.GetComponent<UnityEngine.UI.Image>().sprite = image;
            imageObject.SetActive(true); // 이미지 오브젝트 활성화

            textObject.transform.SetSiblingIndex(1); // 텍스트 박스를 이미지 아래로 이동
        }
    }
~~~
