﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using Extreme.Statistics.Distributions;

public class MapLayoutGeneration : MonoBehaviour
{
    private System.Random rand = new System.Random();
    private int roomWidth = 10, roomHeight = 8;

    // This derived class converts the uniformly distributed random
    // numbers generated by base.Sample() to another distribution.
    public class RandomProportional : System.Random
    {
        // The Sample method generates a distribution proportional to the value
        // of the random numbers, in the range [0.0, 1.0].
        protected override double Sample()
        {
            // return base.Sample() - System.Math.Sqrt(base.Sample());
            return System.Math.Sqrt(base.Sample());
        }

    }


    int[,] map = new int[4, 4]{
                                {0, 0, 0, 0},
                                {0, 0, 0, 0},
                                {0, 0, 0, 0},
                                {0, 0, 0, 0}
                            };

    int[,] prevMove = new int[4, 4]{
                                {-1, -1, -1, -1},
                                {-1, -1, -1, -1},
                                {-1, -1, -1, -1},
                                {-1, -1, -1, -1}
                            };

    private int noOfRooms = 0, roomPos, row, column;
    private bool stopCondition;
    private Vector2 firstRoomPos;

    public GameObject[] rooms;
    public GameObject player, environment, parent;

    private Dictionary<Vector2, GameObject> gameMap = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, Room> gameObjMap = new Dictionary<Vector2, Room>();

    private void DrawMap()
    {
        GameObject playerInst, room;
        for(int i=0; i<=3; i++)
            for(int j=0; j<=3; j++)
            {
                DrawAndGenerateRoom(map[i, j], i*roomHeight, j*roomWidth);
            }

        //Place player in first room.
        gameMap.TryGetValue(firstRoomPos, out room);
        playerInst = Instantiate( player, new Vector2(-131- firstRoomPos.y*roomHeight, 96), Quaternion.identity) as GameObject;
        playerInst.transform.SetParent(environment.transform, false);


    }

    private Room getRoom(Vector2 pos)
    {
        Room room = null;

        if (gameObjMap.ContainsKey(pos))
        {
            gameObjMap.TryGetValue(pos, out room);

        }

        return room;   
    }

    private void GenerateType1Entries(Room room)
    {
        Vector2 pos = room.getPosition();
        Room right, left, up, down;
        int random;

        //Room right, left;
        //Look left and right to detect already existent entries.
        right = getRoom(new Vector2(pos.x - 1, pos.y));
        left = getRoom(new Vector2(pos.x + 1, pos.y));

        if (right != null)
        {
            //Create exit to be consistent with right room entry.
            foreach (Vector2 entry in right.entries)
            {
                if (entry.y == 0)
                {
                    //Add new "entry" to the right.
                    room.setMap((int)entry.x, roomWidth - 1, 0);
                    room.addEntry(new Vector2((int)entry.x, roomWidth - 1), room.entryR);
                }

            }
        }

        if (left != null)
        {
            //Create entry to be consistent with left room exit.
            foreach (Vector2 entry in left.entries)
            {
                if (entry.y == roomWidth - 1)
                {
                    //Add new entry to the left.
                    room.setMap((int)entry.x, 0, 0);
                    room.addEntry(new Vector2((int)entry.x, 0), room.entryL);
                }
            }
        }

        //We want to have at least 1 entries/exits (of size 2) on each relevant side. Max is 12.
        random = rand.Next(1, 12);

        while (room.getNoOfEntries() <= random || (room.entryL.Count < 2) || (room.entryR.Count < 2))
        {
            int pos1 = rand.Next(2, 6);
            int pos2 = rand.Next(2, 6);

            //Mark entries. They should be 2 units tall because the player is 2 units tall.
            room.setMap(pos1, 0, 0);
            room.setMap(pos1 - 1, 0, 0);
            room.setMap(pos2, roomWidth - 1, 0);
            room.setMap(pos2 - 1, roomWidth - 1, 0);


            //Add entries to specific side.
            room.addEntry(new Vector2(pos1, 0), room.entryL);
            room.addEntry(new Vector2(pos1 - 1, 0), room.entryL);
            room.addEntry(new Vector2(pos2, roomWidth - 1), room.entryR);
            room.addEntry(new Vector2(pos2 - 1, roomWidth - 1), room.entryR);

        }

    }

