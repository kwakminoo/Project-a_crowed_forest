public class BattleEndUI : MonoBehaviour
{
    public GameObject endPanel;
    public TextMeshProUGUI resultText;
    public GameObject rewardPanel;
    public Transform rewardContainer;
    public GameObject rewardItemPrefab;
    public Button confirmButton;

    public void ShowEndUI(bool isVictory, List<Item> rewards)
    {
        endPanel.SetActive(true);
        resultText.text = isVictory ? "VICTORY" : "YOU DIED";
        // 애니메이션 트리거 설정
        Animator animator = endPanel.GetComponent<Animator>();
        animator.SetTrigger("Show");

        if (isVictory && rewards != null && rewards.Count > 0)
        {
            StartCoroutine(ShowRewards(rewards));
        }
        else
        {
            // 일정 시간 후 다음 스토리로 이동
            StartCoroutine(ProceedToNext());
        }
    }

    private IEnumerator ShowRewards(List<Item> rewards)
    {
        yield return new WaitForSeconds(2f); // 결과 텍스트 표시 시간
        rewardPanel.SetActive(true);

        foreach (var item in rewards)
        {
            GameObject rewardObj = Instantiate(rewardItemPrefab, rewardContainer);
            rewardObj.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
            // 아이콘 설정 등 추가
        }

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            // 선택된 보상 처리
            // 예: Inventory.Instance.AddItem(selectedItem);
            StartCoroutine(ProceedToNext());
        });
    }

    private IEnumerator ProceedToNext()
    {
        yield return new WaitForSeconds(1f);
        // UI 닫기 애니메이션
        Animator animator = endPanel.GetComponent<Animator>();
        animator.SetTrigger("Hide");
        yield return new WaitForSeconds(1f);
        endPanel.SetActive(false);
        // 다음 스토리로 이동
        DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner != null)
        {
            dialogueRunner.StartDialogue("NextStoryNode");
        }
    }
}
