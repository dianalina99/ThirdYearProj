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
        if(GameManagerScript.instance.dungeonReadyForPlayer)
        {
            GameManagerScript.instance.dungeonReadyForPlayer = false;

            //Move player to entry point.
            GameManagerScript.instance.playerRef.transform.position = GameManagerScript.instance.latestPlayerEntryPoint.transform.GetChild(0).transform.position + new Vector3(1,1,0);
            GameManagerScript.instance.playerRef.transform.SetParent(GameManagerScript.instance.dungeonMapRef.transform, true);

        }


        /*
        if (GameManagerScript.instance.dungeonInUse &&  GameManagerScript.instance.dungeonReadyForPlayer && !playerSpawned && GameManagerScript.instance.playerRef == null)
        {

            //Spawn in player.
            GameManagerScript.instance.playerRef = Instantiate(playerPrefab, this.transform.GetChild(0).transform.position, Quaternion.identity) as GameObject;
            GameManagerScript.instance.playerRef.transform.SetParent(environment.transform, true);
            playerSpawned = true;
        }

        //If there is a player already, just move it to the new entry point.
        else if (GameManagerScript.instance.dungeonInUse && GameManagerScript.instance.dungeonReadyForPlayer && GameManagerScript.instance.playerRef != null)
        {
            GameManagerScript.instance.playerRef.transform.position = MapGeneration.entryPortalRef.transform.position;
            GameManagerScript.instance.playerRef.transform.SetParent(environment.transform, true);
            playerSpawned = true;
        }*/



    }
}
