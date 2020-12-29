using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private float[,] map; 
    private Vector2 position;
    private int type;

    public HashSet<Vector2> entryL;
    public HashSet<Vector2> entryR;
    public HashSet<Vector2> entryU;
    public HashSet<Vector2> entryD;
    public HashSet<Vector2> entries;

    public Room(int index, Vector2 pos)
    {
        this.type = index;
        this.map = new float[10, 10]{
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
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
        this.entryR = new HashSet<Vector2>();
        this.entryL = new HashSet<Vector2>();
        this.entryU = new HashSet<Vector2>();
        this.entryD = new HashSet<Vector2>();
        this.position = pos;

        
    }
    public float[,] getMap()
    {
        return this.map;
    }

    public Vector2 getPosition()
    {
        return this.position;
    }

    public void setPosition(Vector2 newpos)
    {
        this.position = newpos;
    }

    public bool addEntry(Vector2 pos, HashSet<Vector2> entry)
    {
        //Check if entry already exists.
        if(entry.Contains(pos) || this.entries.Contains(pos))
        {
            return false;
        }

        entry.Add(pos);
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

}
