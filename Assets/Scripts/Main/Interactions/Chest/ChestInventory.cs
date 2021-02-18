using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : MonoBehaviour
{
    public int noOfSlots = 9;
    //public List<Item> items = new List<Item>();

    public Dictionary<int, Item> itemAtIndex = new Dictionary<int, Item>();
    public Dictionary<Item, int> indexForItem = new Dictionary<Item, int>();
    public int[] itemsCount;

    public List<Item> possibleItems = new List<Item>();

    private void Start()
    {
        itemsCount = new int[noOfSlots];

        for (int index = 0; index < noOfSlots; index++)
        {
            itemsCount[index] = 0;
        }

        //Populate inventory with random items.
        int noOfItems = Random.Range(1, noOfSlots + 1);
         
        for(int i=0; i< noOfItems; i++)
        {
            int index = Random.Range(0, possibleItems.Count);
            this.Add(possibleItems[index]);
        }
        
    }

    public bool Add(Item item)
    {
        //Only add to inventory if item is default.
        if (item != null && item.isStorable)
        {
            if (itemAtIndex.Count > noOfSlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            //Check if same item type is already in inventory.
            if (itemAtIndex.ContainsValue(item))
            {
                //Increase item count.
                int index;
                indexForItem.TryGetValue(item, out index);

                itemsCount[index]++;

            }
            else
            {
                //Check first free spot and add it there.
                for (int i = 0; i < noOfSlots; i++)
                {
                    if (!itemAtIndex.ContainsKey(i))
                    {
                        //Insert item at that position.
                        itemAtIndex.Add(i, item);
                        indexForItem.Add(item, i);

                        itemsCount[i] = 1;

                        break;
                    }
                }
            }

            //Notify system that inventory has been changed.
            this.gameObject.GetComponent<ChestInventoryUI>().UpdateUI();

            return true;
        }

        return false;

    }

    public void Remove(Item item)
    {
        //Find item in dictionary and remove if count =1;
        //If count bigger, substract one.
        if (indexForItem.ContainsKey(item))
        {
            //Get the index.
            int index;
            indexForItem.TryGetValue(item, out index);

            if (itemsCount[index] == 1)
            {
                //Remove it.
                itemAtIndex.Remove(index);
                itemsCount[index] = 0;
                indexForItem.Remove(item);

            }
            else if (itemsCount[index] > 1)
            {
                //Only remove one instance of it.
                itemsCount[index]--;
            }

        }

        //Notify system that inventory has been changed.
        this.gameObject.GetComponent<ChestInventoryUI>().UpdateUI();
    }

}
