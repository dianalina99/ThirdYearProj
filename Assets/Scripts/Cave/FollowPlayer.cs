using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
        //Check if player is spawned.
        if(GameManagerScript.instance.playerRef != null)
        {
            this.transform.position = new Vector3(GameManagerScript.instance.playerRef.transform.position.x, GameManagerScript.instance.playerRef.transform.position.y, GameManagerScript.instance.playerRef.transform.position.z -5);
        }

        
    }
}
