using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// 게임 저장/로드 시스템 싱글톤
/// 파일 시스템을 사용하여 JSON 형식으로 저장
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    
    private const string SAVE_FOLDER_NAME = "SaveData";
    private const int MAX_SLOTS = 3;
    private int currentSlotNumber = 1; // 현재 사용 중인 슬롯 번호
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 저장 폴더 생성
            EnsureSaveDirectoryExists();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 저장 디렉토리가 존재하는지 확인하고 없으면 생성
    /// </summary>
    private void EnsureSaveDirectoryExists()
    {
        string savePath = GetSaveDirectoryPath();
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            Debug.Log($"[SaveSystem] 저장 디렉토리 생성: {savePath}");
        }
    }
    
    /// <summary>
    /// 저장 디렉토리 경로 반환
    /// </summary>
    private string GetSaveDirectoryPath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FOLDER_NAME);
    }
    
    /// <summary>
    /// 특정 슬롯의 저장 파일 경로 반환
    /// </summary>
    private string GetSaveFilePath(int slotNumber)
    {
        return Path.Combine(GetSaveDirectoryPath(), $"save_slot_{slotNumber}.json");
    }
    
    /// <summary>
    /// 현재 게임 상태를 저장
    /// </summary>
    /// <param name="slotNumber">슬롯 번호 (1-3)</param>
    /// <returns>저장 성공 여부</returns>
    public bool SaveGame(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS)
        {
            Debug.LogError($"[SaveSystem] 잘못된 슬롯 번호: {slotNumber}");
            return false;
        }
        
        try
        {
            GameSaveData saveData = new GameSaveData();
            saveData.slotNumber = slotNumber;
            saveData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            // Yarn 스크립트 데이터 저장
            SaveYarnData(saveData);
            
            // 플레이어 데이터 저장
            SavePlayerData(saveData);
            
            // 인벤토리 데이터 저장
            SaveInventoryData(saveData);
            
            // 오디오 데이터 저장
            SaveAudioData(saveData);
            
            // JSON으로 직렬화하여 파일에 저장
            string json = JsonUtility.ToJson(saveData, true);
            string filePath = GetSaveFilePath(slotNumber);
            
            // 임시 파일에 먼저 저장 (원자성 보장)
            string tempFilePath = filePath + ".tmp";
            File.WriteAllText(tempFilePath, json);
            
            // 성공 시 실제 파일로 이동
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.Move(tempFilePath, filePath);
            
            currentSlotNumber = slotNumber;
            
            Debug.Log($"[SaveSystem] 게임 저장 완료: 슬롯 {slotNumber} ({saveData.saveDate})");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] 게임 저장 실패: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Yarn 스크립트 데이터 저장
    /// </summary>
    private void SaveYarnData(GameSaveData saveData)
    {
        DialogueRunner dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        if (dialogueRunner != null)
        {
            // 현재 노드 이름 저장
            // DialogueRunner에서 현재 노드 이름을 가져오는 방법이 제한적이므로
            // CustomLineView에서 PlayerPrefs에 저장한 값을 사용
            
            // Yarn 변수 저장 (나중에 필요시 구현)
            // DialogueRunner의 variableStorage는 직접 접근이 어려우므로
            // 현재는 변수 저장을 건너뜁니다
            saveData.yarnVariableKeys = new List<string>();
            saveData.yarnVariableValues = new List<string>();
            // TODO: Yarn 변수 저장 로직 추가 (필요시 DialogueRunner의 VariableStorage를 직접 찾아서 사용)
        }
        
        // CustomLineView에서 설정한 값들 사용
        string currentNode = PlayerPrefs.GetString("CurrentYarnNode", "");
        if (!string.IsNullOrEmpty(currentNode))
        {
            saveData.currentNodeName = currentNode;
        }
        
        string characterName = PlayerPrefs.GetString("SelectedCharacterName", "");
        if (!string.IsNullOrEmpty(characterName))
        {
            saveData.characterName = characterName;
        }
        
        string yarnScript = PlayerPrefs.GetString("YarnScriptName", "");
        if (!string.IsNullOrEmpty(yarnScript))
        {
            saveData.yarnScriptName = yarnScript;
        }
    }
    
    /// <summary>
    /// 플레이어 데이터 저장
    /// </summary>
    private void SavePlayerData(GameSaveData saveData)
    {
        if (Player.Instance == null)
        {
            Debug.LogWarning("[SaveSystem] Player.Instance가 null입니다.");
            return;
        }
        
        Player player = Player.Instance;
        saveData.playerCharacterName = player.characterName;
        saveData.level = player.level;
        saveData.experience = player.experience;
        saveData.experienceToNextLevel = player.experienceToNextLevel;
        saveData.maxHP = player.maxHP;
        saveData.currentHP = player.currentHP;
        saveData.maxSanity = player.maxSanity;
        saveData.currentSanity = player.currentSanity;
        saveData.strength = player.strength;
        saveData.dexterity = player.dexterity;
        saveData.vitality = player.vitality;
        saveData.willpower = player.willpower;
        saveData.luck = player.luck;
        saveData.currentMadnessState = (int)player.currentMadnessState;
    }
    
    /// <summary>
    /// 인벤토리 데이터 저장
    /// </summary>
    private void SaveInventoryData(GameSaveData saveData)
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("[SaveSystem] Inventory.Instance가 null입니다.");
            return;
        }
        
        Inventory inventory = Inventory.Instance;
        
        // 아이템 목록 저장 (ScriptableObject 참조는 이름으로 저장)
        saveData.itemNames = new List<string>();
        saveData.itemTypes = new List<int>();
        foreach (Item item in inventory.items)
        {
            if (item != null)
            {
                saveData.itemNames.Add(item.itemName);
                saveData.itemTypes.Add((int)item.itemType);
            }
        }
        
        // 스킬 목록 저장
        saveData.skillNames = new List<string>();
        foreach (Skill skill in inventory.skills)
        {
            if (skill != null)
            {
                saveData.skillNames.Add(skill.skillName);
            }
        }
        
        // 장착된 아이템 저장
        saveData.equippedWeaponName = inventory.equippedWeapon?.itemName ?? "";
        saveData.equippedTopName = inventory.equippedTop?.itemName ?? "";
        saveData.equippedBottomName = inventory.equippedBottom?.itemName ?? "";
        
        // 스킬 슬롯 저장
        saveData.skillSlotNames = new List<string>();
        foreach (Skill skill in inventory.skillSlots)
        {
            saveData.skillSlotNames.Add(skill?.skillName ?? "");
        }
        
        // 소비 아이템 슬롯 저장
        saveData.consumableItemSlotNames = new List<string>();
        foreach (Item item in inventory.consumableItemSlots)
        {
            saveData.consumableItemSlotNames.Add(item?.itemName ?? "");
        }
    }
    
    /// <summary>
    /// 오디오 데이터 저장
    /// </summary>
    private void SaveAudioData(GameSaveData saveData)
    {
        CustomLineView lineView = FindFirstObjectByType<CustomLineView>();
        if (lineView != null && lineView.bgmSource != null && lineView.bgmSource.clip != null)
        {
            saveData.currentBGMName = lineView.bgmSource.clip.name;
        }
    }
    
    /// <summary>
    /// 저장된 게임 데이터 로드
    /// </summary>
    /// <param name="slotNumber">슬롯 번호 (1-3)</param>
    /// <returns>로드된 게임 데이터, 실패 시 null</returns>
    public GameSaveData LoadGame(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS)
        {
            Debug.LogError($"[SaveSystem] 잘못된 슬롯 번호: {slotNumber}");
            return null;
        }
        
        string filePath = GetSaveFilePath(slotNumber);
        
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[SaveSystem] 저장 파일이 존재하지 않습니다: {filePath}");
            return null;
        }
        
        try
        {
            string json = File.ReadAllText(filePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            
            if (saveData == null || !saveData.IsValid())
            {
                Debug.LogError($"[SaveSystem] 저장 데이터가 유효하지 않습니다: 슬롯 {slotNumber}");
                return null;
            }
            
            currentSlotNumber = slotNumber;
            Debug.Log($"[SaveSystem] 게임 로드 완료: 슬롯 {slotNumber} ({saveData.saveDate})");
            return saveData;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] 게임 로드 실패: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 로드된 데이터를 게임에 적용
    /// </summary>
    public void ApplyLoadedData(GameSaveData saveData)
    {
        if (saveData == null)
        {
            Debug.LogError("[SaveSystem] 적용할 저장 데이터가 null입니다.");
            return;
        }
        
        // 플레이어 데이터 적용
        ApplyPlayerData(saveData);
        
        // 인벤토리 데이터 적용
        ApplyInventoryData(saveData);
        
        // Yarn 스크립트 데이터 적용
        ApplyYarnData(saveData);
        
        // 오디오 데이터 적용
        ApplyAudioData(saveData);
        
        Debug.Log("[SaveSystem] 저장 데이터 적용 완료");
    }
    
    /// <summary>
    /// 플레이어 데이터 적용
    /// </summary>
    private void ApplyPlayerData(GameSaveData saveData)
    {
        if (Player.Instance == null)
        {
            Debug.LogWarning("[SaveSystem] Player.Instance가 null입니다.");
            return;
        }
        
        Player player = Player.Instance;
        player.characterName = saveData.playerCharacterName;
        player.level = saveData.level;
        player.experience = saveData.experience;
        player.experienceToNextLevel = saveData.experienceToNextLevel;
        player.maxHP = saveData.maxHP;
        player.currentHP = saveData.currentHP;
        player.maxSanity = saveData.maxSanity;
        player.currentSanity = saveData.currentSanity;
        player.strength = saveData.strength;
        player.dexterity = saveData.dexterity;
        player.vitality = saveData.vitality;
        player.willpower = saveData.willpower;
        player.luck = saveData.luck;
        player.currentMadnessState = (MadnessState)saveData.currentMadnessState;
        
        // HP/Sanity 바 업데이트
        player.UpdateHPBar();
        player.UpdateSanityBar();
    }
    
    /// <summary>
    /// 인벤토리 데이터 적용
    /// </summary>
    private void ApplyInventoryData(GameSaveData saveData)
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("[SaveSystem] Inventory.Instance가 null입니다.");
            return;
        }
        
        Inventory inventory = Inventory.Instance;
        
        // 인벤토리 초기화
        inventory.items.Clear();
        inventory.skills.Clear();
        
        // 아이템 로드
        for (int i = 0; i < saveData.itemNames.Count && i < saveData.itemTypes.Count; i++)
        {
            string itemName = saveData.itemNames[i];
            ItemType itemType = (ItemType)saveData.itemTypes[i];
            inventory.AddItemByName(itemName, itemType);
        }
        
        // 스킬 로드 (스킬은 Resources에서 로드해야 함)
        // TODO: 스킬 로드 로직 추가 (필요시)
        
        // 장착된 아이템 복원
        if (!string.IsNullOrEmpty(saveData.equippedWeaponName))
        {
            Item weapon = inventory.items.Find(item => item != null && item.itemName == saveData.equippedWeaponName);
            if (weapon != null)
            {
                inventory.EquipWeapon(weapon);
            }
        }
        
        if (!string.IsNullOrEmpty(saveData.equippedTopName))
        {
            Item top = inventory.items.Find(item => item != null && item.itemName == saveData.equippedTopName);
            if (top != null)
            {
                inventory.EquipTop(top);
            }
        }
        
        if (!string.IsNullOrEmpty(saveData.equippedBottomName))
        {
            Item bottom = inventory.items.Find(item => item != null && item.itemName == saveData.equippedBottomName);
            if (bottom != null)
            {
                inventory.EquipBottom(bottom);
            }
        }
        
        // 스킬 슬롯 복원
        // TODO: 스킬 슬롯 복원 로직 추가 (필요시)
        
        // 소비 아이템 슬롯 복원
        for (int i = 0; i < saveData.consumableItemSlotNames.Count && i < inventory.consumableItemSlots.Count; i++)
        {
            string itemName = saveData.consumableItemSlotNames[i];
            if (!string.IsNullOrEmpty(itemName))
            {
                Item item = inventory.items.Find(it => it != null && it.itemName == itemName);
                if (item != null)
                {
                    inventory.AssignItemToSlot(item, i);
                }
            }
        }
    }
    
    /// <summary>
    /// Yarn 스크립트 데이터 적용
    /// </summary>
    private void ApplyYarnData(GameSaveData saveData)
    {
        // PlayerPrefs에 저장하여 CustomLineView에서 사용
        PlayerPrefs.SetString("CurrentYarnNode", saveData.currentNodeName);
        PlayerPrefs.SetString("SelectedCharacterName", saveData.characterName);
        PlayerPrefs.SetString("YarnScriptName", saveData.yarnScriptName);
        PlayerPrefs.SetString("YarnStartNode", saveData.currentNodeName);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 오디오 데이터 적용
    /// </summary>
    private void ApplyAudioData(GameSaveData saveData)
    {
        if (!string.IsNullOrEmpty(saveData.currentBGMName))
        {
            CustomLineView lineView = FindFirstObjectByType<CustomLineView>();
            if (lineView != null)
            {
                lineView.PlayBGM(saveData.currentBGMName);
            }
        }
    }
    
    /// <summary>
    /// 특정 슬롯에 저장 데이터가 있는지 확인
    /// </summary>
    public bool HasSaveData(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS)
        {
            return false;
        }
        
        string filePath = GetSaveFilePath(slotNumber);
        return File.Exists(filePath);
    }
    
    /// <summary>
    /// 저장 데이터 정보 가져오기 (UI 표시용)
    /// </summary>
    public GameSaveData GetSaveDataInfo(int slotNumber)
    {
        return LoadGame(slotNumber);
    }
    
    /// <summary>
    /// 가장 최근 저장 슬롯 번호 반환
    /// </summary>
    public int GetLatestSaveSlot()
    {
        int latestSlot = 0;
        DateTime latestDate = DateTime.MinValue;
        
        for (int i = 1; i <= MAX_SLOTS; i++)
        {
            if (HasSaveData(i))
            {
                GameSaveData data = LoadGame(i);
                if (data != null && DateTime.TryParse(data.saveDate, out DateTime saveDate))
                {
                    if (saveDate > latestDate)
                    {
                        latestDate = saveDate;
                        latestSlot = i;
                    }
                }
            }
        }
        
        return latestSlot;
    }
    
    /// <summary>
    /// 저장 파일 삭제
    /// </summary>
    public bool DeleteSave(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS)
        {
            return false;
        }
        
        string filePath = GetSaveFilePath(slotNumber);
        
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"[SaveSystem] 저장 파일 삭제 완료: 슬롯 {slotNumber}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] 저장 파일 삭제 실패: {e.Message}");
                return false;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 현재 사용 중인 슬롯 번호 반환
    /// </summary>
    public int GetCurrentSlotNumber()
    {
        return currentSlotNumber;
    }
    
    /// <summary>
    /// 현재 슬롯 번호 설정
    /// </summary>
    public void SetCurrentSlotNumber(int slotNumber)
    {
        if (slotNumber >= 1 && slotNumber <= MAX_SLOTS)
        {
            currentSlotNumber = slotNumber;
        }
    }
}

