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
            Vector3 newPos = new Vector3(GameManagerScript.instance.playerRef.transform.position.x, GameManagerScript.instance.playerRef.transform.position.y, GameManagerScript.instance.playerRef.transform.position.z -5);
        
            //Check if newpos is within forest bounds. - this is to avoid the camera moving over the edge of the map while player is still on the map.
            if(GameManagerScript.instance.forestInUse)
            {
                //Up
                if(newPos.y > 81.0371)
                {
                    newPos = new Vector3(newPos.x, 81.0371f, newPos.z);
                }

                //Right
                if(newPos.x > 241.102f)
                {
                    newPos = new Vector3(241.102f, newPos.y, newPos.z);
                }

                //Left
                if(newPos.x < 160.68f)
                {
                    newPos = new Vector3(160.68f, newPos.y, newPos.z);
                }

                //Down
                if(newPos.y < -7.2999f)
                {
                    newPos = new Vector3(newPos.x, -7.2999f, newPos.z);
                }
            }

            this.transform.position = newPos;
        }

        
    }
}
