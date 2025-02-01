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
        if (skillSlots.Count != 4)  
        {
            Debug.LogWarning($"⚠ skillSlots 개수 이상: 현재 {skillSlots.Count}개 -> 4개로 조정");
            while (skillSlots.Count > 4)
            {
                skillSlots.RemoveAt(skillSlots.Count - 1);  // 4개 초과 슬롯 삭제
            }
        }
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
    public GameObject equipmentWindowPrefab; // 창 프리펩
    public Transform uiParent; // 장비 창의 부모 UI 오브젝트
    public GameObject itemSlotPrefab; //아이템 슬롯 프리펩
    public GameObject skillSlotPrefab; //스킬 슬롯 프리펩

    [Header("Slot Buttons")]
    public Button inventorySlot; //아이템 목록창
    public Button itemSlot; //아이템 장착 칸
    public Button weaponSlot; // 무기 장착 칸
    public Button topSlot; // 상의 장착 칸
    public Button bottomSlot; // 하의 장착 칸
    public List<Button> skillSlots; // 스킬 슬롯 (4개)

    private Item selectedItem; //선택된 아이템
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
        inventorySlot.onClick.AddListener(OpenBottomWindow);
        itemSlot.onClick.RemoveAllListeners();
        itemSlot.onClick.AddListener(OpenBottomWindow);
        
        for(int i = 0; i < skillSlots.Count; i++)
        {
            int index = i;
            skillSlots[index].onClick.RemoveAllListeners();
            skillSlots[index].onClick.AddListener(() => OpenSkillWindow(index));
        }
        
    }

    public void OpenItemWindow()
    {
        List<Item> consumableItems = inventory.GetItemsByType(ItemType.Consumable);
        List<Item> keyItems = inventory.GetItemsByType(ItemType.key);

        Debug.Log($"소모품 개수: {consumableItems.Count}, 키아이템 개수: {keyItems.Count}");

        List<Item> allItems = new List<Item>();
        allItems.AddRange(consumableItems);  // ✅ 소모품 먼저 추가
        allItems.AddRange(keyItems);         // ✅ 키 아이템 추가

        OpenEquipmentWindow(null, allItems, null, null);
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
        var tops = inventory.GetItemsByType(ItemType.top);
        Debug.Log($"상의 개수: {tops.Count}");
        List<Item> topItems = inventory.GetItemsByType(ItemType.top);
        OpenEquipmentWindow(selectedTop, topItems, item => EquipTop((Item)item), UnequipTop);
    }

    public void OpenBottomWindow()
    {
        var bottoms = inventory.GetItemsByType(ItemType.bottom);
        Debug.Log($"하의 개수: {bottoms.Count}");
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
            Debug.Log($"{itemData.GetName()} 장착");
            HandleEquip(itemData); //타입에 따라 장착 처리
            OptionWindow.SetActive(false);
            DisableItemSkillWindow();
        });

        // 해제 버튼
        unequipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.AddListener(() =>
        {
            Debug.Log($"{itemData.GetName()} 해제");
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

부싯 가루
-
~~~
부싯돌을 곱게 갈아 만든 가루. 휴대가 간편하고 사용법이 단순하여 숙련되지 않은 사람들도 쉽게 사용할 수 있다.
값이 저렴하고 쉽게 구할 수 있어 평민층이나 위험한 일을 주로 하는 용병, 낭인들이 애용한다.

발화 부싯가루 (Flaming Flint Powder)
설명: 불꽃석을 곱게 갈아 만든 가루. 허공에 뿌리고 무기를 휘두르면 가루가 타오르며 화염 속성이 무기에 깃든다.

번개 부싯가루 (Thunder Flint Powder)
설명: 전기석의 가루로, 허공에 뿌릴 때 가루 입자가 전기를 머금고 무기에 번개 속성을 부여한다.

썩은 부싯가루 (Rotten Flint Powder)
설명: 부패한 암석과 독성 광물에서 추출한 가루. 허공에 뿌린 뒤 무기를 휘두르면 독성이 스며든다.

서리 부싯가루 (Frosted Flint Powder)
설명: 극한의 얼음 결정에서 추출한 서리 가루. 가루를 뿌리고 무기를 휘두르면 무기가 차가운 서리로 감싸인다.

붉은 부싯가루 (Crimson Flint Powder)
설명: 응고된 피를 갈아 만든 붉은 가루. 허공에 뿌리고 무기를 내려치면 무기에 출혈 속성이 깃든다.
~~~

부싯돌
-
~~~
특수한 광물과 마법적 공정을 통해 제작된 단단한 돌. 칼날을 따라 표면을 갈아내면 마찰과 마법의 힘으로 무기에 속성을 부여한다.
부싯돌은 고급 장비로, 주로 귀족이나 기사들이 사용한다.
소유 자체가 지위를 상징하며, 종종 세련된 장식이 더해져 귀족들의 취향을 반영하기도 한다.
전투 외에도 가문 간의 결투나 명예를 위한 의식적인 전투에서 부싯돌이 사용되기도 한다.
일부 명문 가문에서는 가문의 문양을 새긴 부싯돌을 제작하여 전투 시뿐만 아니라 가문의 상징으로도 사용한다.
사용법은 다소 번거롭지만, 숙련된 기사들이 무기와 함께 다루는 모습을 통해 그들의 기량과 품격을 드러내는 도구로 여겨진다.

발화 부싯돌 (Flaming Flint)
설명: 화염석으로 제작된 단단한 부싯돌. 칼날을 따라 면을 갈아내면 불꽃이 타올라 무기에 화염 속성을 부여한다.

번개 부싯돌 (Thunder Flint)
설명: 전기석 조각을 정제한 부싯돌. 무기를 갈아내면 전류가 흐르며 무기에 번개 속성을 부여한다.

썩은 부싯돌 (Rotten Flint)
설명: 독성 광물과 부패한 암석을 혼합하여 제작된 부싯돌. 칼날에 문지르면 독성이 스며들어 적을 약화시킨다.

서리 부싯돌 (Frosted Flint)
설명: 극지방에서 채취된 얼음 결정으로 제작된 부싯돌. 칼날을 갈아내면 냉기가 무기를 감싸 적을 둔화시킨다.

붉은 부싯돌 (Crimson Flint)
설명: 응고된 피를 결정화하여 제작된 부싯돌. 칼날을 따라 갈아내면 무기에 출혈 속성이 깃든다.

~~~

검은 잼 캔디
-
~~~
런던의 슬럼가에서 플럼(자두)이나 베리류의 과일 찌꺼기를 설탕으로 코팅한 싸구려 캔디
부유층의 과일 가공장에서 버려진 찌꺼기와 값싼 설탕으로 만들어졌다
절망의 끝에서도 아이들은 이 달콤한 조각을 소중히 품고 하루를 버텼다.

체력을 약간 회복한다.
단, 소량의 독성 성분이 섞여 있어 장기간 섭취하면 신체에 악영향을 미칠 수 있다.
"희망은 언제나 가장 비싼 대가를 요구한다."
~~~

아편
-
~~~
1. 빈민층: 불순물 섞인 아편
아이템 이름: "거무튀튀한 덩어리"

"검붉은 덩어리 속에는 고통과 희망이 뒤섞여 있다. 그러나 남은 것은 결국 질 낮은 절망뿐."

효과:
체력 10% 회복.
일정 시간 동안 공격력 5% 상승.
부작용: 사용 후 30초 동안 받는 피해가 10% 증가.
설명:
불순물이 섞인 저급 아편의 덩어리. 악취가 나며 손에 묻는 검붉은 찌꺼기는 사용자의 건강을 갉아먹는다.
저렴한 값으로 빈민가에서 쉽게 구할 수 있지만, 중독의 위험이 가장 크다.
2. 중산층: 로더넘 (Laudanum)
아이템 이름: "희석된 고통"

"맑은 갈색 액체는 은밀한 위안을 약속하지만, 결국 모든 것을 잠식한다."

효과:
체력 20% 회복.
1분간 받는 피해 10% 감소.
공격력 10% 상승.
설명:
알코올에 아편을 녹여 만든 로더넘. 병에 담긴 갈색 액체는 중산층의 가정에서 진통제와 진정제로 자주 사용되었다.
한 모금으로 일시적인 위안을 얻을 수 있지만, 이내 더 큰 갈망을 불러온다.
3. 고위층: 정제된 아편 알약
아이템 이름: "희귀한 회색빛 알약"

"작고 고운 회색빛 알약은 망각의 문을 열고, 그 끝에서만 진정한 고요를 약속한다."

효과:
체력 30% 회복.
2분간 받는 피해 15% 감소.
공격력 15% 상승.
부작용: 2분이 지나면 이동 속도 10% 감소 (30초).
설명:
귀족과 상류층이 애용하는 최고급 정제 아편. 순수한 재료로 만들어져 효과는 강력하고 부작용은 비교적 적다.
고급 용기에 담겨 거래되며, 쾌락과 사치의 상징으로 여겨졌다.

~~~
