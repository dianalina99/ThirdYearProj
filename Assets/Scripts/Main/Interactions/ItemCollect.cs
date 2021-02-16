using UnityEngine;

public class ItemCollect : Collectible
{
    public Item item;
    public override void Interact()
    {
        base.Interact();

        Collect();
    }

    private void Collect()
    {
        Debug.Log("Collect " + item.name);

        //Add item to inventory and remove from scene.
        if (Inventory.instance.Add(item))
        {
            Destroy(gameObject);
        }
    }

}


/*
public class ItemCollect : Interactable
{
    public Item item;
    public override void Interact()
    {
        base.Interact();

        Collect();
    }

    private void Collect()
    {
        Debug.Log("Collect " + item.name);

        //Add item to inventory and remove from scene.
        if(Inventory.instance.Add(item))
        {
            Destroy(gameObject);

        }
    }
}*/


