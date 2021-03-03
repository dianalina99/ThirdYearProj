using UnityEngine;
using UnityEngine.UI;

public class ChestInventorySlot : MonoBehaviour
{
    Item item;
    public Image icon;
    public ChestInventory chest;
    public TMPro.TextMeshProUGUI noOfItems;

    public void AddItem(Item newItem, int number)
    {
        /*
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true; */

        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        
        //Check if it's an inventory slot for the equip bar - doesn't have text.
        if (noOfItems != null)
        {
            noOfItems.text = number.ToString();
        }

    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;

        noOfItems.text = "0";
    }

    public void TransferItemToInventory()
    {
        //Check if item has multiple instances.
        if(item!= null)
        {
            //Get index of the item.
            int index;
            chest.indexForItem.TryGetValue(item, out index);

            if(chest.itemsCount[index] > 1 && Inventory.instance.Add(item))
            { 
                //Only transfer one item.
                chest.Remove(item);
            }
            else if(chest.itemsCount[index] == 1 && Inventory.instance.Add(item))
            {
                chest.Remove(item);
                ClearSlot();
            }
        }
    }


}
