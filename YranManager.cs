using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class YarnManager : MonoBehaviour
{
    public Text dialogueText;
    public Button[] choiceButtons;

    [Header("Yarn Settings")]
    public TextAsset yarnScript; // Yarn 스크립트 파일 (.yarn)
    public DialogueRunner dialogueRunner;

    void Start()
    {
        // Yarn 스크립트 로드
        dialogueRunner.AddScript(yarnScript);

        // 대화 이벤트 리스너 등록
        dialogueRunner.onLineUpdate.AddListener(OnLineUpdate);
        dialogueRunner.onOptionsUpdate.AddListener(OnOptionsUpdate);
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);

        // 대화 시작
        StartDialogue();
    }

    void StartDialogue()
    {
        dialogueRunner.StartDialogue();
    }

    void OnLineUpdate(string line)
    {
        dialogueText.text = line;
    }

    void OnOptionsUpdate(System.Collections.Generic.List<string> options)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < options.Count)
            {
                choiceButtons[i].GetComponentInChildren<Text>().text = options[i];
                choiceButtons[i].gameObject.SetActive(true);
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnDialogueComplete()
    {
        Debug.Log("대화 종료");
    }

    public void SelectOption(int index)
    {
        dialogueRunner.SelectOption(index);
    }
}
