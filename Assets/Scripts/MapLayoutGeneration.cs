﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using Extreme.Statistics.Distributions;

public class MapLayoutGeneration : MonoBehaviour
{

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
    public GameObject parent;

    private Dictionary<Vector2, GameObject> gameMap = new Dictionary<Vector2, GameObject>();

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
                DrawRoom(0, j * 8, i * 10);
               
            }
    }
    



    void Start()
    {
        
        //RandomProportional rand = new RandomProportional();
        System.Random rand = new System.Random();

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
                //Check if we reached the bottom floor room.
                /* if (row == 3)
                 {
                     stopCondition = true;
                 }*/

                //Check if there are enough rooms generated.

                if (noOfRooms > 8 && row == 3)
            {
                stopCondition = true;
            }

            //Initialize room with 1.
            map[row, column] = 1;


            //Check where we came from.
            if (prevMove[row, column] == 3 || prevMove[row, column] == 2)
            {
                map[row, column] = prevMove[row, column];
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
                        if (map[row - 1, column] != 0 )
                        {
                            if( prevMove[row - 1, column] == 3)
                            {
                                map[row - 1, column] = 4;
                            }
                            else if (prevMove[row - 1, column] == 0 && prevMove[row - 1, column] == 1)
                            {
                                map[row - 1, column] = 2;
                            }
                       
                        }

                        //Check if current room already has a bottom exit.
                        if (map[row, column] == 2)
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
                            else if (prevMove[row + 1, column] == 0 && prevMove[row + 1, column] == 1)
                            {
                                map[row + 1, column] = 3;
                            }

                        }

                        //Check if room already has a top exit.
                        if (map[row, column] == 3)
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

            //Draw room.
            DrawRoom(map[prevRow, prevColumn], prevRow * 8, prevColumn * 10);
        }

        PrintMap();

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
