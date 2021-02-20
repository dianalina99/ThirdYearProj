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

        //Remove potion from inventory OR from active bar.
        if(ActiveBarManager.instance.indexForItem.ContainsKey(this))
        {
            //Remove it from the active bar.
            ActiveBarManager.instance.Remove(this);

        }
        else if(Inventory.instance.indexForItem.ContainsKey(this))
        {
            //Remove it from inventory.
            Inventory.instance.Remove(this);
        }
       
        
        
    }

}
