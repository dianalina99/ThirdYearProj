using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script to spawn given object at a set probability (1/objects.length).
public class SpawnObject : MonoBehaviour
{
    public GameObject[] objects;
    private int[][] room;

    // Start is called before the first frame update
    void Awake()
    {
        int rand = Random.Range(0, objects.Length);
        GameObject tile = Instantiate(objects[rand], this.transform.position, Quaternion.identity) as GameObject;
        tile.transform.parent = this.transform;
    }


}
