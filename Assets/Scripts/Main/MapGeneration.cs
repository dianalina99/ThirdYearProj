using System.Collections;
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
    public GameObject player, environment, parent;
    public int minNoRooms;

    private Dictionary<Vector2, GameObject> gameMap = new Dictionary<Vector2, GameObject>();
    

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
 
        playerInst = Instantiate(player, new Vector2( -firstRoomPos.x * roomWidth + 3, firstRoomPos.y), Quaternion.identity) as GameObject;
        playerInst.transform.SetParent(environment.transform, false);
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

        float[,] layout =  new float[10, 10]{
                                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, -1, -1, -1, -1, -1, -1, -1, -1, 1},
                                {1, 1,1, 1, 1, 1, 1, 1,1, 1}
                            };

        UpdateRoomBorder(room, layout);
        

    }
    private void UpdateRoomBorder(GameObject room, float[,] layout)
    {
 

        foreach(Transform child in room.transform)
        {
            //int i = (int)(4.5 - Mathf.Abs(child.localPosition.y));
            //int j = (int)(4.5 - Mathf.Abs(child.localPosition.x));

            int i = Mathf.Abs((int)(-4.5 + child.localPosition.y));
            int j = Mathf.Abs((int)(4.5 + child.localPosition.x));

            if (layout[i,j] == -1)
            {
                //Destroy child.
                Destroy(child.gameObject);
            }
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

        room = Instantiate(rooms[index], position, Quaternion.identity) as GameObject;
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

            if((prevMove[row,column] == 3 && map[row, column] == 2) || (prevMove[row, column] == 2 && map[row, column] == 3))
            {
                map[row, column] = 4;
            }
            else if((prevMove[row, column] == 3 || prevMove[row, column] == 2) && (map[row, column] == 1 || map[row, column] == 0))
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
