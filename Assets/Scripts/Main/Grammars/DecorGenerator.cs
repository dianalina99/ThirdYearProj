using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorGenerator
{
    public static TreeStruct GenerateTreeStruct()
    {
        return new TreeStruct(4, 4,
            new Tree[] {
                new Tree(TreeType.Type1)},
            new Grass[] {
                new Grass(GrassType.Type1), new Grass(GrassType.Type2) },
            new RandObjStruct[]
            {
                new RandObjStruct(
                   new Grass[]
                    {
                        new Grass(GrassType.Type1)
                    },
                   new RandObj(RandObjType.Type1)
                    )
            }) ;
    }
 
}
