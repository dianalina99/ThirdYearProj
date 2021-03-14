using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!GameManagerScript.instance.playerIsCurrentlyTeleporting && collider.tag == "Player")
        {
            //Move to next dungeon room.
            Debug.Log("Generate new dungeon level...");

            GameManagerScript.instance.Reset();
            GameManagerScript.instance.dungeonInUse = true;
            GameManagerScript.instance.dungeonNeedsRegeneration = true;

            GameManagerScript.instance.playerIsCurrentlyTeleporting = true;
        }

    }
}
