using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EquipmentWindowMode
{
    SlotEquip,      // 슬롯에서 장착/해제
    InventoryView   // 인벤토리에서 사용/버리기
}

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
    private EquipmentWindowMode currentMode;

    public void Initialize<T>
    (
        T currentItem,
        List<T> items,
        System.Action<IItemData> onEquip,
        System.Action onUnequip,
        GameObject slotPrefab,
        EquipmentWindowMode mode = EquipmentWindowMode.SlotEquip
    )
    {
        currentMode = mode;
        itemSlotPrefab = slotPrefab;

        foreach (Transform child in itemListContent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"{items.Count}개의 아이템이 존재합니다");

        foreach (T item in items)
        {
            if (item == null)
            {
                Debug.LogError("Null아이템 발견, Skip");
                continue;
            }

            GameObject itemSlot = Instantiate(itemSlotPrefab, itemListContent);
            Button itemButton = itemSlot.GetComponent<Button>();
            Image itemIcon = itemSlot.GetComponentInChildren<Image>();

            var itemData = item as IItemData;
            if (itemData != null)
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
        currentItemIcon.sprite = itemData.GetIcon();
        currentItemName.text = itemData.GetName();
        currentItemOption.text = itemData.GetOption();

        equipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.RemoveAllListeners();

        if (currentMode == EquipmentWindowMode.InventoryView)
        {
            equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "사용";
            unequipButton.GetComponentInChildren<TextMeshProUGUI>().text = "버리기";

            equipButton.onClick.AddListener(() =>
            {
                if (itemData is Item item)
                {
                    UseConsumable(item);
                    InventoryManager.Instance.RemoveItemFromAllSlots(item); 
                }
                OptionWindow.SetActive(false);
                DisableItemSkillWindow();
            });

            unequipButton.onClick.AddListener(() =>
            {
                if (itemData is Item item)
                {
                    Inventory.Instance.items.Remove(item);
                    Inventory.Instance.RaiseInventoryUpdatedEnent();
                    InventoryManager.Instance.RemoveItemFromAllSlots(item); 
                }
                OptionWindow.SetActive(false);
                DisableItemSkillWindow();
            });
        }
        else // SlotEquip 모드
        {
            equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "장착";
            unequipButton.GetComponentInChildren<TextMeshProUGUI>().text = "해제";

            equipButton.onClick.AddListener(() =>
            {
                Debug.Log($"{itemData.GetName()} 장착");
                HandleEquip(itemData);
                OptionWindow.SetActive(false);
                DisableItemSkillWindow();
            });

            unequipButton.onClick.AddListener(() =>
            {
                Debug.Log($"{itemData.GetName()} 해제");
                onUnequip?.Invoke();
                OptionWindow.SetActive(false);
            });
        }
        
        OptionWindow.SetActive(true);
    }

    private void UseConsumable(Item item)
    {
        Debug.Log($"{item.itemName}을 사용했습니다!");
        Inventory.Instance.items.Remove(item); // ✅ 아이템 사용 후 제거
        Inventory.Instance.RaiseInventoryUpdatedEnent(); // ✅ 인벤토리 갱신
    }

    public void DisableItemSkillWindow()
    {
        Transform uiParent = transform.parent;
        if (uiParent == null)
        {
            Debug.LogError("UI 부모 오브젝트를 찾을 수 없습니다");
            return;
        }

        Transform itemSkillWindow = uiParent.Find("Item_Skill Window(Clone)");
        if (itemSkillWindow != null)
        {
            itemSkillWindow.gameObject.SetActive(false);
            Destroy(itemSkillWindow.gameObject);
        }
    }

    private void HandleEquip(IItemData itemData)
    {
        if (itemData is Item item)
        {
            switch (item.itemType)
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

                case ItemType.Consumable:
                case ItemType.key:
                    // 🔹 장착 모드일 때만 슬롯에 배치
                    InventoryManager.Instance.EquipItemToSlot(item);
                    break;

                default:
                    Debug.LogError($"지원하지 않는 아이템 타입: {item.itemType}");
                    break;
            }
        }
        else if (itemData is Skill skill)
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