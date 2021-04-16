using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntrance : MonoBehaviour
{
    public static bool playerSpawned = false;
    public GameObject playerPrefab, environment;

    // Update is called once per frame
    void Update()
    {
        //If dungeon has finished generating, spawn player in.
        if(GameManagerScript.instance.dungeonReadyForPlayer)
        {
            Debug.Log("Spawing player in dungeon START...");
            GameManagerScript.instance.dungeonReadyForPlayer = false;
            
            //Move player to entry point.
            GameManagerScript.instance.playerRef.transform.position = GameManagerScript.instance.latestPlayerEntryPoint.transform.GetChild(0).transform.position + new Vector3(1,1,0);
            GameManagerScript.instance.playerRef.transform.SetParent(GameManagerScript.instance.dungeonMapRef.transform, true);

            GameManagerScript.instance.playerIsCurrentlyTeleporting = false;
            Debug.Log("Spawing player in dungeon STOP...");

            //Calculate area for the AI enemies.
            AstarPath.active.Scan();

        }
    }
}
