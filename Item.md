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

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        Debug.Log($"아이템 추가: {newItem.itemName}");
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
    public GameObject itemSlotPrefab; //아이템 슬롯 프리펩
    public GameObject skillSlotPrefab; //스킬 슬롯 프리펩

    [Header("Slot Buttons")]
    public Button weaponSlot; // 무기 장착 칸
    public Button topSlot; // 상의 장착 칸
    public Button bottomSlot; // 하의 장착 칸
    public List<Button> skillSlots; // 스킬 슬롯 (4개)

    private Item selectedWeapon; // 선택된 무기
    private Item selectedTop; // 선택된 상의
    private Item selectedBottom; // 선택된 하의
    private Skill selectedSkill; // 선택된 스킬
    public Image weaponImage; //무기 이미지
    public Image topImage; //상의 이미지
    public Image bottomImage; //하의 이미지
    private int selectedSkillSlotIndex = -1;

    private Inventory inventory; // Inventory 스크립트 참조
    private Player player; // Player 스크립트 참조

    public Image inventoryCharacterImage; // 인벤토리의 캐릭터 이미지
    public Image battleCharacterImage; // 배틀 창의 캐릭터 이미지

    public static InventoryManager Instance{get; private set;}
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        Skill currentSkill = inventory.skillSlots[slotIndex];

        List<Skill> availableSkills = inventory.GetEquippedWeaponSkills();
        GameObject skillSlot = Instantiate(skillSlotPrefab, uiParent);
        Debug.Log($"스킬 슬롯 {slotIndex} 클릭. {availableSkills.Count}개의 스킬이 사용 가능.");

        OpenEquipmentWindow<Skill>(
            currentSkill,
            availableSkills,
            skill => EquipSkill((Skill)skill),
            UnequipSkill
        );
    }

    public void OpenEquipmentWindow<T>(T currentItem, List<T> items, System.Action<IItemData> onEquip, System.Action onUnequip)
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
        OpenEquipmentWindow(
            selectedWeapon, 
            weaponItems, 
            item => EquipWeapon((Item)item), 
            UnequipWeapon);
    }

    public void OpenTopWindow()
    {
        List<Item> topItems = inventory.GetItemsByType(ItemType.top);
        OpenEquipmentWindow(selectedTop, topItems, item => EquipTop((Item)item), UnequipTop);
    }

    public void OpenBottomWindow()
    {
        List<Item> bottomItems = inventory.GetItemsByType(ItemType.bottom);
        OpenEquipmentWindow(selectedBottom, bottomItems, item => EquipBottom((Item)item), UnequipBottom);
    }

    public void EquipSkill(Skill skill)
    {
        if(selectedSkillSlotIndex == -1)
        {
            Debug.LogError("스킬 슬롯이 선택되지 않았습니다");
            return;
        }
        
        for(int i = 0; i < inventory.skillSlots.Count; i++)
        {
            if(inventory.skillSlots[i] == skill)
            {
                inventory.skillSlots[i] = null;

                Transform iconTransform = skillSlots[i].transform.Find("Icon");
                if(iconTransform != null)
                {
                    Image skillIcon = iconTransform.GetComponent<Image>();
                    if(skillIcon != null)
                    {
                        skillIcon.sprite = null;
                        skillIcon.enabled = false;
                    }
                }
                break;
            }
        }

        inventory.AssignSkillToSlot(skill, selectedSkillSlotIndex);

        Transform newIconTransform = skillSlots[selectedSkillSlotIndex].transform.Find("Icon");
        if (newIconTransform != null)
        {
            Image skillIcon = newIconTransform.GetComponent<Image>();
            if (skillIcon != null)
            {
                skillIcon.sprite = skill.skillIcon;
                skillIcon.enabled = true;
            }
        }
        
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

        Transform iconTransform = skillSlots[selectedSkillSlotIndex].transform.Find("Icon");
        if(iconTransform != null)
        {
            Image skillIcon = iconTransform.GetComponent<Image>();
            if(skillIcon != null)
            {
                skillIcon.sprite = null;
                skillIcon.enabled = false;
            }
        }

        Debug.Log($"스킬 슬롯 {selectedSkillSlotIndex} 해제");
        selectedSkillSlotIndex = -1;
        UpdateEquipmentImages();
        
    }

    public void EquipWeapon(Item weapon)
    {
        selectedWeapon = weapon;
        inventory.EquipWeapon(weapon);
        Debug.Log($"{weapon.GetName()} 장착됨");
        ClearSkillSlots();
        var weaponSkills = inventory.GetEquippedWeaponSkills();
        UpdateSkillWindow(weaponSkills);
        UpdateEquipmentImages();
        
    }

    public void UnequipWeapon()
    {
        selectedWeapon = null;
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

    public void ClearSkillSlots()
    {
        foreach(var skillSlot in skillSlots)
        {
            Transform iconTransform = skillSlot.transform.Find("Icon");
            if(iconTransform == null) continue;

            Image skillIcon = iconTransform.GetComponent<Image>();
            if(skillIcon != null)
            {
                skillIcon.sprite = null;
                skillIcon.enabled = false;
            }
            skillSlot.onClick.RemoveAllListeners();
        }

        for(int i = 0; i < inventory.skillSlots.Count; i++)
        {
            inventory.skillSlots[i] = null;
        }
    }

    public void UpdateSkillWindow(List<Skill> skills)
    {
        Debug.Log($"스킬 창에 {skills.Count}개의 스킬을 표시합니다.");

        foreach (Skill skill in skills)
        {
            if (skill != null)
            {
                Debug.Log($"스킬 창에 표시: {skill.skillName}");
            }
        }
    }

    private void UpdateEquipmentImages()
    {
        Sprite compositeImage = Player.Instance.GetCompositeCharacterImage();

        inventoryCharacterImage.sprite = compositeImage;

        // 무기 슬롯 아이콘 업데이트
        UpdateSlotIconByName(weaponSlot.transform, "Icon", selectedWeapon?.itemIcon);
        if (weaponImage != null)
        {
            weaponImage.sprite = selectedWeapon?.itemSprite;
            weaponImage.enabled = selectedWeapon != null;
            weaponImage.gameObject.SetActive(true);
        }
        // 상의 슬롯 아이콘 업데이트
        UpdateSlotIconByName(topSlot.transform, "Icon", selectedTop?.itemIcon);
        if (topImage != null)
        {
            topImage.sprite = selectedTop?.itemSprite;
            topImage.enabled = selectedTop != null;
            topImage.gameObject.SetActive(true);
        }
        // 하의 슬롯 아이콘 업데이트
        UpdateSlotIconByName(bottomSlot.transform, "Icon", selectedBottom?.itemIcon);
        if (bottomImage != null)
        {
            bottomImage.sprite = selectedBottom?.itemSprite;
            bottomImage.enabled = selectedBottom != null;
            bottomImage.gameObject.SetActive(true);
        }

        if(battleCharacterImage != null)
        {
            battleCharacterImage.sprite = compositeImage;
        }

    }

    private void UpdateSlotIconByName(Transform slotTransform, string iconName, Sprite iconSprite)
    {
        // 슬롯의 하위에서 이름으로 Icon 오브젝트 찾기
        Transform iconTransform = slotTransform.Find(iconName);

        if (iconTransform == null)
        {
            Debug.LogError($"{slotTransform.name} 슬롯에서 {iconName} 오브젝트를 찾을 수 없습니다.");
            return;
        }

        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogError($"{iconName} 오브젝트에 Image 컴포넌트가 없습니다.");
            return;
        }

        // 아이콘 업데이트
        iconImage.sprite = iconSprite;
        iconImage.enabled = iconSprite != null; // 스프라이트가 null일 경우 비활성화
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
    public Transform itemListContent; //슬롯이 생성될 Content
    public GameObject itemSlotPrefab; //슬롯 프리펩
    public GameObject OptionWindow; //옵션창 오브젝트
    public Image currentItemIcon; //선택된 아이템/스킬 아이콘
    public TextMeshProUGUI currentItemName; //선택한 아이템/스킬 이름
    public TextMeshProUGUI currentItemOption; //선택한 아이템/스킬 옵션
    public Button equipButton; //장착 버튼
    public Button unequipButton; //해제 버튼

    private object selectedItem; //현재 선택된 아이템/스킬

    public void Initialize<T>
    ( 
        T currentItem,
        List<T> items,
        System.Action<IItemData> onEquip,
        System.Action onUnequip,
        GameObject slotPrefab
    )
    {
        itemSlotPrefab = slotPrefab;

        foreach(Transform child in itemListContent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"{items.Count}개의 아이템이 존재합니다");

        foreach(T item in items)
        {
            if(item == null)
            {
                Debug.LogError("Null아이템 발견, Skip");
                continue;
            }

            GameObject itemSlot = Instantiate(itemSlotPrefab, itemListContent);
            Button itemButton = itemSlot.GetComponent<Button>();
            Image itemIcon = itemSlot.GetComponentInChildren<Image>();

            var itemData = item as IItemData;
            if(itemData != null)
            {
                itemIcon.sprite = itemData?.GetIcon();


                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(() => 
                {
                    OnSlotClicked(itemData, onEquip, onUnequip);
                });
            }
            else
            {
                Debug.LogWarning("슬롯 생성 중 잘못된 데이터 타입 발견. 무시합니다.");
            }
        }
    }
    
    private void OnSlotClicked(IItemData itemData, System.Action<IItemData> onEquip, System.Action onUnequip)
    {
        Debug.Log("슬롯 클릭: {itemData.GetName()}");
        ShowOptionWindow(itemData, onEquip, onUnequip);
    }

    public void ShowOptionWindow(IItemData itemData, System.Action<IItemData> onEquip, System.Action onUnequip)
    {
        if(itemData == null) 
        {
            Debug.LogError("옵션 윈도우가 연결되지 않았습니다");
            return;
        }

        OptionWindow.SetActive(true);
        currentItemIcon.sprite = itemData.GetIcon();
        currentItemName.text = itemData.GetName();
        currentItemOption.text = itemData.GetOption();

        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() =>
        {
            Debug.Log($"{itemData.GetName()} 장착!");
            HandleEquip(itemData); //타입에 따라 장착 처리
            OptionWindow.SetActive(false);
            DisableItemSkillWindow();
        });

        // 해제 버튼
        unequipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.AddListener(() =>
        {
            Debug.Log($"{itemData.GetName()} 해제!");
            onUnequip?.Invoke(); // InventoryManager에서 전달된 해제 메서드 호출
            OptionWindow.SetActive(false);
        });
        
    }

    public void DisableItemSkillWindow()
    {
        Transform uiParent = transform.parent;
        if(uiParent == null)
        {
            Debug.LogError("UI 부모 오브젝트를 찾을 수 없습니다");
            return;
        }

        Transform itemSkillWindow = uiParent.Find("Item_Skill Window(Clone)");
        if(itemSkillWindow != null)
        {
            itemSkillWindow.gameObject.SetActive(false);
            Destroy(itemSkillWindow.gameObject);
        }
    }

    private void HandleEquip(IItemData itemData)
    {
        if(itemData is Item item)
        {
            switch(item.itemType)
            {
                case ItemType.weapon:
                    InventoryManager.Instance.EquipWeapon(item);
                    break;

                case ItemType.top:
                    InventoryManager.Instance.EquipTop(item);
                    break;

                case ItemType.bottom:
                    InventoryManager.Instance.EquipBottom(item);
                    break;

                default:
                    Debug.LogError($"지원하지 않는 아이템 타입: {item.itemType}");
                    break;
            }
        }
        else if(itemData is Skill skill)
        {
            InventoryManager.Instance.EquipSkill(skill);
        }
        else
        {
            Debug.LogError("알 수 없는 데이터 타입입니다");
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
