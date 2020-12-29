using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject[] objects;
    private int[][] room;

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, objects.Length);
        GameObject tile = Instantiate(objects[rand], transform.position, Quaternion.identity) as GameObject;
        tile.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
