using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private float[,] map; 
    private HashSet<Vector2> entries;

    private int type;

    public Room(int index)
    {
        this.type = index;
        this.map = new float[8, 10]{
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
                            };
        this.entries = new HashSet<Vector2>();
    }

    public HashSet<Vector2> getEntries()
    {
        return entries;
    }

    public bool addEntry(Vector2 pos)
    {
        //Check if entry already exists.
        if(this.entries.Contains(pos))
        {
            return false;
        }

        this.entries.Add(pos);
        return true;
    }

    public int getType()
    {
        return this.type;
    }

    public int getNoOfEntries()
    {
        return this.entries.Count;
    }

    public void setMap(int x, int y, float value)
    {
        this.map[x, y] = value;
    }

    public float getMapValue(int x, int y)
    {
        return this.map[x, y];
    }

    /*
    public void GenerateRoom(bool is_first)
    {
        if(!is_first)
        {
            switch (type)
            {
                case 1:
                    entries.AddRange(getEntries())
                    
                   
            }
        }

    } */
}
