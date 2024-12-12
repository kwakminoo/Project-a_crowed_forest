Item.cs
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    weapon, top, bottom, key
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item System/Item")]
public class Item : ScriptableObject, IItemData
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public string itemOption;
    public Sprite itemSprite;
    public List<Skill> assignedSkills = new List<Skill>();

    public Sprite GetIcon() => itemIcon;
    public string GetName() => itemName;
    public string GetOption() => itemOption;
}
~~~

Inventory.cs
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; } //싱글톤 페턴
    private List<Item> items = new List<Item>();
    private List<Skill> skills = new List<Skill>();
    public List<Skill> equippedSkills = new List<Skill>(); //장착된 스킬 목록
    public Item equippedWeapon; //장착된 무기
    public Item equippedTop; //장착된 상의
    public Item equippedBottom; //장착된 하의
    public List<Skill> skillSlots = new List<Skill>(); //전투에 사용할 스킬 슬롯
    public List<Item> equippedWeapons = new List<Item>(); //플레이어가 얻은 무기 목록
    public event Action OnInventoryUpdated; //인벤토리 데이터 변경시 호출

    private void Start()
    {
        skillSlots = new List<Skill>{null, null, null, null};
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //씬 전환에도 유지하도록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RaiseInventoryUpdatedEnent()
    {
        OnInventoryUpdated?.Invoke();
    }
    
    public List<Item> GetItemsByType(ItemType type)
    {
        return items.FindAll(item => item.itemType == type);
    }

    public List<Skill> GetAvailableSkills()
    {
        return skills;
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

    public void AddWeapon(Item weapon) //무기를 획득 시 무기 목록에 추가
    {
        if(!equippedWeapons.Contains(weapon))
        {
            equippedWeapons.Add(weapon);
            Debug.Log(weapon.itemName + "을 획득했습니다");
        }
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
        return equippedWeapon != null ? equippedWeapon.assignedSkills : new List<Skill>();
    }

    public bool IsSkillAlreadyEquipped(Skill skill)
    {
        return skillSlots.Contains(skill);
    }

    public void AssignSkillToSlot(Skill skill, int slotIndex)
    {
        Debug.Log("AssignSkillToSlot 호출됨, slotIndex:" + slotIndex);
        if(slotIndex >= 0 && slotIndex < skillSlots.Count)
        {
            skillSlots[slotIndex] = skill;
            OnInventoryUpdated?.Invoke(); //데이터 변경 이벤트 발생
            Debug.Log((skill != null ? skill.skillName : "스킬 없슴") + "이(가) 슬롯" + slotIndex + "할당됐습니다");

            for(int i = 0; i < skillSlots.Count; i ++)
            {
                Debug.Log("슬롯" + i + (skillSlots[i] != null ? skillSlots[i].skillName : "비어있습니다"));
            }

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
    public GameObject equipmentWindowPrefab; // 장비창 프리펩
    public Transform uiParent; // 장비 창의 부모 UI 오브젝트
    public GameObject itemSlotPrefab; //슬롯 프리펩

    [Header("Slot Buttons")]
    public Button weaponSlot; // 무기 장착 칸
    public Button topSlot; // 상의 장착 칸
    public Button bottomSlot; // 하의 장착 칸
    public List<Button> skillSlots; // 스킬 슬롯 (4개)

    private Item selectedWeapon; // 선택된 무기
    private Item selectedTop; // 선택된 상의
    private Item selectedBottom; // 선택된 하의
    private Skill selectedSkill; // 선택된 스킬
    public Image weaponImage;
    public Image topImage;
    public Image bottomImage;
    private int selectedSkillSlotIndex = -1;

    private Inventory inventory; // Inventory 스크립트 참조
    private Player player; // Player 스크립트 참조

    public Image inventoryCharacterImage; // 인벤토리의 캐릭터 이미지
    public Image battleCharacterImage; // 배틀 창의 캐릭터 이미지

    public void OpenInventory()
    {
        Debug.Log("Inventory가 열렸는습니다");
        inventoryWindow.SetActive(true);
    }

    private void Log(string message)
    {
        #if UNITY_EDITOR
        Debug.Log(message);
        #endif
    }
     
    public void Start()
    {
        inventory = Inventory.Instance;
        player = Player.Instance;
        Player.Instance.OnCharacterUpdated += UpdateEquipmentImages;
        UpdateEquipmentImages();

        if(player == null)
        {
            Debug.LogError("Player.Instance가 초기화되지 않았습니다.");
            return;
        }

        weaponSlot.onClick.RemoveAllListeners();
        weaponSlot.onClick.AddListener(OpenWeaponWindow);
        topSlot.onClick.RemoveAllListeners();
        topSlot.onClick.AddListener(OpenTopWindow);
        bottomSlot.onClick.RemoveAllListeners();
        bottomSlot.onClick.AddListener(OpenBottomWindow);
        
        for(int i = 0; i < skillSlots.Count; i++)
        {
            int index = i;
            skillSlots[index].onClick.RemoveAllListeners();
            skillSlots[index].onClick.AddListener(() => OpenSkillWindow(index));
        }
        
    }

    public void OpenSkillWindow(int slotIndex)
    {
        selectedSkillSlotIndex = slotIndex;

        OpenEquipmentWindow<Skill>(
            selectedSkill,
            inventory.GetAvailableSkills(),
            EquipSkill,
            UnequipSkill
        );
    }

    public void OpenEquipmentWindow<T>(T currentItem, List<T> items, System.Action<T> onEquip, System.Action onUnequip)
    {
        foreach(Transform child in uiParent)
        {
            Destroy(child.gameObject);
        }

        GameObject windowInstance = Instantiate(equipmentWindowPrefab, uiParent);
        EquipmentWindow equipmentWindow = windowInstance.GetComponent<EquipmentWindow>();
        
        if(equipmentWindow != null)
        {
            equipmentWindow.Initialize(
                currentItem,
                items,
                onEquip,
                onUnequip,
                itemSlotPrefab
            );
        }

    }

    public void OpenWeaponWindow()
    {
        var weapons = inventory.GetItemsByType(ItemType.weapon);
        Debug.Log($"무기 개수: {weapons.Count}");
        List<Item> weaponItems = inventory.GetItemsByType(ItemType.weapon);
        OpenEquipmentWindow(selectedWeapon, weaponItems, EquipWeapon, UnequipWeapon);
    }

    public void OpenTopWindow()
    {
        List<Item> topItems = inventory.GetItemsByType(ItemType.top);
        OpenEquipmentWindow(selectedTop, topItems, EquipTop, UnequipTop);
    }

    public void OpenBottomWindow()
    {
        List<Item> bottomItems = inventory.GetItemsByType(ItemType.bottom);
        OpenEquipmentWindow(selectedBottom, bottomItems, EquipBottom, UnequipBottom);
    }

    public void EquipSkill(Skill skill)
    {
        if(selectedSkillSlotIndex == -1)
        {
            Debug.LogError("스킬 슬롯이 선택되지 않았습니다");
            return;
        }

        selectedSkill = skill;
        inventory.AssignSkillToSlot(skill, selectedSkillSlotIndex);
        skillSlots[selectedSkillSlotIndex].GetComponent<Image>().sprite = skill.skillIcon;
        Debug.Log($"{skill.GetName()} 장착됨");
        UpdateEquipmentImages();
    }

    public void UnequipSkill()
    {
         if(selectedSkillSlotIndex == -1)
        {
            Debug.LogError("스킬 슬롯이 선택되지 않았습니다");
            return;
        }

        selectedSkill = null;
        inventory.AssignSkillToSlot(null, selectedSkillSlotIndex);
        skillSlots[selectedSkillSlotIndex].GetComponent<Image>().sprite = null;
        Debug.Log($"스킬 해제");
        UpdateEquipmentImages();
        
    }

    public void EquipWeapon(Item weapon)
    {
        selectedWeapon = weapon;
        inventory.EquipWeapon(weapon);
        Debug.Log($"{weapon.GetName()} 장착됨");
        UpdateEquipmentImages();
    }

    public void UnequipWeapon()
    {
        selectedTop = null;
        inventory.UnequipWeapon(null);
        Debug.Log("무기 해제");
        UpdateEquipmentImages();
    }

    public void EquipTop(Item top)
    {
        selectedTop = top;
        inventory.EquipTop(top);
        Debug.Log($"{top.GetName()} 장착됨");
        UpdateEquipmentImages();
    }

    public void UnequipTop()
    {
        selectedTop = null;
        inventory.EquipTop(selectedTop);
        Debug.Log($"상의 해제");
        UpdateEquipmentImages();
    }

    public void EquipBottom(Item bottom)
    {
        selectedBottom = bottom;
        inventory.EquipBottom(bottom);
        Debug.Log($"{bottom.GetName()} 장착됨");
        UpdateEquipmentImages();
    }

    public void UnequipBottom()
    {
        selectedBottom = null;
        inventory.EquipBottom(selectedBottom);
        Debug.Log($"하의 해제");
        UpdateEquipmentImages();
    }

    private void UpdateEquipmentImages()
    {
        Sprite compositeImage = Player.Instance.GetCompositeCharacterImage();

        inventoryCharacterImage.sprite = compositeImage;

        if (battleCharacterImage != null)
        {
            battleCharacterImage.sprite = compositeImage;
        }

        // Null 체크 추가
        if (weaponImage != null)
        {
            weaponImage.sprite = selectedWeapon?.itemSprite;
            weaponImage.enabled = selectedWeapon != null;
        }

        if (topImage != null)
        {
            topImage.sprite = selectedTop?.itemSprite;
            topImage.enabled = selectedTop != null;
        }

        if (bottomImage != null)
        {
            bottomImage.sprite = selectedBottom?.itemSprite;
            bottomImage.enabled = selectedBottom != null;
        }

        if(battleCharacterImage != null)
        {
            battleCharacterImage.sprite = compositeImage;
        }

        /*weaponSlot.GetComponent<Image>().sprite = selectedWeapon?.itemIcon;
        topSlot.GetComponent<Image>().sprite = selectedTop?.itemIcon;
        bottomSlot.GetComponent<Image>().sprite = selectedBottom.itemIcon;*/
    }

    public void ApplyChangesToPlayer()
    {
        Player.Instance.UpdateCharacterState(selectedWeapon, selectedTop, selectedBottom);
    }

    public void UpdatePreviewCharacterImages()
    {
        Sprite updateSprite = GenerateCompositeSprite(
            Player.Instance.baseCharacterSprite,
            selectedWeapon?.itemSprite,
            selectedTop?.itemSprite,
            selectedBottom?.itemSprite
        );

        inventoryCharacterImage.sprite = updateSprite;
    }

    private Sprite GenerateCompositeSprite(Sprite baseSprite, Sprite weaponSprite, Sprite topSprite, Sprite bottomSprite)
    {
        return baseSprite;
    }
}
~~~

EquipmentWindow
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Search;

public class EquipmentWindow : MonoBehaviour
{
    public Transform itemListContent;
    public GameObject itemSlotPrefab;
    public Image currnetitemIcon;
    public TextMeshProUGUI currnetItemName;
    public TextMeshProUGUI currnetItemOption;
    public Button equipButton;
    public Button unequipButton;

    public void Initialize<T>(
        T currentItem,
        List<T> items,
        System.Action<T> onEquip,
        System.Action onUnequip
    )
    {
        if(currentItem != null)
        {
            var itemData = currentItem as IItemData;
            currnetitemIcon.sprite = itemData?.GetIcon();
            currnetItemName.text = itemData?.GetName() ?? "창착 없음";
            currnetItemOption.text = itemData?.GetOption() ?? "아이템을 선택하세요"; 
            unequipButton.onClick.AddListener(() => onUnequip?.Invoke());
        }
        else
        {
            currnetitemIcon.sprite = null;
            currnetItemName.text = "장착 없음";
            currnetItemOption.text = "아이템을 선택하세요";
            unequipButton.onClick.RemoveAllListeners();
        }

        foreach(Transform child in itemListContent)
        {
            Destroy(child.gameObject);
        }

        foreach(T item in items)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, itemListContent);
            Button itemButton = itemSlot.GetComponent<Button>();
            Image itemIcon = itemSlot.GetComponentInChildren<Image>();
            TextMeshProUGUI itemName = itemSlot.GetComponentInChildren<TextMeshProUGUI>();

            var itemData = item as IItemData;
            itemIcon.sprite = itemData?.GetIcon();
            itemName.text = itemData?.GetName();

            itemButton.onClick.AddListener(() => onEquip?.Invoke(item));
        }
        
    }
}

public interface IItemData
{
    Sprite GetIcon();
    string GetName();
    string GetOption();
}
~~~

![pxArt](https://github.com/user-attachments/assets/ec3dca95-1124-4f0a-bd67-741802c3529a)
