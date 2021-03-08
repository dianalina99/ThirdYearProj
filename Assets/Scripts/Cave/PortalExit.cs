﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        //Move to next dungeon room.
        Debug.Log("Generate new dungeon level");

        GameManagerScript.instance.Reset();
        GameManagerScript.instance.dungeonNeedsRegeneration = true;
        GameManagerScript.instance.dungeonInUse = true;
    }

}
