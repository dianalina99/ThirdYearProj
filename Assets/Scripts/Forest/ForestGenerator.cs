using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ForestGrid
{
    private int[,] map;
    private List<Vector2> walkableArea;

    private string seed;

    ForestGrid left, right, up, down;


    public ForestGrid(string seed, int width, int height)
    {
        this.seed = seed;
        this.map = new int[width, height];
        this.walkableArea = new List<Vector2>();

        left = right = up = down = null;
    }

    public void SetAdjacentForestGrids(ForestGrid left, ForestGrid right, ForestGrid up, ForestGrid down)
    {
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;
    }

    public int[,] GetMap()
    {
        return this.map;
    }

    public void SetMap(int[,] givenMap)
    {
        this.map = givenMap;
    }

    public List<Vector2> GetWalkableArea()
    {
        return this.walkableArea;
    }

    public void SetWalkableArea(List<Vector2> givenArea)
    {
        this.walkableArea = givenArea;
    }

}

public class ForestGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int noiseDensity;

    public int width, height, iterationsCount, majority;
    public bool regenerate = false;

    public GameObject portalToDungeonPrefab;
    public GameObject forestEntrancePrefab;

    private GameObject entryRef;
    private GameObject exitRef;
    private Dictionary<string, ForestGrid> listOfForests;




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

        listOfForests = new Dictionary<string, ForestGrid>();
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
        if(GameManagerScript.instance.forestInUse && GameManagerScript.instance.forestNeedsRegeneration || regenerate)
        {
            regenerate = false;
            Debug.Log("Generating forest...");

            GameManagerScript.instance.forestNeedsRegeneration = false;
            GenerateAllForest();
        }
    }

    private void GenerateAllForest()
    {
        Reset();
        GameManagerScript.instance.forestInUse = true;
        GenerateMap(null);

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

    private void GenerateMap(string seed)
    {
        VoxelGenerator voxGen = GetComponent<VoxelGenerator>();

        //Check if we have to use custom seed.
        if (seed != null)
        {
            //Get forest and display it.
            ForestGrid forest;
            listOfForests.TryGetValue(seed, out forest);

            if(forest == null)
            {
                Debug.LogError("No forest exists with this seed!");
                return;
            }
            else
            {
                //Display forest on screen.
                voxGen.GenerateMesh(forest.GetMap(), 2);
            }
        }
        else
        {
            seed = Time.time.ToString();

            //If no custom seed was provided, generate new forest with new random seed.
            ForestGrid newForest = new ForestGrid(seed, this.width, this.height);
            newForest.SetMap(ApplyCellularAutomata(GenerateNoiseGrid(newForest.GetMap(), noiseDensity, seed), iterationsCount, majority));
            EliminateUnreachableAreas(newForest.GetMap(), newForest.GetWalkableArea());

            //Add forest to dictionary of maps.
            this.listOfForests.Add(seed, newForest);

            //Display it in screen.
            voxGen.GenerateMesh(newForest.GetMap(), 2);
        }
    }

    private int[,] GenerateNoiseGrid(int[,] noiseMap, int density, string seed)
    {
        
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

                /*
                //Check if tile is border - if true mark it as 1.
                if( x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    noiseMap[x, y] = 1;
                }*/

                //We want the map to have 4 exit ways: up,down,left,right.
                //So, make sure we create a bunch of 0s on each edge so player can exit.
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
        int[,] tempMap = new int[noiseMap.GetLength(0), noiseMap.GetLength(1)];

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

    private void FloodFill(int[,] map, int x, int y, List<Vector2> walkableAreaCoords)
    {
        if(!IsInMapBounds(x,y) || map[x,y] == 1)
        {
            return;
        }

        if(map[x, y] == 0 && !walkableAreaCoords.Contains(new Vector2(x,y)))
        {
            //Add it to walkable area if it's not already present.
            walkableAreaCoords.Add(new Vector2(x, y));

            //Continue looking in all 4 neighbouring directions.
            FloodFill(map, x + 1, y, walkableAreaCoords);
            FloodFill(map, x - 1, y, walkableAreaCoords);
            FloodFill(map, x, y + 1, walkableAreaCoords);
            FloodFill(map, x, y - 1, walkableAreaCoords);
        }
    }

    private void EliminateUnreachableAreas(int[,] map, List<Vector2> walkableAreaCoords)
    {
        //Identify main walkable area - we want it to be "centered" so we start from the closest ground tile to the center of the map.
        int[,] checkedTiles = new int[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y< height; y++)
            {
                checkedTiles[x, y] = 0;
            }
        }

        Vector2 pos = FindClosestTileWithValue(map, checkedTiles, width / 2, height / 2, 0);

        if(pos != new Vector2(-1,-1))
        {
            FloodFill(map, (int)pos.x, (int)pos.y, walkableAreaCoords);
        }
        else
        {
            //Code shouldn't get here unless map is all empty, but we can print an error just in case.
            Debug.LogError("Why is map all empty??");
            return;
        }



        //Go over all map points. If within walkable area, do nothing, else make it a wall.
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(!walkableAreaCoords.Contains(new Vector2(x,y)))
                {
                    map[x,y] = 1;
                }
            }
        }

    }

    private Vector2 FindClosestTileWithValue(int[,] map, int[,] checkedTiles, int x, int y, int value)
    {
        Vector2 notGroundReturn = new Vector2(-1, -1);

        if(!IsInMapBounds(x,y) || checkedTiles[x,y] == 1)
        {
            return notGroundReturn;
        }

        //Mark it as checked.
        checkedTiles[x, y] = 1;

        if (map[x, y] == value)
        {
            return new Vector2(x, y); 
        }

        //Else, continue looking in 4 neighbouring directions until we find the first ground tile.
        Vector2 n1 = FindClosestTileWithValue(map, checkedTiles, x + 1, y + 0, value);
        Vector2 n2 = FindClosestTileWithValue(map, checkedTiles, x - 1, y + 0, value);
        Vector2 n3 = FindClosestTileWithValue(map, checkedTiles, x + 0, y + 1, value);
        Vector2 n4 = FindClosestTileWithValue(map, checkedTiles, x + 0, y - 1, value);

        if (n1 != notGroundReturn)
        {
            return n1;
        }
        else if(n2 != notGroundReturn)
        {
            return n2;
        }
        else if (n3 != notGroundReturn)
        {
            return n3;
        }
        else if (n4 != notGroundReturn)
        {
            return n4;
        }
        else
        {
            return notGroundReturn;
        }

    }
}


