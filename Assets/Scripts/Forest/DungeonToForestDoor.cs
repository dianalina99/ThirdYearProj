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

            //Delete all created dungeon maps, except the first one.
            foreach(Transform child in GameManagerScript.instance.dungeonMapRef.transform)
            {
                if(child.gameObject.name == "Environment(Clone)")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            //Update the latest generated dungeon environment variable so player can spawn correctly next time it enters the dungeon.
           GameManagerScript.instance.latestGeneratedEnvironmentDungeon = GameManagerScript.instance.dungeonMapRef.transform.GetChild(0).transform.gameObject;
        } 
    }
}
