using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; } //싱글톤 페턴
    public List<Item> items = new List<Item>(); //모든 아이템을 저장
    public List<Skill> skills = new List<Skill>(); //모든 스킬을 저장
    public List<Skill> equippedSkills = new List<Skill>(); //장착된 스킬 목록
    public Item equippedWeapon; //장착된 무기
    public Item equippedTop; //장착된 상의
    public Item equippedBottom; //장착된 하의
    public List<Skill> skillSlots = new List<Skill>(); //전투에 사용할 스킬 슬롯
    public List<Item> consumableItemSlots = new List<Item>(new Item[4]); // 전투에 사용할 아이템 슬롯
    public event Action OnInventoryUpdated; //인벤토리 데이터 변경시 호출

    private void Start()
    {
        if (skillSlots.Count != 4)
        {
            Debug.LogWarning($"⚠ skillSlots 개수 이상: 현재 {skillSlots.Count}개 -> 4개로 조정");
            while (skillSlots.Count > 4)
            {
                skillSlots.RemoveAt(skillSlots.Count - 1);  // 4개 초과 슬롯 삭제
            }
        }
        if (consumableItemSlots.Count != 4)
        {
            Debug.LogWarning("⚠ consumableItemSlots 개수 이상: 4개로 맞춤");
            while (consumableItemSlots.Count > 4)
                consumableItemSlots.RemoveAt(consumableItemSlots.Count - 1);
            while (consumableItemSlots.Count < 4)
                consumableItemSlots.Add(null);
        }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad는 "루트 GameObject"에만 적용 가능
            var rootGo = transform.root != null ? transform.root.gameObject : gameObject;
            DontDestroyOnLoad(rootGo); //씬 전환에도 유지하도록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItemByName(string itemName, ItemType itemType)
    {
        string folder = GetFolderByItemType(itemType);
        string itemPath = $"Inventory/{folder}/{itemName}";  // ✅ 새로운 폴더 경로 반영

        Item foundItem = Resources.Load<Item>(itemPath);

        if (foundItem != null)
        {
            items.Add(foundItem);
            Debug.Log($"🎁 아이템 획득: {foundItem.itemName} (경로: {itemPath})");
            RaiseInventoryUpdatedEnent();
        }
        else
        {
            Debug.LogError($"❌ '{itemName}' 아이템을 찾을 수 없습니다. (경로: {itemPath})");
        }
    }

    // ✅ 아이템 타입에 따라 올바른 폴더 경로 반환
    private string GetFolderByItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.weapon: return "Weapon";
            case ItemType.top: return "Top";
            case ItemType.bottom: return "Bottom";
            case ItemType.Consumable: return "Item";
            case ItemType.key: return "Item";
            default: return "Item";
        }
    }

    public void RaiseInventoryUpdatedEnent()
    {
        OnInventoryUpdated?.Invoke();
    }
    
    public List<Item> GetItemsByType(ItemType type)
    {
        return items.Where(item => item.itemType == type).ToList();
    }

    public List<Skill> GetAvailableSkills()
    {
        return skills;
    }

    public void AssignItemToSlot(Item item, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < consumableItemSlots.Count)
        {
            consumableItemSlots[slotIndex] = item;
            OnInventoryUpdated?.Invoke();
            Debug.Log($"{item?.itemName ?? "없음"} → 아이템 슬롯 {slotIndex}번에 할당됨");
        }
        else
        {
            Debug.LogError($"잘못된 슬롯 인덱스: {slotIndex}");
        }
    }

    public void EquipWeapon(Item weapon) //선택한 무기를 장착
    {
        equippedWeapon = weapon;
        RaiseInventoryUpdatedEnent();
        equippedSkills.Clear();
        equippedSkills.AddRange(weapon.assignedSkills);
        Debug.Log(weapon.itemName + "을 장착했습니다. 할당된 스킬 수:" + equippedSkills.Count);
    }

    public void EquipTop(Item top) //선택한 무기를 장착
    {
        equippedTop = top;
        RaiseInventoryUpdatedEnent();
        equippedSkills.Clear();
        equippedSkills.AddRange(top.assignedSkills);
        Debug.Log(top.itemName + "을 장착했습니다. 할당된 스킬 수:" + equippedSkills.Count);
    }

    public void UnequipTop()
    {

    }

    public void EquipBottom(Item bottom) //선택한 무기를 장착
    {
        equippedBottom = bottom;
        RaiseInventoryUpdatedEnent();
        equippedSkills.Clear();
        equippedSkills.AddRange(bottom.assignedSkills);
        Debug.Log(bottom.itemName + "을 장착했습니다. 할당된 스킬 수:" + equippedSkills.Count);
    }

    public void UnequipBottom()
    {
        
    }
    public void UnequipWeapon(Item weapon) //선택한 무기를 해제
    {
        if(equippedWeapon == weapon)
        {
            equippedWeapon = null;
        }
    }

    public bool IsEquipped(Item weapon) //무기를 장착중인지 확인
    {
        return equippedWeapon == weapon;
    }

    public void ClearSkillSlots()
    {
        for(int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i] = null;
        }
        Debug.Log("스킬 슬롯 초기화");
    }

    public List<Skill> GetEquippedWeaponSkills()
    {
        if(equippedWeapon == null)
        {
            Debug.LogError("장착된 무기가 없습니다");
            return new List<Skill>();
        }

        if(equippedWeapon.assignedSkills == null || equippedWeapon.assignedSkills.Count == 0)
        {
            Debug.LogError($"{equippedWeapon.itemName}에 할당된 스킬이 없습니다 ");
            return new List<Skill>();
        }

        return equippedWeapon.assignedSkills;
    }

    public bool IsSkillAlreadyEquipped(Skill skill)
    {
        return skillSlots.Contains(skill);
    }

    public void AssignSkillToSlot(Skill skill, int slotIndex)
    {
        if(slotIndex >= 0 && slotIndex < skillSlots.Count)
        {
            skillSlots[slotIndex] = skill;
            OnInventoryUpdated?.Invoke(); //데이터 변경 이벤트 발생
            Debug.Log((skill != null ? skill.skillName : "스킬 없슴") + "이(가) 슬롯" + slotIndex + "할당됐습니다");
            RaiseInventoryUpdatedEnent();
        }
        else
        {
            Debug.LogError("스킬 할당에 실패했습니다. 잘못된 스킬 인덱스:" + slotIndex);
        }
    }

    public List<Skill> GetBattleSkills()
    {
        return skillSlots;
    }

    public bool HasItem(string itemName)
    {
        return items.Any(item => item.itemName == itemName);
    }

}
