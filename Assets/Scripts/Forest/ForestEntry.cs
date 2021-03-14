using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestEntry : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(GameManagerScript.instance.forestReadyForPlayer)
        {
            Debug.Log("Spawing player in forest START...");
            GameManagerScript.instance.forestReadyForPlayer = false;

            //Spawn player next to forest entry point (child of this object).
            GameManagerScript.instance.playerRef.transform.position = GameManagerScript.instance.latestPlayerEntryPoint.transform.GetChild(0).transform.position + new Vector3(1, 1, 0);
            GameManagerScript.instance.playerRef.transform.SetParent(GameManagerScript.instance.dungeonMapRef.transform, true);

            GameManagerScript.instance.playerIsCurrentlyTeleporting = false;

            Debug.Log("Spawing player in forest STOP...");
        }        
    }
}
