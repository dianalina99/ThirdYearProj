using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorRenderer : MonoBehaviour
{
    public Transform treeStructPrefab;
    public Transform[] treesPrefab;
    public Transform[] grassPrefab;
    public Transform[] randObjPrefab;
    Transform treeStructFolder;

    public void Render(TreeStruct treeStruct)
    {
        treeStructFolder = new GameObject("Tree").transform;
        treeStructFolder.position = new Vector3(9, 9, 0);

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
        //Transform grassFolder = new GameObject("Grass").transform;
        //grassFolder.SetParent(parent);
        Transform temp = Instantiate(treesPrefab[(int)tree.Type], new Vector3((float)Random.Range(0, 20) / 10.0f, (float)Random.Range(0, 20) / 10.0f, parent.transform.position.z), Quaternion.identity);

        temp.SetParent(parent);
    }

    private void RenderGrass(Grass grass, Transform parent)
    {
        //Transform grassFolder = new GameObject("Grass").transform;
        //grassFolder.SetParent(parent);
        Transform temp = Instantiate(grassPrefab[(int)grass.Type],new Vector3((float)Random.Range(0, 20)/10.0f, -(float)Random.Range(0, 10)/10.0f, parent.transform.position.z), Quaternion.identity);
        
        temp.SetParent(parent);
    }



    public void RenderRandObjStruct(RandObjStruct rand, Transform parent)
    {
        Transform randomObjFolder = new GameObject("RandObjStruct").transform;
        randomObjFolder.SetParent(parent);

        foreach (Grass gr in rand.Grass)
        {
            RenderGrass(gr, randomObjFolder);
        }
        RenderRandObj(rand.Rand, randomObjFolder);
    }

    public void RenderRandObj(RandObj rand, Transform parent)
    {
        Transform temp = Instantiate(randObjPrefab[(int)rand.Type], new Vector3((float)Random.Range(0, 20) / 10.0f, -(float)Random.Range(0, 10) / 10.0f, parent.transform.position.z), Quaternion.identity);
        temp.SetParent(parent);
    }






}