    private void GenerateType2Entries(Room room)
    {
        Vector2 pos = room.getPosition();
        Room right, left, up, down;
        int random;

        //Look left and right to detect already existent entries.
        right = getRoom(new Vector2(pos.x - 1, pos.y));
        left = getRoom(new Vector2(pos.x + 1, pos.y));
        down = getRoom(new Vector2(pos.x, pos.y - 1));

        if (right != null)
        {
            //Create exit to be consistent with right room entry.
            foreach (Vector2 entry in right.entries)
            {
                if (entry.y == 0)
                {
                    room.setMap((int)entry.x, roomWidth - 1, 0);
                    room.addEntry(new Vector2((int)entry.x, roomWidth - 1), room.entryR);
                }

            }
        }

        if (left != null)
        {
            //Create entry to be consistent with right room exit.
            foreach (Vector2 entry in left.entries)
            {
                if (entry.y == roomWidth - 1)
                {
                    room.setMap((int)entry.x, 0, 0);
                    room.addEntry(new Vector2((int)entry.x, 0), room.entryL);
                }
            }
        }

        if (down != null)
        {
            //Create exit to be consistent with bottom room entry.
            foreach (Vector2 entry in down.entries)
            {
                if (entry.x == 0)
                {
                    room.setMap(roomHeight - 1, (int)entry.y, 0);
                    room.addEntry(new Vector2(roomHeight - 1, (int)entry.y), room.entryD);
                }
            }
        }

        //We want to have at least 1 entries/exits on each relevant side. Max is 18.
        random = rand.Next(1, 18);

        while (room.getNoOfEntries() <= random || (room.entryL.Count < 2) || (room.entryR.Count < 2) || (room.entryD.Count < 2))
        {
            int pos1 = rand.Next(2, 6);
            int pos2 = rand.Next(2, 6);
            int pos3 = rand.Next(2, 8);

            //Mark entries. They should be 2 units tall because the player is 2 units tall.
            room.setMap(pos1, 0, 0);
            room.setMap(pos1 - 1, 0, 0);
            room.setMap(pos2, roomWidth - 1, 0);
            room.setMap(pos2 - 1, roomWidth - 1, 0);
            room.setMap(roomHeight - 1, pos3, 0);
            room.setMap(roomHeight - 1, pos3 - 1, 0);

            //Add entries to specific side.
            room.addEntry(new Vector2(pos1, 0), room.entryL);
            room.addEntry(new Vector2(pos1 - 1, 0), room.entryL);
            room.addEntry(new Vector2(pos2, roomWidth - 1), room.entryR);
            room.addEntry(new Vector2(pos2 - 1, roomWidth - 1), room.entryR);
            room.addEntry(new Vector2(roomHeight - 1, pos3), room.entryD);
            room.addEntry(new Vector2(roomHeight - 1, pos3 - 1), room.entryD);

        }

    }

    private void GenerateType3Entries(Room room)
    {
        Vector2 pos = room.getPosition();
        Room right, left, up, down;
        int random;

        //Look left and right to detect already existent entries.
        right = getRoom(new Vector2(pos.x - 1, pos.y));
        left = getRoom(new Vector2(pos.x + 1, pos.y));
        up = getRoom(new Vector2(pos.x, pos.y + 1));

        if (right != null)
        {
            //Create exit to be consistent with right room entry.
            foreach (Vector2 entry in right.entries)
            {
                if (entry.y == 0)
                {
                    room.setMap((int)entry.x, roomWidth - 1, 0);
                    room.addEntry(new Vector2((int)entry.x, roomWidth - 1), room.entryR);
                }

            }
        }

        if (left != null)
        {
            //Create entry to be consistent with left room exit.
            foreach (Vector2 entry in left.entries)
            {
                if (entry.y == roomWidth - 1)
                {
                    room.setMap((int)entry.x, 0, 0);
                    room.addEntry(new Vector2((int)entry.x, 0), room.entryL);
                }
            }
        }

        if (up != null)
        {
            //Create exit to be consistent with bottom room entry.
            foreach (Vector2 entry in up.entries)
            {
                if (entry.x == roomHeight - 1)
                {
                    room.setMap(0, (int)entry.y, 0);
                    room.addEntry(new Vector2(0, (int)entry.y), room.entryU);
                }
            }
        }

        //We want to have at least 1 entries/exits on each relevant side. Max is 18.
        random = rand.Next(1, 18);

        while (room.getNoOfEntries() <= random || (room.entryL.Count < 2) || (room.entryR.Count < 2) || (room.entryU.Count < 2))
        {
            int pos1 = rand.Next(2, 6);
            int pos2 = rand.Next(2, 6);
            int pos3 = rand.Next(2, 8);

            //Mark entries. They should be 2 units tall because the player is 2 units tall.
            room.setMap(pos1, 0, 0);
            room.setMap(pos1 - 1, 0, 0);
            room.setMap(pos2, roomWidth - 1, 0);
            room.setMap(pos2 - 1, roomWidth - 1, 0);
            room.setMap(0, pos3, 0);
            room.setMap(0, pos3 - 1, 0);

            //Add entries to specific side.
            room.addEntry(new Vector2(pos1, 0), room.entryL);
            room.addEntry(new Vector2(pos1 - 1, 0), room.entryL);
            room.addEntry(new Vector2(pos2, roomWidth - 1), room.entryR);
            room.addEntry(new Vector2(pos2 - 1, roomWidth - 1), room.entryR);
            room.addEntry(new Vector2(0, pos3), room.entryU);
            room.addEntry(new Vector2(0, pos3 - 1), room.entryU);
        }
    }
    private void GenerateType4Entries(Room room)
    {
        Vector2 pos = room.getPosition();
        Room right, left, up, down;
        int random;

    }
    private void GenerateRoomTemplate(Room room)
    {
        //Only generate exits and entries for now, everything else should be 1.
        switch (room.getType())
        {
            
            case 0:
                break;
            case 1:
               GenerateType1Entries(room);
                break;
            case 2:
                GenerateType2Entries(room);
                break;
            case 3:
                GenerateType3Entries(room);
                break;
            case 4:
                GenerateType4Entries(room);
                break;

        }
    }


