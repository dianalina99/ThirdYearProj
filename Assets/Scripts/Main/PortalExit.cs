using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalExit : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        //SceneManager.LoadScene("MapLayout");
        Debug.Log("Generate new level");
    }
}
