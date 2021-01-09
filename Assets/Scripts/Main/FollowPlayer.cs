using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //Check if player is spawned.
        if(player != null)
        {
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z -5);
        }

        
    }
}
