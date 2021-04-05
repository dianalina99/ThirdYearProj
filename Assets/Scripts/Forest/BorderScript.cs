using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderScript : MonoBehaviour
{
    public string borderDirection; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && !GameManagerScript.instance.playerIsCurrentlyTeleporting)
        {
            if (borderDirection == "up")
            {
                Debug.Log("Player hit UP border");
                GameManagerScript.instance.previousForestGrid = GameManagerScript.instance.currentForestGrid;
                GameManagerScript.instance.currentForestGrid = GameManagerScript.instance.currentForestGrid.Up;
               
            }
            else if (borderDirection == "down")
            {
                Debug.Log("Player hit DOWN border");
                GameManagerScript.instance.previousForestGrid = GameManagerScript.instance.currentForestGrid;
                GameManagerScript.instance.currentForestGrid = GameManagerScript.instance.currentForestGrid.Down;
            }
            else if (borderDirection == "left")
            {
                Debug.Log("Player hit LEFT border");
                GameManagerScript.instance.previousForestGrid = GameManagerScript.instance.currentForestGrid;
                GameManagerScript.instance.currentForestGrid = GameManagerScript.instance.currentForestGrid.Left;
            }
            else if (borderDirection == "right")
            {
                Debug.Log("Player hit RIGHT border");
                GameManagerScript.instance.previousForestGrid = GameManagerScript.instance.currentForestGrid;
                GameManagerScript.instance.currentForestGrid = GameManagerScript.instance.currentForestGrid.Right;
            }

            GameManagerScript.instance.forestNeedsRegeneration = true;
        }
    }
}
