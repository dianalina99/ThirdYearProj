using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Item item;
    public Image icon;
    public Button removeButton;
    public TMPro.TextMeshProUGUI noOfItems;

    public void AddItem(Item newItem, int number)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;

        //Check if it's an inventory slot for the equip bar - doesn't have text.
        if(noOfItems != null)
        {
            noOfItems.text = number.ToString();
        }
        
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

}
