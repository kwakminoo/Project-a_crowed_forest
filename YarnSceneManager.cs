using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class YarnSceneManager : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner.AddCommandHandler("SelectClass", SelectClass);
    }

    void SelectClass(string[] parameters)
    {
        if (parameters.Length < 1)
        {
            Debug.LogError("SelectClass command requires a class parameter.");
            return;
        }

        string selectedClass = parameters[0];

        // 예시: 선택에 따른 씬 로드
        switch (selectedClass)
        {
            case "전투":
                SceneManager.LoadScene("BattleScene");
                break;
            default:
                Debug.LogWarning($"Unknown class: {selectedClass}");
                break;
        }
    }
}
