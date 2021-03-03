using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalExit : MonoBehaviour
{
    public static bool generateNewMap = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //SceneManager.LoadScene("MapLayout");
        Debug.Log("Generate new level");
        generateNewMap = true;
        PortalEntrance.playerSpawned = false;
    }

}
