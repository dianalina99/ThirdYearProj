using UnityEngine;
using UnityEngine.UI;

public class ActiveBarSlot : MonoBehaviour
{
    Item item;
    public Image icon;
    public Button removeButton;

    public TMPro.TextMeshProUGUI noOfItems;
    public KeyCode activeKey;

    private void Update()
    {
        if(Input.GetKeyDown(activeKey))
        {
            //Use item from that slot.
            item.Use();

            //Update UI.

        }
    }

    public void AddItem(Item newItem, int number)
    {
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
        if (item != null)
        {
            //Get index of the item.
            int index;
            ActiveBarManager.instance.indexForItem.TryGetValue(item, out index);

            if (ActiveBarManager.instance.itemsCount[index] > 1 && Inventory.instance.Add(item))
            {
                //Only transfer one item.
                ActiveBarManager.instance.Remove(item);
            }
            else if (ActiveBarManager.instance.itemsCount[index] == 1 && Inventory.instance.Add(item))
            {
                ActiveBarManager.instance.Remove(item);
                ClearSlot();
            }
        }
    }


}
