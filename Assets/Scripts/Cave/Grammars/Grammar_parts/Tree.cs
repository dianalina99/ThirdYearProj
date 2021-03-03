using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree 
{
    TreeType type;
    float radius;

    public TreeType Type { get => type; }

    public Tree()
    {
        this.type = DecorGenerator.GetRandomEnum<TreeType>();
        this.radius = 0.5f;
    }

    public Tree(TreeType type)
    {
        this.type = type;
        this.radius = 0.5f;
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


