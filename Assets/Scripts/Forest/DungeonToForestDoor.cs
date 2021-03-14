using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonToForestDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!GameManagerScript.instance.playerIsCurrentlyTeleporting && collider.tag == "Player")
        {
            Debug.Log("Exit dungeon. Entering forest...");

            GameManagerScript.instance.Reset();
            GameManagerScript.instance.forestInUse = true;
            GameManagerScript.instance.forestNeedsRegeneration = true;

            GameManagerScript.instance.playerIsCurrentlyTeleporting = true;
        } 
    }
}
