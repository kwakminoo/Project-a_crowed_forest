아이템 획득 및 인벤토리에 추가
--------------
1. 아이템 클래스: 게임에서 다양한 아이템을 정의하기 위해, Item 클래스를 만듭니다. 각 아이템은 고유한 속성(이름, 설명, 아이템 타입 등)을 가집니다.

2. 아이템 획득 로직: 플레이어가 특정 이벤트(전투 승리, 상자 열기 등)에서 아이템을 획득했을 때, 해당 아이템을 인벤토리에 추가합니다.

3. 인벤토리 시스템: 인벤토리는 플레이어가 소유한 아이템을 관리하는 리스트입니다. 리스트는 Item 객체로 구성되며, 아이템 추가, 제거, 정렬 등의 기능을 수행합니다.
~~~C#
public class Item
{
    public string itemName;
    public string description;
    public ItemType itemType;  // 장비, 소모품 등 아이템 타입
}

public class Player : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();

    public void AddItemToInventory(Item newItem)
    {
        inventory.Add(newItem);
        Debug.Log(newItem.itemName + "을(를) 인벤토리에 추가했습니다.");
    }
}

public class Inventory
{
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
}

~~~

장비 시스템
--------------
1. 장비 클래스: 장비(무기, 방어구 등)는 특별한 아이템으로, 플레이어의 능력치에 영향을 줍니다. 각 장비는 장착 가능한 슬롯(무기 슬롯, 방어구 슬롯 등)을 가지고 있습니다.

2. 장비 슬롯: 캐릭터는 특정 슬롯에만 장비를 장착할 수 있습니다. 예를 들어, 무기는 무기 슬롯에만 장착할 수 있습니다.

~~~C#
public class Equipment : Item
{
    public int attackBonus;
    public int defenseBonus;
}

public enum EquipmentSlot { Head, Body, Weapon }

public class EquipmentManager
{
    public Dictionary<EquipmentSlot, Equipment> equippedItems = new Dictionary<EquipmentSlot, Equipment>();

    public void Equip(Equipment newEquip, EquipmentSlot slot)
    {
        equippedItems[slot] = newEquip;
    }

    public void Unequip(EquipmentSlot slot)
    {
        equippedItems.Remove(slot);
    }
}

~~~
