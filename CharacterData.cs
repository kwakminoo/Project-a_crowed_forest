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
        
        // 경로 생성 (확장자 제외)
        string path = $"UI/{portraitSpriteName}";
        
        // 먼저 Sprite로 직접 로드 시도
        Sprite sprite = Resources.Load<Sprite>(path);
        
        // Sprite로 로드 실패 시 Texture2D로 로드 후 변환 시도
        if (sprite == null)
        {
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                Debug.Log($"[CharacterData] {characterName}: Texture2D를 Sprite로 변환했습니다.");
            }
        }
        
        if (sprite == null)
        {
            Debug.LogError($"[CharacterData] Resources/{path}를 찾을 수 없습니다. " +
                          $"확인사항: 1) 파일이 Resources/UI 폴더에 있는지, 2) 파일명이 정확한지, 3) Texture Type이 Sprite(2D and UI)로 설정되어 있는지");
        }
        else
        {
            Debug.Log($"[CharacterData] {characterName}: 초상화 로드 성공 - {path}");
        }
        
        return sprite;
    }
}


