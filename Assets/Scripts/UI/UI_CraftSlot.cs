using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{

    protected override void Start()
    {
        base.Start();


    }

    public void SetupCraftSlotWith(ItemDataEquipments _data )
    {

        if( _data == null)
            return;

        item.data = _data;

        itemImage.sprite = _data.itemIcon;
        itemText.text = _data.itemName;

        if(itemText.text.Length > 12)
            itemText.fontSize *= 0.7f;
        else
            itemText.fontSize = 24; 

    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemDataEquipments);
    }
}
