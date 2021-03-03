using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntrance : MonoBehaviour
{
    public static bool playerSpawned = false;
    public GameObject playerPrefab, environment;


    private void Start()
    {
        environment = GameObject.FindGameObjectWithTag("Environment");    
    }

    // Update is called once per frame
    void Update()
    {
        if (MapGeneration.readyForPlayer && !playerSpawned && MapGeneration.playerRef == null)
        {
           
            //Spawn in player.
            MapGeneration.playerRef = Instantiate(playerPrefab, this.transform.GetChild(0).transform.position, Quaternion.identity) as GameObject;
            MapGeneration.playerRef.transform.SetParent(environment.transform, true);
            playerSpawned = true;
        }

        //If there is a player already, just move it to the new entry point.
        else if (MapGeneration.readyForPlayer && !playerSpawned )
        {
            MapGeneration.playerRef.transform.position = MapGeneration.entryPortalRef.transform.position;
            MapGeneration.playerRef.transform.SetParent(environment.transform, true);
            playerSpawned = true;
        }



    }
}
