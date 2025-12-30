using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 저장 데이터 구조체
/// JSON 직렬화를 위해 Serializable로 표시
/// </summary>
[Serializable]
public class GameSaveData
{
    [Header("저장 메타데이터")]
    public int slotNumber;                    // 슬롯 번호 (1, 2, 3)
    public string saveDate;                    // 저장 날짜/시간 (ISO 8601 형식)
    public string characterName;               // 캐릭터 이름
    public string yarnScriptName;              // Yarn 스크립트 이름
    
    [Header("Yarn 스크립트 데이터")]
    public string currentNodeName;             // 현재 Yarn 노드 이름
    public List<string> yarnVariableKeys;      // Yarn 변수 키 목록 (JSON 직렬화를 위해 List 사용)
    public List<string> yarnVariableValues;    // Yarn 변수 값 목록
    
    [Header("플레이어 데이터")]
    public string playerCharacterName;         // 플레이어 캐릭터 이름
    public int level;                         // 레벨
    public int experience;                     // 경험치
    public int experienceToNextLevel;          // 다음 레벨까지 필요 경험치
    public int maxHP;                          // 최대 HP
    public int currentHP;                     // 현재 HP
    public int maxSanity;                      // 최대 정신력
    public int currentSanity;                  // 현재 정신력
    public int strength;                       // 근력
    public int dexterity;                      // 기량
    public int vitality;                       // 체력
    public int willpower;                      // 정신력
    public int luck;                           // 운
    public int currentMadnessState;            // 현재 광기 상태 (enum을 int로 저장)
    
    [Header("인벤토리 데이터")]
    public List<string> itemNames;            // 아이템 이름 목록 (ScriptableObject 참조용)
    public List<int> itemTypes;                // 아이템 타입 목록 (enum을 int로 저장)
    public List<string> skillNames;            // 스킬 이름 목록
    public string equippedWeaponName;          // 장착된 무기 이름
    public string equippedTopName;            // 장착된 상의 이름
    public string equippedBottomName;          // 장착된 하의 이름
    public List<string> skillSlotNames;       // 스킬 슬롯에 할당된 스킬 이름들 (최대 4개)
    public List<string> consumableItemSlotNames; // 소비 아이템 슬롯 이름들 (최대 4개)
    
    [Header("오디오 데이터")]
    public string currentBGMName;              // 현재 재생 중인 BGM 이름
    
    /// <summary>
    /// 기본 생성자
    /// </summary>
    public GameSaveData()
    {
        slotNumber = 1;
        saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        characterName = "";
        yarnScriptName = "";
        currentNodeName = "";
        yarnVariableKeys = new List<string>();
        yarnVariableValues = new List<string>();
        
        playerCharacterName = "";
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;
        maxHP = 100;
        currentHP = 100;
        maxSanity = 100;
        currentSanity = 100;
        strength = 5;
        dexterity = 5;
        vitality = 5;
        willpower = 5;
        luck = 5;
        currentMadnessState = 0;
        
        itemNames = new List<string>();
        itemTypes = new List<int>();
        skillNames = new List<string>();
        equippedWeaponName = "";
        equippedTopName = "";
        equippedBottomName = "";
        skillSlotNames = new List<string>();
        consumableItemSlotNames = new List<string>();
        
        currentBGMName = "";
    }
    
    /// <summary>
    /// 저장 데이터가 유효한지 확인
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(currentNodeName) && 
               !string.IsNullOrEmpty(characterName);
    }
}

