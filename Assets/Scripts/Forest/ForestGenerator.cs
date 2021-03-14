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
    public GameObject portalToDungeonPrefab;
    public GameObject forestEntrancePrefab;

    private GameObject entryRef;
    private GameObject exitRef;


    private int[,] map;

    // Start is called before the first frame update
    void Start()
    { 
        //Spawn player as first thing when hitting play - it'll be moved after.
        GameManagerScript.instance.playerRef = Instantiate(GameManagerScript.instance.playerPrefab, new Vector3(55, 55, 0), Quaternion.identity) as GameObject;
        GameManagerScript.instance.playerRef.transform.SetParent(this.transform, false);


        GameManagerScript.instance.Reset();
        GameManagerScript.instance.forestMapRef = this.gameObject;
        GameManagerScript.instance.forestInUse = true;
        GameManagerScript.instance.forestNeedsRegeneration = true;

    }

    private void GenerateAllForest()
    {
        
        Reset();
        GameManagerScript.instance.forestInUse = true;
        GenerateMap();

        

        //Place entry and exit points.
        exitRef = Instantiate(this.portalToDungeonPrefab, new Vector3(50, 50, 0), Quaternion.identity) as GameObject;
        exitRef.transform.SetParent(this.transform, false);

        //Spawn entry point portal and save it as lastest entry point.
        entryRef = Instantiate(this.forestEntrancePrefab, new Vector3(60, 50, 0), Quaternion.identity) as GameObject;
        entryRef.transform.SetParent(this.transform, false);
        GameManagerScript.instance.latestPlayerEntryPoint = entryRef;

        //Mark map generation as done.
        GameManagerScript.instance.forestReadyForPlayer = true;

        Debug.Log("Forest generated, ready for player...");

    }

    public void Reset()
    {
        //Delete entry and exits.
        GameObject.Destroy(entryRef);
        GameObject.Destroy(exitRef);

        GameManagerScript.instance.Reset();
    }

    private void Update()
    {
        if(GameManagerScript.instance.forestInUse && GameManagerScript.instance.forestNeedsRegeneration)
        {
            Debug.Log("Generating forest...");

            GameManagerScript.instance.forestNeedsRegeneration = false;
            GenerateAllForest();
        }
    }

    private void GenerateMap()
    {
        map = new int[width, height];
        map = ApplyCellularAutomata(GenerateNoiseGrid(map, noiseDensity), iterationsCount, majority);

        VoxelGenerator voxGen = GetComponent<VoxelGenerator>();
        voxGen.GenerateMesh(map, 2);
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

    /*
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
    }*/

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
