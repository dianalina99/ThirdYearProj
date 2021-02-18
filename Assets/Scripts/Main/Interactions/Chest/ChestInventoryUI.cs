using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventoryUI : MonoBehaviour
{
    ChestInventory chestInventory;
    public Transform itemsParent;

    ChestInventorySlot[] slots;
    
    public void UpdateUI()
    {
        chestInventory = this.GetComponent<ChestInventory>();

        slots = itemsParent.GetComponentsInChildren<ChestInventorySlot>();

        Debug.Log("Updating chest inventory UI");


        for (int i = 0; i < slots.Length; i++)
        {
            //Check if slot has item using its index.
            if (chestInventory.itemAtIndex.ContainsKey(i))
            {
                //Get the item.
                Item item;
                chestInventory.itemAtIndex.TryGetValue(i, out item);

                //Add the item in the slot.
                slots[i].AddItem(item, chestInventory.itemsCount[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }

        }

    }
}
