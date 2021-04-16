using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenDoorCollision : MonoBehaviour
{
    public Item keyItemRef;
    private Room roomReference;
    private bool locked = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If door collides with wall, remove the wall tile.
        if(collision.gameObject.tag == "WallTile")
        {
            Destroy(collision.gameObject);
        }

        //Only open when player has key when colliding.
        if(collision.gameObject.tag == "Player" && locked)
        {
            //Check if the player has required key.
            if(Inventory.instance.itemAtIndex.ContainsValue(keyItemRef))
            {
                locked = false;
                Destroy(this.gameObject);

                //Remove the key from inventory.
                int index = -1;
                Inventory.instance.indexForItem.TryGetValue(keyItemRef, out index);

                if(index != -1)
                {
                    Item key;
                    Inventory.instance.itemAtIndex.TryGetValue(index, out key);

                    if(key != null)
                    {
                        key.RemoveFromInventory();
                    }
                }

                
            }
            
        }
        
    }
}
