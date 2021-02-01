using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_hidden_room : MonoBehaviour
{
    public GameObject[] objects;
    public GameObject hiddenDoorPrefab;
  

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, objects.Length);
        GameObject tile = Instantiate(objects[rand], this.transform.position, Quaternion.identity) as GameObject;
        tile.transform.parent = this.transform;

        GameObject door = Instantiate(hiddenDoorPrefab, new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z) , Quaternion.identity) as GameObject;
        door.transform.parent = this.transform;




    }



}
