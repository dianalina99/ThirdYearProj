using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 5);
    }
}
