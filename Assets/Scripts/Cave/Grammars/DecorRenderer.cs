using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorRenderer : MonoBehaviour
{
    public Transform[] treesPrefab;
    public Transform[] grassPrefab;
    public Transform[] randObjPrefab;
    private Transform treeStructFolder;
    private List<Transform> renderedObj = new List<Transform>();

    private bool makeStatic = true;
    private int iterations = 0;

    public void FixedUpdate()
    {

        if(makeStatic && iterations > 10)
        {
            //Make all rendered object as static rigid bodies.
            foreach (Transform temp in renderedObj)
            {
                if (temp.tag != "EmptyTile")
                {
                    temp.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    temp.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                }

            }

            makeStatic = false;
        }

        iterations++;
    }


    public void Render(TreeStruct treeStruct)
    {
        treeStructFolder = new GameObject("Tree").transform;
        treeStructFolder.SetParent(this.transform);
        treeStructFolder.localPosition = new Vector3(0, 0, 0);
        
        
        
        foreach (Tree tr in treeStruct.Trees)
        {
            RenderTree(tr, treeStructFolder);
        }

        foreach (Grass gr in treeStruct.GrassStruct)
        {
            RenderGrass(gr, treeStructFolder);
        }

        foreach (RandObjStruct ra in treeStruct.RandObjStruct)
        {
            RenderRandObjStruct(ra, treeStructFolder);
        }

    }

    private void RenderTree(Tree tree, Transform parent)
    {
        Transform temp = Instantiate(treesPrefab[(int)tree.Type], new Vector3(0,0,0), Quaternion.identity);
        temp.SetParent(parent);
        temp.localPosition = new Vector3((float)Random.Range(0, 20) / 10.0f, (float)Random.Range(0, 20) / 10.0f, parent.transform.position.z);

        //Add it to rendered objects.
        renderedObj.Add(temp);
    }

    private void RenderGrass(Grass grass, Transform parent)
    {
        Transform temp = Instantiate(grassPrefab[(int)grass.Type], new Vector3(0, 0, 0), Quaternion.identity);
        
        temp.SetParent(parent);
        temp.localPosition = new Vector3((float)Random.Range(0, 20) / 10.0f, -(float)Random.Range(0, 10) / 10.0f, parent.transform.position.z);

        //Add it to rendered objects.
        renderedObj.Add(temp);
    }



    public void RenderRandObjStruct(RandObjStruct rand, Transform parent)
    {
        Transform randomObjFolder = new GameObject("RandObjStruct").transform;
        randomObjFolder.SetParent(parent);
        randomObjFolder.localPosition = new Vector3(0, 0, 0);

        foreach (Grass gr in rand.Grass)
        {
            RenderGrass(gr, randomObjFolder);
        }

        RenderRandObj(rand.Rand, randomObjFolder);
    }

    public void RenderRandObj(RandObj rand, Transform parent)
    {
        Transform temp = Instantiate(randObjPrefab[(int)rand.Type], new Vector3(0,0,0), Quaternion.identity);
        temp.SetParent(parent);
        temp.localPosition = new Vector3((float)Random.Range(0, 20) / 10.0f, -(float)Random.Range(0, 10) / 10.0f, parent.transform.position.z);

        //Add it to rendered objects.
        renderedObj.Add(temp);
    }


    private bool CheckCollision(Transform obj)
    {
        BoxCollider2D objCollider = obj.gameObject.GetComponent<BoxCollider2D>();
        
        foreach (Transform temp in renderedObj)
        {
            if(temp.tag == "EmptyTile")
            {
                continue;
            }

            BoxCollider2D collider = temp.gameObject.GetComponent<BoxCollider2D>();

            //Check if it collides with 
            if(collider.IsTouching(objCollider) && (temp.tag == "DecorTile" || temp.tag == "WallTile"))
            {
                return true;
            }

        }

        return false;
    }

}
