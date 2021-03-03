using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenDoorCollision : MonoBehaviour
{
    private Room roomReference;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "WallTile")
        {
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "Player")
        {
            //Check if the player has required key.
            Destroy(this.gameObject);
        }
        
    }
}
