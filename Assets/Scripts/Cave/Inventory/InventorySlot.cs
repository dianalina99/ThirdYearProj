using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Item item;
    public Image icon;
    public Button removeButton;
    public Button addButton;
    public TMPro.TextMeshProUGUI noOfItems;

    public void AddItem(Item newItem, int number)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;

        if(newItem.isForActiveBar && addButton != null)
        {
            addButton.interactable = true;
        }

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

        //Check if it's not an inventory slot with add button.
        if (addButton != null)
        {
            addButton.interactable = false;
        }

        //Check if it's an inventory slot for the equip bar - doesn't have text.
        if (noOfItems != null)
        {
            noOfItems.text = "0";
        }

        
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

    public void OnAddButton()
    {
        //Check if item has multiple instances.
        if (item != null && item.isForActiveBar)
        {
            //Get index of the item.
            int index;
            Inventory.instance.indexForItem.TryGetValue(item, out index);

            if (ActiveBarManager.instance.Add(item))
            {
                //Only transfer one item.
                Inventory.instance.Remove(item);
            }

        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

}
