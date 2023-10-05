using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Equipment : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {   
        if(item == null || item.data == null)
            return;

        Inventory.instance.UnequipItem(item.data as ItemDataEquipments);
        Inventory.instance.AddItem(item.data as ItemDataEquipments);

        ui.itemToolTips.HideToolTip();

        CleanUpSlot();
    }

}
