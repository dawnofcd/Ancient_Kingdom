using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItem;
    [SerializeField] private float chanceToLooseMaterial;

    public override void GenerateDrop()
    {   
        Inventory inventory = Inventory.instance;

        List<InventoryItem> currentStash = inventory.GetStashList();
        List<InventoryItem> materialToUnequip = new List<InventoryItem>();

        List<InventoryItem> currentEquipment = inventory.GetEquipmentsList();
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();

        foreach (InventoryItem item in currentEquipment)
        {
            if(Random.Range(0, 100) <= chanceToLooseItem )
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
            
        }

        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemDataEquipments);
            
        }

        foreach (InventoryItem item in currentStash)
        {
            if(Random.Range(0, 100) <= chanceToLooseMaterial)
            {
                DropItem(item.data);
                materialToUnequip.Add(item);
            }   
        }

        for (int i = 0; i < materialToUnequip.Count; i++)
        {
            inventory.RemoveItem(materialToUnequip[i].data);
        }

    }



}
