using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : MonoBehaviour
{
    public int noOfSlots = 9;
    public List<Item> items = new List<Item>();

    public List<Item> possibleItems = new List<Item>();

    private void Start()
    {
        //Populate inventory with random items.
        int noOfItems = Random.Range(1, noOfSlots + 1);
         
        for(int i=0; i< noOfItems; i++)
        {
            int index = Random.Range(0, possibleItems.Count);
            this.Add(possibleItems[index]);
        }

        this.Add(possibleItems[0]);
    }

    public bool Add(Item item)
    {
        //Only add to inventory if item is default.
        if (item.isStorable && item!= null)
        {
            if (items.Count >= noOfSlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            items.Add(item);

            //Notify system that inventory has been changed.
            this.gameObject.GetComponent<ChestInventoryUI>().UpdateUI();

            return true;
        }

        return false;

    }

    public void Remove(Item item)
    {
        items.Remove(item);

        //Notify system that inventory has been changed.
        this.gameObject.GetComponent<ChestInventoryUI>().UpdateUI();
    }

}
