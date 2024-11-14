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
        Debug.Log(weapon.itemName + "을 장착했습니다. 할당된 스킬 수:" + equippedSkills.Count);
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

    public void AddWeapon(Item weapon) //무기를 획득 시 무기 목록에 추가
    {
        if(!equippedWeapons.Contains(weapon))
        {
            equippedWeapons.Add(weapon);
            Debug.Log(weapon.itemName + "을 획득했습니다");
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
            Debug.Log((skill != null ? skill.skillName : "스킬 없슴") + "이(가) 슬롯" + slotIndex + "할당됐습니다");
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
    public GameObject inventoryWindow;

    [Header("Weapon Slots")]
    private Item selectedWeapon; //선택한 무기
    public Button weaponSlot; //무기 잗착 칸
    public Image weaponSlotIcon; //무기장창칸에 들어갈 무기 아이콘
    public Transform weaponContent; //무기 아이템을 추가할 Content
    public GameObject weaponItemSlotPrefabs; //무기아이템 슬롯 프리펩
    public GameObject weaponItemWindow; //무기 아이템 창
    public GameObject weaponWindow; //무기 설명창
    public TextMeshProUGUI weaponNameText; //무기 이름
    public TextMeshProUGUI weaponOptionText;//무기 설명
    public Image weaponIcon;//무기 아이콘 
    public Button weaponEquipButton; //무기장착버튼
    public Button weaponUnequipButton; //무기해제 버튼

    [Header("Top Slot")]
    public Button topSlot; //상의 장착 칸

    [Header("Bottom Slot")]
    public Button bottomSlot; //하의 장착 칸

    [Header("Skill Slots")]
    private Skill selectedSkill; //선택한 스킬
    private Button selectSkillSlot; //선택한 스킬 슬롯
    public List<Button> skillSlots; //스킬 지정 칸(4개)
    public List<Image> skillSlotIcon;//스킬슬롯에 들어갈 스킬 아이콘
    public Transform skillContent; //스킬 아이템을 푸가할 Content
    public GameObject skillSlotPrefabs; //스킬 슬롯 프리펩
    public GameObject skillSelectWindow;//스킬들이 모여있는 창
    public GameObject skillWindow;//스킬 설명창
    public Image skillIconImage;//스킬 설명창에 들어갈 아이콘이미지
    public TextMeshProUGUI skillNameText;//스킬 이름
    public TextMeshProUGUI skillOptionText;//스킬 설명
    public Button skillEquipButton; //스킬장착버튼
    public Button skillUnequipButton; //스킬해제 버튼

    private Inventory inventory; //Inventory 스크립트 참조
    public void OpenInventory()
    {
        inventoryWindow.SetActive(true);
        PopulateWeaponItems();
    }

    public void Start()
    {
        inventory = FindObjectOfType<Inventory>(); //Inventory 스크립트 참조

        //스킬 슬롯 클릭 시 무기에 할당된 스킬 목록 열기
        foreach(Button skillSlot in skillSlots)
        {
            skillSlot.onClick.AddListener(() => OpenSkillSelection(skillSlot));
        }

        weaponEquipButton.onClick.RemoveAllListeners();
        weaponEquipButton.onClick.AddListener(EquipWeapon);
        weaponUnequipButton.onClick.RemoveAllListeners();
        weaponUnequipButton.onClick.AddListener(UnequipWeapon);

        skillEquipButton.onClick.RemoveAllListeners();
        skillEquipButton.onClick.AddListener(() => EquipSkill());
        skillUnequipButton.onClick.RemoveAllListeners();
        skillUnequipButton.onClick.AddListener(() => UnquipSkill());
    }

    public void EquipWeapon()
    {
        if(selectedWeapon != null && !inventory.IsEquipped(selectedWeapon))
        {
            inventory.IsEquipped(selectedWeapon);
            inventory.EquipWeapon(selectedWeapon);
            Debug.Log(selectedWeapon.itemName + "장착됨");

            if(weaponSlotIcon != null)
            {
                weaponSlotIcon.sprite = selectedWeapon.itemIcon;
                weaponSlotIcon.enabled = true;
            }
        }
        
        UpdateSkillSlot(selectedWeapon.assignedSkills);
        weaponWindow.SetActive(false);
        weaponItemWindow.SetActive(false);
    }

    public void UnequipWeapon()
    {
        if(selectedWeapon != null && !inventory.IsEquipped(selectedWeapon))
        {
            inventory.UnequipWeapon(selectedWeapon);
            weaponSlotIcon.sprite = null;
            weaponSlotIcon.enabled = false;
            CloseSkillSlot();
            Debug.Log(selectedWeapon.itemName + "해제됨");
        }
    }

    public void CloseSkillSlot()
    {
        foreach(Image skillIcon in skillSlotIcon)
        {
            if(skillIcon != null)
            {
                skillIcon.sprite = null;
                skillIcon.enabled = false;
            }
        }
    }

    public void UpdateSkillSlot(List<Skill> skills)
    {
        for(int i = 0; i < skillSlotIcon.Count; i++)
        {
            if(i <skills.Count)
            {
                skillSlotIcon[i].sprite = skills[i].skillIcon;
                skillSlotIcon[i].enabled = true;
            }
            else
            {
                skillSlotIcon[i].sprite = null;
                skillSlotIcon[i].enabled = false;
            }
        }
    }

    public void OpenSkillSelection(Button skillSlot)
    {
        selectSkillSlot = skillSlot;
        //무기에 할당된 스킬을 보여주고 플레이어가 선택한 스킬을 지정
        List<Skill> availableSkills = inventory.GetEquippedWeaponSkills();
        if(availableSkills.Count == 0)
        {
            Debug.LogError("무기에 할당된 스킳이 없습니다");
            return;
        }

        foreach(Transform child in skillContent)
        {
            Destroy(child.gameObject);
        }

        foreach(Skill skill in availableSkills)
        {
            GameObject skillSlotObject = Instantiate(skillSlotPrefabs, skillContent);
            skillSlotObject.GetComponentInChildren<Image>().sprite = skill.skillIcon;
            Button skillButton = skillSlotObject.GetComponent<Button>();
            skillButton.onClick.AddListener(() => SkillWindow(skill));

        }
    }

    public void SkillWindow(Skill skill)
    {
        selectedSkill = skill;
        skillWindow.SetActive(true);
        skillNameText.text = skill.skillName;
        skillOptionText.text = skill.skillOption;
        skillIconImage.sprite = skill.skillIcon;
    }

    public void EquipSkill()
    {
        int skillIndex = skillSlots.IndexOf(selectSkillSlot);
        if(skillIndex >= 0 && skillIndex < skillSlotIcon.Count)
        {
            inventory.AssignSkillToSlot(selectedSkill, skillIndex);

            skillSlotIcon[skillIndex].sprite = selectedSkill.skillIcon;
            skillSlotIcon[skillIndex].enabled = true;

            Debug.Log(selectedSkill.skillName + "이(가)" + skillIndex + "슬롯에 장착됐습니다");
        } 
        else
        {
            Debug.Log(selectedSkill.skillName + "이(가)" + skillIndex + "슬롯에 장착을 실패했습니다");
        } 

        skillWindow.SetActive(false);
        skillSelectWindow.SetActive(false);      
    }

    public void UnquipSkill()
    {
        int skillIndex = skillSlots.IndexOf(selectSkillSlot);
        if(skillIndex >= 0 && skillIndex < skillSlotIcon.Count)
        {
            inventory.AssignSkillToSlot(null, skillIndex);

            skillSlotIcon[skillIndex].sprite = null;
            skillSlotIcon[skillIndex].enabled = false;

            Debug.Log(selectedSkill.skillName + "이(가)" + skillIndex + "슬롯에 해제됐습니다");
        }

        skillWindow.SetActive(false);
    }

    public void WeaponWindow(Item weapon)
    {
        selectedWeapon = weapon;
        weaponWindow.SetActive(true);
        weaponNameText.text = weapon.itemName;
        weaponOptionText.text = weapon.itemOption;
        weaponIcon.sprite = weapon.itemIcon;
    }

    public void PopulateWeaponItems()
    {
        foreach(Transform child in weaponContent)
        {
            Destroy(child.gameObject);
        }

        foreach(Item weapon in inventory.equippedWeapons)
        {
            GameObject slot = Instantiate(weaponItemSlotPrefabs, weaponContent);
            slot.GetComponentInChildren<Image>().sprite = weapon.itemIcon;
            Button button = slot.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => WeaponWindow(weapon));


        } 
    }
}
~~~

![pxArt](https://github.com/user-attachments/assets/ec3dca95-1124-4f0a-bd67-741802c3529a)
