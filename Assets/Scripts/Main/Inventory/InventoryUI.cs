using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    public Transform itemsParent;

    InventorySlot[] slots;
    public GameObject inventoryUI;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
        
    }

    void UpdateUI()
    {
       Debug.Log("Updating inventory UI");

        for (int i = 0; i < slots.Length; i++)
        {
            //Check if slot has item using its index.
            if(Inventory.instance.itemAtIndex.ContainsKey(i))
            {
                //Get the item.
                Item item;
                inventory.itemAtIndex.TryGetValue(i, out item);

                //Add the item in the slot.
                slots[i].AddItem(item, inventory.itemsCount[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }

        }

        /*
        for(int i=0; i< slots.Length; i++ )
        {
            if(i< inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }*/

    }
}
