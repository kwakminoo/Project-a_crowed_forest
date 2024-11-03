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

    private void Start()
    {
        skillSlots = new List<Skill>{null, null, null, null};
    }

    public void EquipWeapon(Item weapon)
    {
        equippedWeapon = weapon;
        equippedSkills.Clear();
        equippedSkills.AddRange(weapon.assignedSkills);
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

public class InventoryManager : MonoBehaviour
{
    [Header("Equipment Slots")]
    public Button weaponSlot; //무기 잗착 칸
    public Button topSlot; //상의 장착 칸
    public Button bottomSlot; //하의 장착 칸

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
}
~~~
