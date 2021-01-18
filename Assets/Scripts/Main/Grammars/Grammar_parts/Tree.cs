using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree 
{
    TreeType type;

    public TreeType Type { get => type; }

    public Tree(TreeType type)
    {
        this.type = type;
    }

    public override string ToString()
    {
        return "Tree: " + type.ToString();
    }
}

public enum TreeType
{
    Type1,
    Type2,
    Type3,
    Type4
}
