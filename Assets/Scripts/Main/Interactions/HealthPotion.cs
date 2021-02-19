using UnityEngine;


[CreateAssetMenu(fileName = "New Health Potion", menuName = "Inventory/Item/HealthPotion")]
public class  HealthPotion: Item
{
    public int healAmount;

    public override void Use()
    {
        base.Use();

        //Increase Player health.
        PlayerStats.instance.IncreaseHealth(healAmount);

        //Remove potion from inventory.
        Inventory.instance.Remove(this);
    }

}
