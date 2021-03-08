using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestToDungeonDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManagerScript.instance.Reset();
        GameManagerScript.instance.dungeonInUse = true;
        GameManagerScript.instance.dungeonNeedsRegeneration = true;
    }
}