    private void DrawAndGenerateRoom(int index, int x, int y)
    {
        GameObject room, tile;
        Room roomObj;
        

        Vector2 position = new Vector2(-y, -x);

        //Check if dictionary entry exists and remove room before overwriting.
        if (gameMap.ContainsKey(position))
        {
            gameMap.TryGetValue(position, out room);
            gameMap.Remove(position);
            Destroy(room);
        }

        
        //gameObjMap is a collection of rooms. Each room has a type and an array of entries.
        //Entries = point where the player moves out/in the room = entries + exits
        if (gameObjMap.ContainsKey(position))
        {
            gameObjMap.TryGetValue(position, out roomObj);
            gameObjMap.Remove(position);
            roomObj = null;

        }

        //Create new room template.
        Room newRoom = new Room(index, position);
        GenerateRoomTemplate(newRoom);

        gameObjMap.Add(position, newRoom);

        //Instantiate as GameObject in the game.
        //room = Instantiate(rooms[index], position, Quaternion.identity) as GameObject;
        //room.transform.SetParent(parent.transform, false);


        //Instantiate tiles for room
        //i = x
        //j = y
        /*
        foreach(Vector2 entry in newRoom.entries)
        {
            Vector2 pos = new Vector2(position.x + entry.x, position.y - entry.y);
            tile = Instantiate(rooms[5], pos, Quaternion.identity) as GameObject;
            tile.transform.SetParent(parent.transform, false);
        }*/

        int i = 0, j = 0;
        foreach(float entry in newRoom.getMap())
        {
            i++; j++;

            if(entry == -1)
            {
                Vector2 pos = new Vector2(position.x + i, position.y - j);
                tile = Instantiate(rooms[5], pos, Quaternion.identity) as GameObject;
                tile.transform.SetParent(parent.transform, false);

            }
        }

        /*
        for(int i=0; i<roomWidth;i++)
            for(int j = 0; j< roomHeight; j++)
            {
                if(!newRoom.entries.Contains(new Vector2(i,j)))
                {
                    Vector2 pos = new Vector2(position.x + i, position.y - j);
                    tile = Instantiate(rooms[5], pos, Quaternion.identity) as GameObject;
                    tile.transform.SetParent(parent.transform, false);
                }        
            }*/

        /*
        Vector2 pos = new Vector2(position.x+1 , position.y-1 );
        tile = Instantiate(rooms[5], pos, Quaternion.identity) as GameObject;
        tile.transform.SetParent(parent.transform, false); */

        //gameMap.Add(position, room);

    }

