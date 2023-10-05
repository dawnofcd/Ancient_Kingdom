using Newtonsoft.Json.Converters;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItem;
    public List<InventoryItem> equipment;
    public Dictionary<ItemDataEquipments, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData,InventoryItem> inventoryDictionary;
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;


    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_Equipment[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;
    
    public float flaskCooldown {get; private set;}
    private float armorCooldown;

    [Header("Data base")]
    
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemDataEquipments> loadedEquipments;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else 
            Destroy(gameObject);    
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemDataEquipments, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_Equipment>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItem();
    }

    private void AddStartingItem()
    {   
        foreach(ItemDataEquipments item in loadedEquipments)
        {
            EquipItem(item);
        }

        if(loadedItems.Count > 0 )
        {
            foreach(InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }
            return;
        }

        for (int i = 0; i < startingItem.Count; i++)
        {   
            if(startingItem[i] != null)
                AddItem(startingItem[i]);
        }
    }

    public void EquipItem(ItemData _item)
    {
        ItemDataEquipments newEquipment = _item as ItemDataEquipments;

        InventoryItem newItem = new InventoryItem(newEquipment);
        ItemDataEquipments oldEquipment = null;

        foreach (KeyValuePair<ItemDataEquipments, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
            }
        }

        if(oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemDataEquipments itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemDataEquipments, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }


        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }


        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();

    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++) //Update info of Stat in character UI
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);

        }
        else if(_item.itemType == ItemType.Material)
        {
            AddToStash(_item);

        }


        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if(inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if(value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }

        if(stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if(stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);

            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();

    }

    public bool CanAddItem()
    {
        if(inventory.Count >= inventoryItemSlot.Length)
        {
            return false;
        }
        return true;

    }


    public bool CanCraft(ItemDataEquipments _itemToCraft, List<InventoryItem> _requireMaterials)
    {   
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requireMaterials.Count; i++)
        {
            if(stashDictionary.TryGetValue(_requireMaterials[i].data, out InventoryItem stashValue))
            {
                if(stashValue.stackSize < _requireMaterials[i].stackSize)
                {
                    Debug.Log("not enough material");
                    return false;
                }
                else
                {   
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("not enough material");
                return false;
            }

        }

        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }

        AddItem(_itemToCraft);
        Debug.Log(" Item " + _itemToCraft.name);

        return true;


    }

    public List<InventoryItem> GetEquipmentsList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemDataEquipments GetEquipment(EquipmentType _type)
    {
        ItemDataEquipments equipedItem = null;


        foreach (KeyValuePair<ItemDataEquipments, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
            {
                equipedItem = item.Key;
            }
        }  

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemDataEquipments currentFlask = GetEquipment(EquipmentType.Flask);

        if(currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if(canUseFlask)
        {   
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null); 
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("Flask on cooldown");

    }

    public bool CanUSeArmor()
    {   
        ItemDataEquipments currentArmor = GetEquipment(EquipmentType.Armor);

        if(Time.time  > lastTimeUsedArmor + armorCooldown)
        {       
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }


        Debug.Log("Armor on cooldown");
        return false;

    }

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach(var item in itemDataBase)
            {
                if(item != null && item.itemID == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach(string loadedItemID in _data.equipmentID)
        {
            foreach(var item in itemDataBase)
            {
                if(item != null && loadedItemID == item.itemID)
                {
                    loadedEquipments.Add(item as ItemDataEquipments);
                }
            }
        }


    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentID.Clear();
        
        foreach(KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemDataEquipments, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentID.Add(pair.Key.itemID);
        }

    }

#if UNITY_EDITOR
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetName = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" }); //ko muốn bộ lọc, muốn biết hướng đi

        foreach(string SOName in assetName)
        {   
            //chuyển đường dẫn scriptable thành cơ sở dữ liệu nội dung 
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            //dữ liệu thành dữ liệu nội dung
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }
        return itemDataBase; // trả vè cơ sở dữ liệu vật phẩm
    
    }
#endif
}
