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
    public OnItemChanged OnItemChangedCallback;

    public int noOfSlots = 10;
    public List<Item> items = new List<Item>();

    public bool Add(Item item)
    {
        //Only add to inventory if item is default.
        if(item.isStorable)
        {
            if(items.Count >= noOfSlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            items.Add(item);

            //Notify system that inventory has been changed.
            if(OnItemChangedCallback != null)
            {
                OnItemChangedCallback.Invoke();
            }

            return true;
        }

        return false;
        
    }

    public void Remove( Item item)
    {
        items.Remove(item);

        //Notify system that inventory has been changed.
        if (OnItemChangedCallback != null)
        {
            OnItemChangedCallback.Invoke();
        }
    }
}
