using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    #region Singleton
    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More inventories found!");
            return;
        }
        instance = this;
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int noOfSlots = 9;

    public Dictionary<int, Item> itemAtIndex = new Dictionary<int, Item>();
    public Dictionary<Item, int> indexForItem = new Dictionary<Item, int>();
    public int[] itemsCount;

    private void Start()
    {
        itemsCount = new int[noOfSlots];

       for(int index =0; index< noOfSlots; index ++)
        {
            itemsCount[index] = 0;
        }
    }

    public Transform interactingWithChest = null;


    public bool Add(Item item)
    {
        //Only add to inventory if item is default.
        if(item.isStorable)
        {
            if(itemAtIndex.Count > noOfSlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            //Check if same item type is already in inventory.
            if(itemAtIndex.ContainsValue(item))
            {
                //Increase item count.
                int index;
                indexForItem.TryGetValue(item, out index);

                itemsCount[index] ++;

                //Notify system that inventory has been changed.
                if (onItemChangedCallback != null)
                {
                    onItemChangedCallback.Invoke();
                }

                return true;
            }

            //Check first free spot and add it there.
            for(int i=0; i< noOfSlots; i++)
            {
                if(!itemAtIndex.ContainsKey(i))
                {
                    //Insert item at that position.
                    itemAtIndex.Add(i, item);
                    indexForItem.Add(item, i);

                    itemsCount[i] = 1;

                    break;
                }
            }
            

            //Notify system that inventory has been changed.
            if(onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }

            return true;
        }

        return false;
        
    }

    public void Remove( Item item)
    {
      
        //Find item in dictionary and remove if count =1;
        //If count bigger, substract one.
        if(indexForItem.ContainsKey(item))
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
            else if(itemsCount[index] > 1)
            {
                //Only remove one instance of it.
                itemsCount[index]--;
            }

        }

        //Notify system that inventory has been changed.
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }    

    }
}
