using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UI_ItemSlot : MonoBehaviour , IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item; 

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public  void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";

    }

    public void UpdateSlot(InventoryItem _newItem)
    {   
        item = _newItem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.itemIcon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {       
        if(item == null)
            return;

        if(Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }
        if(item.data.itemType == ItemType.Equipment)
            Inventory.instance.EquipItem(item.data);

        ui.itemToolTips.HideToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if(item == null)
            return;
        ui.itemToolTips.ShowToolTip(item.data as ItemDataEquipments);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(item == null)
            return;
        
        ui.itemToolTips.HideToolTip();
    }
}
