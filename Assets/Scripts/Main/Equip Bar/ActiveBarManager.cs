using System.Collections.Generic;
using UnityEngine;

public class ActiveBarManager : MonoBehaviour
{
    #region Singleton
    public static ActiveBarManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More active bars found!");
            return;
        }
        instance = this;
    }

    #endregion

    public int noOfSlots = 7;
    public Dictionary<int, Item> itemAtIndex = new Dictionary<int, Item>();
    public Dictionary<Item, int> indexForItem = new Dictionary<Item, int>();
    public int[] itemsCount;

    public delegate void OnActiveBarChanged();
    public OnActiveBarChanged onActiveBarChanged;

    private void Start()
    {
        itemsCount = new int[noOfSlots];

        for (int index = 0; index < noOfSlots; index++)
        {
            itemsCount[index] = 0;
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
            if(onActiveBarChanged != null)
            {
                onActiveBarChanged.Invoke();
            }

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
        if (onActiveBarChanged != null)
        {
            onActiveBarChanged.Invoke();
        }
    }
}
