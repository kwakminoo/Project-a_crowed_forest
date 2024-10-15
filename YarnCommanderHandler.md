이미지 출력
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
