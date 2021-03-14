using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestToDungeonDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {      
        if(!GameManagerScript.instance.playerIsCurrentlyTeleporting && collider.tag == "Player")
        {
            Debug.Log("Exit forest. Entering dungeon...");

            GameManagerScript.instance.Reset();
            GameManagerScript.instance.dungeonInUse = true;
            GameManagerScript.instance.dungeonNeedsRegeneration = true;

            GameManagerScript.instance.playerIsCurrentlyTeleporting = true;
        }
    }
}
