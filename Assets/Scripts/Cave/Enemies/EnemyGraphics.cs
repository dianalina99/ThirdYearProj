using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyGraphics : MonoBehaviour
{
    // Start is called before the first frame update
    public AIPath aiPath;
    public AIDestinationSetter destination;
    private bool hasTarget = false;


    // Update is called once per frame
    void Update()
    {
        if(!hasTarget)
        {
            destination.target = GameObject.FindGameObjectWithTag("Player").transform;
            hasTarget = true;
        }

        if(aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
