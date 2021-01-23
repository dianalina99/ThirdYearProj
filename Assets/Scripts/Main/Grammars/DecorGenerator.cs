using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorGenerator
{
    public static TreeStruct GenerateTreeStruct(TreeStructSettings settings)
    {
        return GenerateTreeStruct(settings.Size.x, settings.Size.y);
    }

    static TreeStruct GenerateTreeStruct(int sizeX, int sizeY)
    {
        return new TreeStruct(sizeX, sizeY, GenerateTrees(), GenerateGrassStruct(), GenerateRandObjStructs());
    }

    static Tree[] GenerateTrees()
    {
        return new Tree[] { GenerateTree()};
    }
    static Tree GenerateTree()
    {
        return new Tree();
    }

    static Grass[] GenerateGrassStruct()
    {
        return new Grass[] { GenerateGrass() };
    }

    static RandObjStruct[] GenerateRandObjStructs()
    {
        return new RandObjStruct[] { GenerateRandObjStruct() };
    }
    static RandObjStruct GenerateRandObjStruct()
    {
        return new RandObjStruct(GenerateGrassStruct(), GenerateRandObj());
    }

    static Grass GenerateGrass()
    {
        return new Grass();
    }

    static RandObj GenerateRandObj()
    {
        return new RandObj();
    }






    public static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }



}