    private void DrawRoom(int index, int x, int y)
    {
        GameObject room;
    
        Vector2 position = new Vector2(-y, -x);

        //Check if dictionary entry exists and remove room before overwriting.
        if(gameMap.ContainsKey( position ))
        {
            gameMap.TryGetValue( position, out room);
            gameMap.Remove(position);
            Destroy(room);
        }
      
        room = Instantiate(rooms[index], position, Quaternion.identity) as GameObject;
        room.transform.SetParent(parent.transform, false);

        gameMap.Add(position, room);
    }

    private void InitializeGrid()
    {   
        for(int i=0; i<=3;i++)
            for(int j=0; j<=3;j++)
            {
                //Draw room.
                DrawRoom(0, j * roomHeight, i * roomWidth);
               
            }
    }
    
    private void GenerateMapGrid()
    {
        //RandomProportional rand = new RandomProportional();
        

        InitializeGrid();


        //Pick first room. It is always 1.
        roomPos = rand.Next(0, 4);
        row = 0;
        column = roomPos;
        stopCondition = false;
        int prevRow;
        int prevColumn;

        firstRoomPos = new Vector2(row, column);

        while (!stopCondition)
        {

            prevRow = row;
            prevColumn = column;

            //Check if there are enough rooms generated.

            if (noOfRooms > 8 && row == 3)
            {
                stopCondition = true;
            }

            //Check where we came from.
            if (map[row, column] == 0)
            {
                map[row, column] = 1;
            }
            if (prevMove[row, column] == 3 || prevMove[row, column] == 2)
            {
                if (map[row, column] != 1 && map[row, column] != 0)
                {
                    map[row, column] = 4;
                }
                else
                {
                    map[row, column] = prevMove[row, column];
                }

            }


            /*Choose moving direction:
            *
            *0 = left
            * 1 = right
            * 2 = up
            * 3 = down*/

            int dir = rand.Next(0, 4);

            switch (dir)
            {
                case 0:
                    //Move to the left. Perform checks first.
                    if (CanMove(row, column - 1))
                    {
                        column--;
                        prevMove[row, column] = dir;

                        noOfRooms++;
                    }

                    break;

                case 1:
                    //Move to the right.
                    if (CanMove(row, column + 1))
                    {
                        column++;
                        prevMove[row, column] = dir;

                        noOfRooms++;
                    }

                    break;

                case 2:
                    //Change room type to have a top exit and move up.
                    if (CanMove(row - 1, column))
                    {
                        //Check if next room was already used.
                        if (map[row - 1, column] != 0)
                        {
                            if (prevMove[row - 1, column] == 3)
                            {
                                //If prev room already has a top exit, add a bottom one.
                                map[row - 1, column] = 4;
                            }
                            else
                            {
                                map[row - 1, column] = 2;
                            }

                        }

                        //Check if current room already has a bottom exit.
                        if (map[row, column] == 2 || map[row, column] == 4)
                        {
                            map[row, column] = 4;
                        }
                        else
                        {
                            map[row, column] = 3;
                        }

                        row--;
                        prevMove[row, column] = dir;

                        noOfRooms++;

                    }

                    break;

                case 3:
                    //Change room type to have a bottom exit and move down.
                    if (CanMove(row + 1, column))
                    {
                        //Check if next room was already used.
                        if (map[row + 1, column] != 0)
                        {
                            if (prevMove[row + 1, column] == 2)
                            {
                                map[row + 1, column] = 4;
                            }
                            else
                            {
                                map[row + 1, column] = 3;
                            }

                        }

                        //Check if room already has a top exit.
                        if (map[row, column] == 3 || map[row, column] == 4)
                        {
                            map[row, column] = 4;
                        }
                        else
                        {
                            map[row, column] = 2;
                        }

                        row++;
                        prevMove[row, column] = dir;

                        noOfRooms++;
                    }

                    break;

                default:
                    //Don't move at all.
                    break;
            }
        }
        PrintMap();
        DrawMap();
    }


    void Start()
    {
        GenerateMapGrid();
        
    }

    private bool CanMove(int i, int j)
    {
        if (i < 0 || i > 3 || j < 0 || j > 3)
        {
            //Ooops, out of the map.
            return false;
        }

        //Stop if next room is first room.
        if(new Vector2(i,j) == firstRoomPos)
        {
            return false;
        }

        return true;
    }

    void PrintMap()
    {
        using (StreamWriter writer = new StreamWriter("C:\\Users\\diana\\Desktop\\3rdYearProj\\ThirdYearProj\\map.txt"))
        {
            //Print map.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    writer.Write(map[i, j] + ", ");
                }
                writer.WriteLine();

            }
        }
    }

    
}