using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMiniMap : MonoBehaviour
{
    private Animator miniMapSmall;
    private Animator miniMapBig;

    // Start is called before the first frame update
    void Start()
    {
        miniMapSmall = GameObject.FindGameObjectWithTag("MiniMapMin").GetComponent<Animator>();
        miniMapBig = GameObject.FindGameObjectWithTag("MiniMapMax").GetComponent<Animator>();
    }

    public void ShowMiniMap()
    {
        miniMapSmall.SetBool("on", false);
        miniMapBig.SetBool("on", true);
    }

    public void HideMiniMap()
    {
        miniMapSmall.SetBool("on", true);
        miniMapBig.SetBool("on", false);
    }


}


