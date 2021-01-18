using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandObj
{
    RandObjType type;

    public RandObjType Type { get => type; }

    public RandObj(RandObjType type)
    {
        this.type = type;
    }

    public override string ToString()
    {
        return "RandObj: " + type.ToString();
    }

}

public enum RandObjType
{
    Type1,
    Type2,
    Type3,
    Type4
}

