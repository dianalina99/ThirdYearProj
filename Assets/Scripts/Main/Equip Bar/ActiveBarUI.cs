using UnityEngine;

public class ActiveBarUI : MonoBehaviour
{
    public Transform itemsParent;

    ActiveBarSlot[] slots;

    private void Start()
    {
        ActiveBarManager.instance.onActiveBarChanged += UpdateUI;
    }

    public void UpdateUI()
    {
        slots = itemsParent.GetComponentsInChildren<ActiveBarSlot>();

        Debug.Log("Updating chest inventory UI");


        for (int i = 0; i < slots.Length; i++)
        {
            //Check if slot has item using its index.
            if (ActiveBarManager.instance.itemAtIndex.ContainsKey(i))
            {
                //Get the item.
                Item item;
                ActiveBarManager.instance.itemAtIndex.TryGetValue(i, out item);

                //Add the item in the slot.
                slots[i].AddItem(item, ActiveBarManager.instance.itemsCount[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }

        }

    }
}
