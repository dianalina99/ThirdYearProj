using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TreeStruct tree = DecorGenerator.GenerateTreeStruct();
        GetComponent<DecorRenderer>().Render(tree);
        Debug.Log(tree.ToString()); 
    }

}
