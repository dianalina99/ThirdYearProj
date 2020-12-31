using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntrance : MonoBehaviour
{
    public bool playerSpawned = false;
    public GameObject player, environment;


    private void Start()
    {
        environment = GameObject.FindGameObjectWithTag("Environment");    
    }

    // Update is called once per frame
    void Update()
    {
        if (MapGeneration.readyForPlayer && !playerSpawned )
        {
            GameObject playerInst;

            //Spawn in player.
            playerInst = Instantiate(player, new Vector2(this.transform.GetChild(0).transform.position.x, this.transform.GetChild(0).transform.position.y),Quaternion.identity) as GameObject;
            playerInst.transform.SetParent(environment.transform, true);
            playerSpawned = true;
        }


    }
}
