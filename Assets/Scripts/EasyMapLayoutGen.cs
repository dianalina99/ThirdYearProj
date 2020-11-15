using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EasyMapLayoutGen : MonoBehaviour
{ 
    // Start is called before the first frame update
    int[,] map = new int[4, 4]{
                                {0, 0, 0, 0},
                                {0, 0, 0, 0},
                                {0, 0, 0, 0},
                                {0, 0, 0, 0}
                            };
    private int noOfRooms = 0, roomPos, row, column, prevMove = -1;
    private bool stopCondition;

    void Start()
    {
        System.Random rand = new System.Random();

        //Pick first room. It is always 1.
        roomPos = rand.Next(0, 4);
        row = 0;
        column = roomPos;


        while (!stopCondition)
        {
            //Check if we reached the bottom floor room.
            if (row == 3)
            {
                stopCondition = true;
            }

            //Initialize room with 1.
            map[row, column] = 1;

            //Check where we came from.
            if (prevMove == 3 || prevMove == 2)
            {
                map[row, column] = prevMove;
            }

            /* Choose moving direction:
             * 
             * 0 = left
             * 1 = right
             * 2 = up
             * 3 = down
             */

            int dir = rand.Next(0, 4);

            switch (dir)
            {
                case 0:
                    //Move to the left. Perform checks first.
                    if (CanMove(row, column - 1))
                    {
                        prevMove = dir;
                        column--;
                        noOfRooms++;
                    }

                    break;

                case 1:
                    //Move to the right.
                    if (CanMove(row, column + 1))
                    {
                        prevMove = dir;
                        column++;
                        noOfRooms++;
                    }

                    break;

                case 2:
                    //Change room type to have a top exit and move up.
                    if (CanMove(row - 1, column))
                    {

                        //Check if current room already has a bottom exit.
                        if (map[row, column] == 2)
                        {
                            map[row, column] = 4;
                        }
                        else
                        {
                            map[row, column] = 3;
                        }

                        prevMove = dir;
                        row--;
                        noOfRooms++;
                    }

                    break;

                case 3:
                    //Change room type to have a bottom exit and move down.
                    if (CanMove(row + 1, column))
                    {


                        //Check if room already has a top exit.
                        if (map[row, column] == 3)
                        {
                            map[row, column] = 4;
                        }
                        else
                        {
                            map[row, column] = 2;
                        }

                        prevMove = dir;
                        row++;
                        noOfRooms++;
                    }

                    break;

                default:
                    //Don't move at all.
                    break;

            }
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


        if (map[i, j] != 0)
        {
            //We've been here before, so don't pass twice.
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

    // Update is called once per frame
    void Update()
    {

    }
}