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
    public GameObject equipmentWindowPrefab; // 창 프리펩
    public Transform uiParent; // 장비 창의 부모 UI 오브젝트
    public GameObject itemSlotPrefab; //아이템 슬롯 프리펩
    public GameObject skillSlotPrefab; //스킬 슬롯 프리펩

    [Header("Slot Buttons")]
    public Button inventorySlot; //아이템 목록창
    public Button weaponSlot; // 무기 장착 칸
    public Button topSlot; // 상의 장착 칸
    public Button bottomSlot; // 하의 장착 칸
    public List<Button> skillSlots; // 스킬 슬롯 (4개)
    private int selectedSkillSlotIndex = -1; // 현재 선택된 스킬 슬롯 인덱스
    public List<Button> itemSlots; // 4개의 소비 아이템 슬롯 버튼
    private int selectedItemSlotIndex = -1; // 현재 선택된 아이템 슬롯 인덱스


    private Item selectedItem; //선택된 아이템
    private Item selectedWeapon; // 선택된 무기
    private Item selectedTop; // 선택된 상의
    private Item selectedBottom; // 선택된 하의
    // private Skill selectedSkill; // 선택된 스킬 (현재 사용되지 않음)
    public Image weaponImage; //무기 이미지
    public Image topImage; //상의 이미지
    public Image bottomImage; //하의 이미지
    

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
        List<Item> allItems = inventory.items;  // 모든 아이템 그대로 가져옴
        UpdateEquipmentImages();
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

        // 💡 인벤토리에서 저장된 무기 데이터를 가져와서 적용
        selectedWeapon = inventory.equippedWeapon;
        selectedTop = inventory.equippedTop;
        selectedBottom = inventory.equippedBottom;
        //selectedItem = inventory.equippedItem;

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
        inventorySlot.onClick.RemoveAllListeners();
        inventorySlot.onClick.AddListener(OpenItemWindow);
        
        for(int i = 0; i < skillSlots.Count; i++)
        {
            int index = i;
            skillSlots[index].onClick.RemoveAllListeners();
            skillSlots[index].onClick.AddListener(() => OpenSkillWindow(index));
        }
        for (int i = 0; i < itemSlots.Count; i++)
        {
            int index = i;
            itemSlots[index].onClick.RemoveAllListeners();
            itemSlots[index].onClick.AddListener(() => OpenItemEquipWindow(index));
        }
    }

    public void RemoveItemFromAllSlots(Item targetItem)
    {
        for (int i = 0; i < inventory.consumableItemSlots.Count; i++)
        {
            if (inventory.consumableItemSlots[i] == targetItem)
            {
                inventory.consumableItemSlots[i] = null;
            }
        }

        UpdateEquipmentImages(); // UI 갱신
    }

    public void OpenItemWindow()
    {
        List<Item> allItems = inventory.items; // 모든 아이템 보여주기
        Debug.Log($"전체 아이템 개수: {allItems.Count}");

        OpenEquipmentWindow<Item>(
            null,
            allItems,
            null,
            null,
            itemSlotPrefab,
            EquipmentWindowMode.InventoryView // ✅ 인벤토리 모드
        );
    }

    public void OpenSkillWindow(int slotIndex)
    {
        selectedSkillSlotIndex = slotIndex;

        Skill currentSkill = inventory.skillSlots[slotIndex];

        List<Skill> availableSkills = inventory.GetEquippedWeaponSkills();
        GameObject skillSlot = Instantiate(skillSlotPrefab, uiParent);
        Debug.Log($"스킬 슬롯 {slotIndex} 클릭. {availableSkills.Count}개의 스킬이 사용 가능.");

        OpenEquipmentWindow(
            currentSkill,
            availableSkills,
            skill => EquipSkill((Skill)skill),
            UnequipSkill,
            skillSlotPrefab,
            EquipmentWindowMode.SlotEquip
        );
    }

    public void OpenItemEquipWindow(int slotIndex)
    {
        selectedItemSlotIndex = slotIndex;

        Item currentItem = inventory.consumableItemSlots[slotIndex];

        List<Item> availableItems = inventory.items.FindAll(item =>
            item.itemType == ItemType.Consumable || item.itemType == ItemType.key
        );

        OpenEquipmentWindow<Item>(
            currentItem,
            availableItems,
            item => EquipItemToSlot((Item)item),
            UnequipItemFromSlot,
            itemSlotPrefab,
            EquipmentWindowMode.SlotEquip // ✅ 장착 모드
        );
    }

    public void EquipItemToSlot(Item item)
    {
        if (selectedItemSlotIndex == -1) return;

        // 중복 제거
        for (int i = 0; i < inventory.consumableItemSlots.Count; i++)
        {
            if (inventory.consumableItemSlots[i] == item)
            {
                inventory.consumableItemSlots[i] = null;
                break;
            }
        }

        inventory.AssignItemToSlot(item, selectedItemSlotIndex);
        selectedItemSlotIndex = -1;
        UpdateEquipmentImages();
    }

    public void UnequipItemFromSlot()
    {
        if (selectedItemSlotIndex == -1) return;

        inventory.AssignItemToSlot(null, selectedItemSlotIndex);
        selectedItemSlotIndex = -1;
        UpdateEquipmentImages();

        
    }

    public void OpenEquipmentWindow<T>(
        T currentItem,
        List<T> items,
        System.Action<IItemData> onEquip,
        System.Action onUnequip,
        GameObject slotPrefab,
        EquipmentWindowMode mode // ✅ 추가된 모드 인자
    )
    {
        foreach (Transform child in uiParent)
        {
            Destroy(child.gameObject);
        }

        GameObject windowInstance = Instantiate(equipmentWindowPrefab, uiParent);
        EquipmentWindow equipmentWindow = windowInstance.GetComponent<EquipmentWindow>();

        if (equipmentWindow != null)
        {
            equipmentWindow.Initialize(
                currentItem,
                items,
                onEquip,
                onUnequip,
                slotPrefab,
                mode // ✅ 추가된 인자 전달
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
            UnequipWeapon,
            itemSlotPrefab,
            EquipmentWindowMode.SlotEquip // 무기 장착은 장비 슬롯
        );
    }

    public void OpenTopWindow()
    {
        var tops = inventory.GetItemsByType(ItemType.top);
        Debug.Log($"상의 개수: {tops.Count}");
        List<Item> topItems = inventory.GetItemsByType(ItemType.top);
        OpenEquipmentWindow(
            selectedTop,
            topItems,
            item => EquipTop((Item)item),
            UnequipTop,
            itemSlotPrefab,
            EquipmentWindowMode.SlotEquip
        );
    }

    public void OpenBottomWindow()
    {
        var bottoms = inventory.GetItemsByType(ItemType.bottom);
        Debug.Log($"하의 개수: {bottoms.Count}");
        List<Item> bottomItems = inventory.GetItemsByType(ItemType.bottom);
        OpenEquipmentWindow(
            selectedBottom,
            bottomItems,
            item => EquipBottom((Item)item),
            UnequipBottom,
            itemSlotPrefab,
            EquipmentWindowMode.SlotEquip
        );
    }

    public void EquipSkill(Skill skill)
    {
        if(selectedSkillSlotIndex == -1)
        {
            Debug.LogError("스킬 슬롯이 선택되지 않았습니다");
            return;
        }

        // 기존 스킬을 찾아서 제거
        for (int i = 0; i < inventory.skillSlots.Count; i++)
        {
            if (inventory.skillSlots[i] == skill)
            {
                inventory.skillSlots[i] = null;
                break;
            }
        }

        inventory.AssignSkillToSlot(skill, selectedSkillSlotIndex);
        
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

        // selectedSkill = null;
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

        ClearSkillSlots();  // ✅ 기존 슬롯 개수 유지하면서 초기화

        UpdateEquipmentImages();  // ✅ UI 먼저 업데이트

        var weaponSkills = inventory.GetEquippedWeaponSkills();
        UpdateSkillWindow(weaponSkills);  // ✅ 기존 4개 슬롯만 활용하도록 수정
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
        for (int i = 0; i < 4; i++)  // ✅ 무조건 4개 슬롯만 유지
        {
            inventory.skillSlots[i] = null;  // 데이터 초기화
        }

        foreach (var skillSlot in skillSlots)
        {
            Transform iconTransform = skillSlot.transform.Find("Icon");
            if (iconTransform == null) continue;

            Image skillIcon = iconTransform.GetComponent<Image>();
            if (skillIcon != null)
            {
                skillIcon.sprite = null;
                skillIcon.enabled = false;
            }
            skillSlot.onClick.RemoveAllListeners();
        }

        Debug.Log("✅ 스킬 슬롯 초기화 완료 (UI 슬롯 개수 유지)");
    }

    public void UpdateSkillWindow(List<Skill> skills)
    {
        Debug.Log($"스킬 창에 {skills.Count}개의 스킬을 표시합니다.");

        for (int i = 0; i < 4; i++)  // ✅ 무조건 4개 슬롯만 유지
        {
            Transform iconTransform = skillSlots[i].transform.Find("Icon");
            if (iconTransform == null) continue;

            Image skillIcon = iconTransform.GetComponent<Image>();
            if (skillIcon != null)
            {
                skillIcon.sprite = (i < skills.Count) ? skills[i]?.skillIcon : null;
                skillIcon.enabled = (i < skills.Count) && (skills[i] != null);
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
            weaponImage.gameObject.SetActive(selectedWeapon != null); // null이면 비활성화
        }
        // 상의 슬롯 아이콘 업데이트
        UpdateSlotIconByName(topSlot.transform, "Icon", selectedTop?.itemIcon);
        if (topImage != null)
        {
            topImage.sprite = selectedTop?.itemSprite;
            topImage.enabled = selectedTop != null;
            topImage.gameObject.SetActive(selectedTop != null);
        }
        // 하의 슬롯 아이콘 업데이트
        UpdateSlotIconByName(bottomSlot.transform, "Icon", selectedBottom?.itemIcon);
        if (bottomImage != null)
        {
            bottomImage.sprite = selectedBottom?.itemSprite;
            bottomImage.enabled = selectedBottom != null;
            bottomImage.gameObject.SetActive(selectedBottom != null);
        }

        // 🔹 스킬 슬롯 업데이트 (💡 추가된 부분)
        for (int i = 0; i < skillSlots.Count; i++)
        {
            Transform iconTransform = skillSlots[i].transform.Find("Icon");
            if (iconTransform == null) continue;

            Image skillIcon = iconTransform.GetComponent<Image>();
            if (skillIcon != null)
            {
                skillIcon.sprite = inventory.skillSlots[i]?.skillIcon;  // 인벤토리의 스킬 아이콘을 가져옴
                skillIcon.enabled = inventory.skillSlots[i] != null;  // 스킬이 없으면 비활성화
            }
        }
        
        for (int i = 0; i < itemSlots.Count; i++)
        {
            Transform iconTransform = itemSlots[i].transform.Find("Icon");
            if (iconTransform == null) continue;

            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = inventory.consumableItemSlots[i]?.itemIcon;
                iconImage.enabled = inventory.consumableItemSlots[i] != null;
            }
        }

        if (battleCharacterImage != null)
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