using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int noiseDensity;

    public int width, height, iterationsCount, majority;
    public bool regenerate = false, useCustomSeed = false;
    public string seed;
    

    private int[,] map;
   
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if(regenerate)
        {
            regenerate = false;
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        map = new int[width, height];
        // map = GenerateNoiseGrid(noiseDensity);
        map = ApplyCellularAutomata(GenerateNoiseGrid(map, noiseDensity), iterationsCount, majority);
    }

    private int[,] GenerateNoiseGrid(int[,] noiseMap, int density)
    {
        //Check if we should use a custom seed. If not generate unique seed using time function.
        if (!useCustomSeed)
        {
            seed = Time.time.ToString();
        }

        //Trnasform string seed into random unique integer.
        System.Random randSeed = new System.Random(seed.GetHashCode());

        for(int x=0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int rand = randSeed.Next(0, 100);

                if (rand >= density)
                {
                    //Make tile 0.
                    noiseMap[x, y] = 0;

                }
                else
                {
                    //Make tile 1.
                    noiseMap[x, y] = 1;
                }

                
                //Check if tile is border - if true mark it as 1.
                if( x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    noiseMap[x, y] = 1;
                } 

            }

        }
            

        return noiseMap;
    }

    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f, 0 );
                    Gizmos.DrawCube(pos, Vector3.one);
                }

            }

        }
    }

    private bool IsInMapBounds(int x, int y)
    {
        if(x < 0 || y < 0 || x >= width || y >= height)
        {
            return false;
        }

        return true;
    }

    private int[,] ApplyCellularAutomata(int[,] noiseMap, int noOfIterations, int majority)
    {
        int[,] tempMap = new int[map.GetLength(0), map.GetLength(1)];

        for (int count = 0; count< noOfIterations; count ++)
        {
            //Create map placeholder so we don't get any bias by overwriting directly on main map.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tempMap[x, y] = noiseMap[x, y];
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Check Moore neighbours - if majority are ground, map[i][j] becomes ground. Same for wall.
                    int noOfNeighboursWalls = 0;

                    //Go through neighbours.
                    for (int nx = x - 1; nx <= x + 1; nx++)
                    {
                        for (int ny = y - 1; ny <= y + 1; ny++)
                        {
                            //Check if it's within map bounds.
                            if (IsInMapBounds(nx, ny))
                            {
                                //Check neighbour value. Don't count current tile as its own neighbour.
                                //if (tempMap[nx, ny] == 1 && (nx != x || ny != y))
                                if (nx != x || ny != y)
                                {
                                    noOfNeighboursWalls += tempMap[nx, ny];
                                }
                            }
                            else
                            {
                                //We want borders to be considered walls too.
                                noOfNeighboursWalls++;
                            }

                        }
                    }

                    //Assign tile to the same value as the majority of its neighbours.
                    if (noOfNeighboursWalls > majority)
                    {
                        noiseMap[x, y] = 1;
                    }
                    else if(noOfNeighboursWalls < majority)
                    {
                        noiseMap[x, y] = 0;
                    }
                }

            }
                
        }

        return noiseMap;
    }

    
}
