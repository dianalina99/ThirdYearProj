using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStruct 
{
    Vector2Int size;
    Tree[] trees;
    Grass[] grass;
    RandObjStruct[] randObj;
    

    public Vector2Int Size { get { return size; } }
    public Grass[] GrassStruct { get { return grass; } }
    public RandObjStruct[] RandObjStruct { get { return randObj; } }
    public Tree[] Trees { get => trees; }

    public TreeStruct(int sizeX, int sizeY, Tree[] trees, Grass[] grass, RandObjStruct[] randObj)
    {
        size = new Vector2Int(sizeX, sizeY);
        this.grass = grass;
        this.randObj = randObj;
        this.trees = trees;
    }

    public override string ToString()
    {
        string tree = "TreeStruct:(" + size.ToString() + "; grass = " + grass.Length + "; randObj = " + randObj.Length + ")\n";

        foreach (Tree tr in trees)
        {
            tree += "\t" + tr.ToString() + "\n";
        }

        foreach (Grass gr in grass)
        {
            tree += "\t" + gr.ToString() + "\n";
        }

        foreach (RandObjStruct ra in randObj)
        {
            tree += "\t" + ra.ToString() + "\n";
        }

        return tree;
    }
}
