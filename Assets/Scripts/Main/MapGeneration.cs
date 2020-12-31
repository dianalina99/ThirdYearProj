﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using Extreme.Statistics.Distributions;

public class MapGeneration : MonoBehaviour
{
    private System.Random rand = new System.Random();
    private int roomWidth = 10, roomHeight = 10;

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

    private int noOfRooms = 1, roomPos, row, column;
    private bool stopCondition;
    private Vector2 firstRoomPos;

    public GameObject[] rooms;
    public GameObject player, environment, parent, baseRoom;
    public int minNoRooms;

    private Dictionary<Vector2, GameObject> gameMap = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, Room> gameObjMap = new Dictionary<Vector2, Room>();

    private void DrawMap()
    {
        GameObject playerInst, room;

        for (int i = 0; i <= 3; i++)
            for (int j = 0; j <= 3; j++)
            {
                DrawAndGenerateRoom(map[i, j], i * roomHeight, j * roomWidth);
            }

        //Place player in first room.
        gameMap.TryGetValue(firstRoomPos, out room);

        playerInst = Instantiate(player, new Vector2(-firstRoomPos.x * roomWidth + 3, firstRoomPos.y), Quaternion.identity) as GameObject;
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

        //Instantiate as GameObject in the game.
        room = Instantiate(rooms[index], position, Quaternion.identity) as GameObject;
        room.transform.SetParent(parent.transform, false);

        //Generate a room type object in order to handle exits and entries.

        // gameObjMap is a collection of rooms. Each room has a type and an array of entries.
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

        /*
        float[,] layout =  new float[10, 10]{
                                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {-1, -1,-1, -1, -1, -1, -1, -1,- 1, -1}
                            };*/

        UpdateRoomBorder(room, newRoom.getMap());

    }


