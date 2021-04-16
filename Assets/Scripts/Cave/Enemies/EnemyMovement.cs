using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public Transform graphics;
    public float triggerAreaRadius;

    private bool hasTarget = false;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachEndOfPath = false;
    private Seeker seeker;
    private Rigidbody2D rb;

    private float distanceToPlayer;


    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        //Update the path of the enemy every half a second.
        InvokeRepeating("UpdatePath", 0f, .5f);

        distanceToPlayer = 100;
    }

    void UpdatePath()
    {
        if(GameManagerScript.instance.dungeonInUse && this.hasTarget)
        {
            //Calculate distance to player.
            distanceToPlayer = Vector3.Distance(rb.transform.position, target.position);

            if (seeker.IsDone() && distanceToPlayer <= triggerAreaRadius)
            {
                //AstarPath.active.Scan();
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }
        
    }

    void OnPathComplete(Path path)
    {
        if(!path.error)
        {
            this.path = path;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(GameManagerScript.instance.dungeonInUse)
        {
            if (!hasTarget)
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
                hasTarget = true;
            }

            if (path == null)
            {
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachEndOfPath = true;
                return;
            }
            else
            {
                reachEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (force.x >= 0.01f)
            {
                graphics.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (force.x <= -0.01f)
            {
                graphics.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
