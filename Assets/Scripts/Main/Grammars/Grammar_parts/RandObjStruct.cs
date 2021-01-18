using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandObjStruct 
{
    Grass[] grass;
    RandObj rand;

    public Grass[] Grass { get => grass; }
    public RandObj Rand { get => rand; }

    public RandObjStruct(Grass[] grass, RandObj rand)
    {
        this.grass = grass;
        this.rand = rand;
    }


    public override string ToString()
    {
        string print = "RandObj:(" + "grass: " + grass.Length + "; " + rand.ToString() + ")\n";
        foreach (Grass gr in grass)
        {
            print +="\t\t" + gr.ToString() + "\n";
        }

        return print;
    }
}
