using UnityEngine;
using UnityEngine.UI;

public class ChestInventorySlot : MonoBehaviour
{
    Item item;
    public Image icon;
    public ChestInventory chest;
   
    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    public void TransferItemToInventory()
    {
        //Add item to main inventory.
        if(Inventory.instance.Add(item))
        {
            //Remove item from chest inventory.
            chest.Remove(item);
            ClearSlot();
        }
    }


}
