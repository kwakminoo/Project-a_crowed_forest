using UnityEngine;

/// <summary>
/// 캐릭터 정보를 담는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "New Character", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("캐릭터 기본 정보")]
    [Tooltip("캐릭터 이름")]
    public string characterName;
    
    [Tooltip("캐릭터 설명")]
    [TextArea(3, 5)]
    public string description;
    
    [Header("캐릭터 이미지")]
    [Tooltip("Resources/UI 폴더에 있는 초상화 스프라이트 이름 (확장자 제외)")]
    public string portraitSpriteName;
    
    /// <summary>
    /// Resources에서 초상화 스프라이트를 로드합니다.
    /// </summary>
    public Sprite GetPortrait()
    {
        if (string.IsNullOrEmpty(portraitSpriteName))
        {
            Debug.LogError($"[CharacterData] {characterName}의 portraitSpriteName이 설정되지 않았습니다.");
            return null;
        }
        
        string path = $"UI/{portraitSpriteName}";
        Sprite sprite = Resources.Load<Sprite>(path);
        
        if (sprite == null)
        {
            Debug.LogError($"[CharacterData] Resources/{path}를 찾을 수 없습니다.");
        }
        
        return sprite;
    }
}

