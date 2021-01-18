using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass
{
    GrassType type;

    public GrassType Type { get => type; }

    public Grass(GrassType type)
    {
        this.type = type;
    }

    public override string ToString()
    {
        return "Grass: " + type.ToString();
    }
}

public enum GrassType
{
    Type1,
    Type2,
    Type3,
    Type4
}
