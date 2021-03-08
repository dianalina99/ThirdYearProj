using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonToForestDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManagerScript.instance.Reset();
        GameManagerScript.instance.forestInUse = true;
        GameManagerScript.instance.forestNeedsRegeneration = true;
    }
}
