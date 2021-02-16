using UnityEngine;

public class EquipBarUI : MonoBehaviour
{
    EquipmentManager equipment;
    public Transform itemsParent;

    InventorySlot[] equipSlots;

    // Start is called before the first frame update
    void Start()
    {
        equipment = EquipmentManager.instance;
        equipment.onEquipmentChanged += UpdateUI;
        equipSlots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

  

    void UpdateUI(Equipment newItem, Equipment oldItem)
    {
        Debug.Log("Updating equipment bar UI");

        for (int i = 0; i < equipment.currentEquipment.Length; i++)
        {
            if (equipment.currentEquipment[i] != null)
            {
                equipSlots[i].AddItem(equipment.currentEquipment[i]);
            }
            else
            {
                equipSlots[i].ClearSlot();
            }
        }

    }
}
