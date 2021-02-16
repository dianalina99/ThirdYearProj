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
            if (i < chestInventory.items.Count)
            {
                slots[i].AddItem(chestInventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }

    }
}