    private void UpdateRoomBorder(GameObject room, float[,] layout)
    {


        foreach (Transform child in room.transform)
        {
            //int i = (int)(4.5 - Mathf.Abs(child.localPosition.y));
            //int j = (int)(4.5 - Mathf.Abs(child.localPosition.x));

            int i = Mathf.Abs((int)(-4.5 + child.localPosition.y));
            int j = Mathf.Abs((int)(4.5 + child.localPosition.x));

            if (layout[i, j] == 0)
            {
                //Destroy child.
                Destroy(child.gameObject);
            }
        }

    }
    private void GenerateType1Entries(Room room)
    {
        Vector2 pos = room.getPosition();
        Room right, left;
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
        random = rand.Next(1, 10);
        
        while (room.getNoOfEntries() <= random || (room.entryL.Count < 2) || (room.entryR.Count < 2))
        {
            int pos1 = rand.Next(2, roomHeight-1);
            int pos2 = rand.Next(2, roomHeight-1);

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
        random = rand.Next(1, 12);

        while (room.getNoOfEntries() <= random || (room.entryL.Count < 2) || (room.entryR.Count < 2) || (room.entryD.Count < 2))
        {
            int pos1 = rand.Next(2, roomHeight -1);
            int pos2 = rand.Next(2, roomHeight - 1);
            int pos3 = rand.Next(2, roomWidth - 1);

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
        random = rand.Next(1, 12);

        while (room.getNoOfEntries() <= random || (room.entryL.Count < 2) || (room.entryR.Count < 2) || (room.entryU.Count < 2))
        {
            int pos1 = rand.Next(2, roomHeight - 1);
            int pos2 = rand.Next(2, roomHeight - 1);
            int pos3 = rand.Next(2, roomWidth - 1);

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

        //Look left and right, up, down to detect already existent entries.
        right = getRoom(new Vector2(pos.x - 1, pos.y));
        left = getRoom(new Vector2(pos.x + 1, pos.y));
        up = getRoom(new Vector2(pos.x, pos.y + 1));
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

        if (down != null)
        {
            //Create exit to be consistent with up room entry.
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
        random = rand.Next(1, 12);

        while (room.getNoOfEntries() <= random || (room.entryL.Count < 2) || (room.entryR.Count < 2) || (room.entryU.Count < 2) || (room.entryD.Count < 2))
        {
            int pos1 = rand.Next(2, roomHeight - 1);
            int pos2 = rand.Next(2, roomHeight - 1);
            int pos3 = rand.Next(2, roomWidth - 1);
            int pos4 = rand.Next(2, roomWidth - 1);

            //Mark entries. They should be 2 units tall because the player is 2 units tall.
            room.setMap(pos1, 0, 0);
            room.setMap(pos1 - 1, 0, 0);
            room.setMap(pos2, roomWidth - 1, 0);
            room.setMap(pos2 - 1, roomWidth - 1, 0);
            room.setMap(0, pos3, 0);
            room.setMap(0, pos3 - 1, 0);
            room.setMap(roomHeight -1, pos4, 0);
            room.setMap(roomHeight - 1, pos4 - 1, 0);

            //Add entries to specific side.
            room.addEntry(new Vector2(pos1, 0), room.entryL);
            room.addEntry(new Vector2(pos1 - 1, 0), room.entryL);
            room.addEntry(new Vector2(pos2, roomWidth - 1), room.entryR);
            room.addEntry(new Vector2(pos2 - 1, roomWidth - 1), room.entryR);
            room.addEntry(new Vector2(0, pos3), room.entryU);
            room.addEntry(new Vector2(0, pos3 - 1), room.entryU);
            room.addEntry(new Vector2(roomHeight - 1, pos4), room.entryD);
            room.addEntry(new Vector2(roomHeight - 1, pos4 - 1), room.entryD);
        }

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
    private void DrawRoom(int index, int x, int y)
    {
        GameObject room;

        Vector2 position = new Vector2(-y, -x);

        //Check if dictionary entry exists and remove room before overwriting.
        if (gameMap.ContainsKey(position))
        {
            gameMap.TryGetValue(position, out room);
            gameMap.Remove(position);
            Destroy(room);
        }

        //room = Instantiate(rooms[index], position, Quaternion.identity) as GameObject;
        room = Instantiate(baseRoom, position, Quaternion.identity) as GameObject;
        room.transform.SetParent(parent.transform, false);

        gameMap.Add(position, room);
    }

    private void InitializeGrid()
    {
        for (int i = 0; i <= 3; i++)
            for (int j = 0; j <= 3; j++)
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


        firstRoomPos = new Vector2(column, row);

        while (!stopCondition)
        {


            //Check if there are enough rooms generated.

            if (noOfRooms > minNoRooms && row == 3)
            {
                stopCondition = true;
            }

            //Check where we came from.
            if (map[row, column] == 0)
            {
                map[row, column] = 1;
            }

            /*
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

            }*/

            if ((prevMove[row, column] == 3 && map[row, column] == 2) || (prevMove[row, column] == 2 && map[row, column] == 3))
            {
                map[row, column] = 4;
            }
            else if ((prevMove[row, column] == 3 || prevMove[row, column] == 2) && (map[row, column] == 1 || map[row, column] == 0))
            {
                map[row, column] = prevMove[row, column];
            }



            /*Choose moving direction:
            *
            * 0 = left
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
                        if (map[row, column - 1] == 0)
                        {
                            noOfRooms++;
                        }

                        column--;
                        prevMove[row, column] = dir;


                    }

                    break;

                case 1:
                    //Move to the right.
                    if (CanMove(row, column + 1))
                    {
                        //Check if room was already used.
                        if (map[row, column + 1] == 0)
                        {
                            noOfRooms++;
                        }

                        column++;
                        prevMove[row, column] = dir;


                    }

                    break;

                case 2:
                    //Change room type to have a top exit and move up.
                    if (CanMove(row - 1, column))
                    {
                        //Check if next room was already used.
                        if (map[row - 1, column] != 0)
                        {
                            if (map[row - 1, column] == 3 || map[row - 1, column] == 4)
                            {
                                //If prev room already has a top exit, add a bottom one.
                                map[row - 1, column] = 4;
                            }
                            else
                            {
                                map[row - 1, column] = 2;
                            }

                        }
                        else
                        {
                            noOfRooms++;
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



                    }

                    break;

                case 3:
                    //Change room type to have a bottom exit and move down.
                    if (CanMove(row + 1, column))
                    {
                        //Check if next room was already used.
                        if (map[row + 1, column] != 0)
                        {
                            //if (prevMove[row + 1, column] == 2)
                            if (map[row + 1, column] == 2 || map[row + 1, column] == 4)
                            {
                                map[row + 1, column] = 4;
                            }
                            else
                            {
                                map[row + 1, column] = 3;
                            }

                        }
                        else
                        {
                            noOfRooms++;
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
        if (new Vector2(i, j) == firstRoomPos)
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