Inventory.cs
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Skill> equippedSkills = new List<Skill>(); //장착된 스킬 목록
    public Item equippedWeapon; //장착된 무기
    public List<Skill> skillSlots = new List<Skill>(4); //전투에 사용할 스킬 슬롯
    public List<Item> equippedWeapons = new List<Item>(); //플레이어가 얻은 무기 목록

    private void Start()
    {
        skillSlots = new List<Skill>{null, null, null, null};
    }

    public void EquipWeapon(Item weapon) //선택한 무기를 장착
    {
        equippedWeapon = weapon;
        equippedSkills.Clear();
        equippedSkills.AddRange(weapon.assignedSkills);
    }

    public void UnequipWeapon(Item weapon) //선택한 무기를 해제
    {
        if(equippedWeapon = weapon)
        {
            equippedWeapon = null;
        }
    }

    public bool IsEquipped(Item weapon) //무기를 장착중인지 확인
    {
        return equippedWeapon == weapon;
    }

    public void AddWeapon(Item weapon) //무기를 획득 시 무기 목록에 추가
    {
        if(!equippedWeapons.Contains(weapon))
        {
            equippedWeapons.Add(weapon);
        }
    }

    public List<Skill> GetEquippedWeaponSkills()
    {
        return equippedWeapon != null ? equippedWeapon.assignedSkills : new List<Skill>();
    }

    public void AssignSkillToSlot(Skill skill, int slotIndex)
    {
        if(slotIndex >= 0 && slotIndex < skillSlots.Count)
        {
            skillSlots[slotIndex] = skill;
        }
    }

    public List<Skill> GetBattleSkills()
    {
        return skillSlots;
    }
}

~~~


Inventory Manager.cs
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject weaponItemWindow; //무기 아이템 창
    public GameObject weaponWindow; //무기 설명창
    public Text weaponNameText; //무기 이름
    public Text weaponOptionText;//무기 설명 
    public Button equipButton;// 장착/해제 버튼

    [Header("Equipment Slots")]
    public Button weaponSlot; //무기 잗착 칸
    public Button topSlot; //상의 장착 칸
    public Button bottomSlot; //하의 장착 칸
    public Transform weaponContent;
    public GameObject weaponItemSlotPrefabs;
    private Item selectedWeapon;

    [Header("Skill Slots")]
    public List<Button> skillSlots; //스킬 지정 칸(4개)

    private Inventory inventory; //Inventory 스크립트 참조

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>(); //Inventory 스크립트 참조

        //스킬 슬롯 클릭 시 무기에 할당된 스킬 목록 열기
        foreach(Button skillSlot in skillSlots)
        {
            skillSlot.onClick.AddListener(() => OpenSkillSelection(skillSlot));
        }

        equipButton.onClick.AddListener(ToggleEquipWeapon);
    }

    private void ToggleEquipWeapon()
    {
        if(selectedWeapon != null)
        {
            if(inventory.IsEquipped(selectedWeapon))
            {
                inventory.UnequipWeapon(selectedWeapon);
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "장착";
            }
            else
            {
                inventory.EquipWeapon(selectedWeapon);
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "해제";
            }
        }
    }

    private void OpenSkillSelection(Button skillSlot)
    {
        //무기에 할당된 스킬을 보여주고 플레이어가 선택한 스킬을 지정
        List<Skill> availableSkills = inventory.GetEquippedWeaponSkills();
        int skillIndex = skillSlots.IndexOf(skillSlot);

        foreach(Skill skill in availableSkills)
        {
            skillSlot.GetComponentInChildren<Text>().text = skill.skillName;
            inventory.AssignSkillToSlot(skill, skillSlots.IndexOf(skillSlot));

            Transform iconTransform = skillSlot.transform.Find("Icon");
            if(iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                iconImage.sprite = skill.skillIcon;
                iconImage.enabled = skill.skillIcon != null;
            }
        }
    }

    private void WeaponWindow(Item weapon)
    {
        selectedWeapon = weapon;
        weaponNameText.text = weapon.itemName;
        weaponOptionText.text = weapon.itemOption;

        equipButton.GetComponentInChildren<TextMeshProUGUI>().text = inventory.IsEquipped(weapon) ? "장착" : "해제";
    }

    private void PopulateWeaponItems()
    {
        foreach(Item weapon in inventory.equippedWeapons)
        {
            GameObject slot = Instantiate(weaponItemSlotPrefabs, weaponContent);
            slot.GetComponentInChildren<Image>().sprite = weapon.itemIcon;
            Button button = slot.GetComponent<Button>();
            button.onClick.AddListener(() => WeaponWindow(weapon));
        } 
    }
}
~~~

![pxArt](https://github.com/user-attachments/assets/ec3dca95-1124-4f0a-bd67-741802c3529a)
