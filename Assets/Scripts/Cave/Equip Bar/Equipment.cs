using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;
    public int armorModifier;
    public int damageModifier;



    public override void Use()
    {
        base.Use();

        //Equip the item
        EquipmentManager.instance.Equip(this);

        //Remove from inventory
        RemoveFromInventory();

        //Add it to equipment bar.

    }


}

public enum EquipmentSlot { Head, Body, Feet, Weapon}
