using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass
{
    GrassType type;

    public GrassType Type { get => type; }

    public Grass()
    {
        this.type = DecorGenerator.GetRandomEnum<GrassType>();
    }

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
    Type4,
    Type5,
    Type6,
    Type7,
    Type8,
    Type9,
    Type10
}
