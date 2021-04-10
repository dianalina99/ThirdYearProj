using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestEntry : MonoBehaviour
{
    public int distanceBetweenMaps;
    
    // Update is called once per frame
    void Update()
    {
        if(GameManagerScript.instance.forestReadyForPlayer)
        {
            Debug.Log("Spawing player in forest START...");
            GameManagerScript.instance.forestReadyForPlayer = false;

            //Spawn player next to forest entry point (child of this object).
            GameManagerScript.instance.playerRef.transform.position = GameManagerScript.instance.latestPlayerEntryPoint.transform.GetChild(0).transform.position;
            GameManagerScript.instance.playerRef.transform.SetParent(GameManagerScript.instance.forestMapRef.transform, true);

            GameManagerScript.instance.playerIsCurrentlyTeleporting = false;

            Debug.Log("Spawing player in forest STOP...");

            //Show new map on the minimap view.
            GameManagerScript.instance.minimapCamera.orthographicSize = 50;

            if (GameManagerScript.instance.minimapRef.transform.position.x < 1000)
            {
                GameManagerScript.instance.minimapRef.transform.position = new Vector3(GameManagerScript.instance.minimapRef.transform.position.x + distanceBetweenMaps,
                                                                                    565f,
                                                                                    GameManagerScript.instance.minimapRef.transform.position.z);

            }
                
        }        
    }
}
