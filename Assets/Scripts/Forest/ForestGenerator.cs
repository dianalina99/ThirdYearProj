using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForestGrid 
{
    #region Properties
    private int width, height, iterationsCount, majority;
    private int[,] map;
    private List<Vector2> walkableArea;

    private string seed;
    private ForestGrid left, right, up, down;
    private Vector2 entryL, entryR, entryU, entryD;
    private Vector2 exitToDungeon;

    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }
    public int IterationsCount { get => iterationsCount; set => iterationsCount = value; }
    public int Majority { get => majority; set => majority = value; }
    public ForestGrid Left { get => left; set => left = value; }
    public ForestGrid Right { get => right; set => right = value; }
    public ForestGrid Up { get => up; set => up = value; }
    public ForestGrid Down { get => down; set => down = value; }
    public int[,] Map { get => map; set => map = value; }
    public List<Vector2> WalkableArea { get => walkableArea; set => walkableArea = value; }
    public string Seed { get => seed; set => seed = value; }
    
    public Vector2 EntryR { get => entryR; set => entryR = value; }
    public Vector2 EntryU { get => entryU; set => entryU = value; }
    public Vector2 EntryD { get => entryD; set => entryD = value; }
    public Vector2 EntryL { get => entryL; set => entryL = value; }
    public Vector2 ExitToDungeon { get => exitToDungeon; set => exitToDungeon = value; }


    #endregion


    public ForestGrid(string seed, int width, int height, int iterationCount, int majority)
    {
        this.Seed = seed;
        this.Map = new int[width, height];
        this.WalkableArea = new List<Vector2>();
        this.iterationsCount = iterationCount;
        this.majority = majority;

        Left = Right = Up = Down = null;

        this.width = width;
        this.height = height;

        this.entryL = this.entryR = this.entryD = this.entryU = this.ExitToDungeon = new Vector2(-1, -1);
    }

    public void SetAdjacentForestGrids(ForestGrid left, ForestGrid right, ForestGrid up, ForestGrid down)
    {
        this.Left = left;
        this.Right = right;
        this.Up = up;
        this.Down = down;
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

    public MeshGenerator centerMesh, rightMesh, leftMesh, upMesh, downMesh;

    private GameObject entryRef;
    private GameObject exitRef;
    private Dictionary<string, ForestGrid> listOfForests;
    ForestGrid centerMap;

    private int[,] concatenatedMap;



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

        if(GameManagerScript.instance.currentForestGrid != null)
        {
            GenerateCenterMap(GameManagerScript.instance.currentForestGrid.Seed);
        }
        else
        {
            GenerateCenterMap(null);
        }
        
        //Place entry and exit points.
        if( GameManagerScript.instance.currentForestGrid.ExitToDungeon == new Vector2(-1,-1))
        {
            //Pick a random place in walkable area.
            System.Random rand = new System.Random();
            int randomIndex = rand.Next(0, GameManagerScript.instance.currentForestGrid.WalkableArea.Count);

            Vector2 spawnPoint = GameManagerScript.instance.currentForestGrid.WalkableArea[randomIndex];

            //Multiply spawn point by 2 because the square cell size of the mesh is 2 in our case.
            exitRef = Instantiate(this.portalToDungeonPrefab, spawnPoint * 2, Quaternion.identity) as GameObject;
            exitRef.transform.SetParent(this.transform, false);
            GameManagerScript.instance.currentForestGrid.ExitToDungeon = spawnPoint;
        }
        else
        {
            exitRef = Instantiate(this.portalToDungeonPrefab, GameManagerScript.instance.currentForestGrid.ExitToDungeon * 2, Quaternion.identity) as GameObject;
            exitRef.transform.SetParent(this.transform, false);
        }
        

        //Check if player just entered the game or if it came back from a dungeon.
        if (GameManagerScript.instance.previousForestGrid == null)
        {
            
            
             //Place entry point somewhere in the middle of the map.
             entryRef = Instantiate(this.forestEntrancePrefab, new Vector3(52, 52, 0), Quaternion.identity) as GameObject;
             entryRef.transform.SetParent(this.transform, false);
             GameManagerScript.instance.latestPlayerEntryPoint = entryRef;
          
        }
        else if(GameManagerScript.instance.previousForestGrid == GameManagerScript.instance.currentForestGrid)
        {
            //If player came back from a dungeon, spawn it in the proximity of the exit portal previously used.
            Vector2 left, right, up, down, spawnPoint = new Vector2(-1,-1);

            left = new Vector2(GameManagerScript.instance.currentForestGrid.ExitToDungeon.x + 1, GameManagerScript.instance.currentForestGrid.ExitToDungeon.y );
            right = new Vector2(GameManagerScript.instance.currentForestGrid.ExitToDungeon.x - 1, GameManagerScript.instance.currentForestGrid.ExitToDungeon.y );
            up = new Vector2(GameManagerScript.instance.currentForestGrid.ExitToDungeon.x  , GameManagerScript.instance.currentForestGrid.ExitToDungeon.y - 1);
            down = new Vector2(GameManagerScript.instance.currentForestGrid.ExitToDungeon.x, GameManagerScript.instance.currentForestGrid.ExitToDungeon.y + 1);


            if (GameManagerScript.instance.currentForestGrid.WalkableArea.Contains(left))
            {
                spawnPoint = left * 2;
            }
            else if (GameManagerScript.instance.currentForestGrid.WalkableArea.Contains(right))
            {
                spawnPoint = right * 2;
            }
            else if (GameManagerScript.instance.currentForestGrid.WalkableArea.Contains(up))
            {
                spawnPoint = up * 2;
            }
            else if (GameManagerScript.instance.currentForestGrid.WalkableArea.Contains(down))
            {
                spawnPoint = down * 2;
            }

            entryRef = Instantiate(this.forestEntrancePrefab, spawnPoint, Quaternion.identity) as GameObject;
            entryRef.transform.SetParent(this.transform, false);
            GameManagerScript.instance.latestPlayerEntryPoint = entryRef;

        }
        else 
        {
            //See where the player came from and instantiate entry there.
            if(GameManagerScript.instance.previousForestGrid == GameManagerScript.instance.currentForestGrid.Down)
            {
                //Spawn entry points on all available entries (L R D U) and mark them as not-active.
                entryRef = Instantiate(this.forestEntrancePrefab, GameManagerScript.instance.currentForestGrid.EntryD * 2, Quaternion.identity) as GameObject;
                entryRef.transform.SetParent(this.transform, false);
            }
            else if(GameManagerScript.instance.previousForestGrid == GameManagerScript.instance.currentForestGrid.Up)
            {
                entryRef = Instantiate(this.forestEntrancePrefab, GameManagerScript.instance.currentForestGrid.EntryU * 2, Quaternion.identity) as GameObject;
                entryRef.transform.SetParent(this.transform, false);
            }
            else if(GameManagerScript.instance.previousForestGrid == GameManagerScript.instance.currentForestGrid.Left)
            {
                entryRef = Instantiate(this.forestEntrancePrefab, GameManagerScript.instance.currentForestGrid.EntryL * 2, Quaternion.identity) as GameObject;
                entryRef.transform.SetParent(this.transform, false);

            }
            else if(GameManagerScript.instance.previousForestGrid == GameManagerScript.instance.currentForestGrid.Right)
            {
                entryRef = Instantiate(this.forestEntrancePrefab, GameManagerScript.instance.currentForestGrid.EntryR * 2, Quaternion.identity) as GameObject;
                entryRef.transform.SetParent(this.transform, false);
            }

            GameManagerScript.instance.latestPlayerEntryPoint = entryRef;
        }

        //Mark map generation as done.
        GameManagerScript.instance.forestReadyForPlayer = true;
        GameManagerScript.instance.playerIsCurrentlyTeleporting = true;

        Debug.Log("Forest generated, ready for player...");

    }

    private void GenerateCenterMap(string seed)
    {
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
                //Check if forest is missing any adjacent forest grids.
                //If yes, generate them, ensure connectivity and then display.
                if(forest.Left == null)
                {
                    seed = (Time.time + 2).ToString();
                    ForestGrid left = GenerateNewMapGrid(seed);
                    left.Right = forest;
                    forest.Left = left;
                }
                if (forest.Right == null)
                {
                    seed = (Time.time + 1).ToString();
                    ForestGrid right = GenerateNewMapGrid(seed);
                    right.Left = forest;
                    forest.Right = right;


                }
                if(forest.Up == null)
                {
                    seed = (Time.time + 3).ToString();
                    ForestGrid up = GenerateNewMapGrid(seed);
                    up.Down = forest;
                    forest.Up = up;
                    
                }
                if(forest.Down == null)
                {
                    seed = (Time.time + 4).ToString();
                    ForestGrid down = GenerateNewMapGrid(seed);
                    down.Up = forest;
                    forest.Down = down;
                    
                }


                this.centerMap = forest;
                
            }
        }
        else
        {
            /*  When generating a new "center/main" map, we need to also generate adjacent grids.
            *   The reason is, we need to ensure connectivity, hence create "tunnels" that connect the
            *   main grid with the adjacent ones.
            *   After tunnels are created, we run cellular automata for a few iterations on the whole 5 grids
            *   in order to smooth everything out.
            */ 

            seed = Time.time.ToString();
            ForestGrid center = GenerateNewMapGrid(seed);

            seed = (Time.time + 1).ToString();
            ForestGrid right = GenerateNewMapGrid(seed);

            seed = (Time.time + 2).ToString();
            ForestGrid left = GenerateNewMapGrid(seed);

            seed = (Time.time + 3).ToString();
            ForestGrid up = GenerateNewMapGrid(seed);

            seed = (Time.time + 4).ToString();
            ForestGrid down = GenerateNewMapGrid(seed);

            this.centerMap = center;
            this.centerMap.SetAdjacentForestGrids(left, right, up, down);
            this.centerMap.Up.Down = this.centerMap;
            this.centerMap.Down.Up = this.centerMap;
            this.centerMap.Left.Right = this.centerMap;
            this.centerMap.Right.Left = this.centerMap;

            GameManagerScript.instance.previousForestGrid = GameManagerScript.instance.currentForestGrid;
            GameManagerScript.instance.currentForestGrid = this.centerMap;


        }

        


        //Ensure connectivity.
        EnsureConnectivity(this.centerMap, this.centerMap.Right, this.centerMap.Left, this.centerMap.Up, this.centerMap.Down);

        //Detect entry points for each map.
        AssignEntryPoints(this.centerMap.Right);
        AssignEntryPoints(this.centerMap.Left);
        AssignEntryPoints(this.centerMap.Up);
        AssignEntryPoints(this.centerMap.Down);
        AssignEntryPoints(this.centerMap);

        //Display maps on screen.
        DisplayMapOnScreen(this.centerMap, centerMesh);
    }

    private void AssignEntryPoints(ForestGrid forest)
    {
        
        if(forest.Right != null)
        {
            for (int x = forest.Width - 1; x >= 0; x--)
            {
                for (int y = 0; y < forest.Height; y++)
                {
                    if (forest.Map[x, y] == 0)
                    {
                        //forest.Map[x - 1, y] = 2;
                        //RIGHT-SIDE on screen
                        //forest.EntryR = new Vector2(x - 2, y);
                        forest.EntryR = new Vector2(x , y);
                        x = -1;
                        break;
                    }
                }
            }
        }
        
        if (forest.Left != null)
        {
            for (int x = 0; x < forest.Width; x++)
            {
                for (int y = 0; y < forest.Height; y++)
                {
                    if (forest.Map[x, y] == 0)
                    {
                        //forest.Map[x + 1, y] = 2;
                        //LEFT-SIDE on screen
                        //forest.EntryL = new Vector2(x + 2, y);
                        forest.EntryL = new Vector2(x , y);
                        x = forest.Width;
                        break;
                    }
                }
            }
        }

        if (forest.Down != null)
        {
            for (int y = 0; y < forest.Height; y++)
            {
                for (int x = 0; x < forest.Width; x++)
                {
                    if (forest.Map[x, y] == 0)
                    {
                        //forest.Map[x, y + 1] = 2;
                        //DOWN-SIDE on screen
                        //forest.EntryD = new Vector2(x, y + 2);
                        forest.EntryD = new Vector2(x , y);
                        y = forest.Height;
                        break;
                    }
                }
            }

        }
        
        if(forest.Up != null)
        {
            for (int y = forest.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < forest.Width; x++)
                {
                    if (forest.Map[x, y - 1] == 0)
                    {
                        //forest.Map[x, y] = 2;
                        //UP-SIDE on screen
                        //forest.EntryU = new Vector2(x, y - 2);
                        forest.EntryU = new Vector2(x , y);
                        y = -1;
                        break;
                    }
                }
            }

        }
       
    }

    private void EnsureConnectivity(ForestGrid center, ForestGrid right, ForestGrid left, ForestGrid up, ForestGrid down)
    {
        /* We need to construct tunnels between center map and each adjacent map.
         * To do this, we need to find the closest ground tile to border in each 4 directions for center map.
         * Once done, create straight rectangular areas of set width from found ground tiles towards adjacent map grids.
         * Lastly, we run cellular automata for a few iterations on all maps to "blend" tunnel edges
         */

        Vector2 centerRight, centerLeft, centerUp, centerDown, rightLeft, leftRight, upDown, downUp;
        centerRight = centerLeft = centerUp = centerDown = rightLeft = leftRight = upDown = downUp =  new Vector2(0, 0);

        /* When finding the tunnel points, we need to use the normal coordinate system (screen).
         * So we don't treat the (0,0) as top left corner, but instead as bottom left corner.
         * 
         * See axis in following comment.
         *
         */

        #region Find Tunnel Starting Points
        for ( int x = 0; x < center.Width; x++)
        {
            for( int y = 0; y < center.Height; y++)
            {
                if(center.Map[x,y] == 0)
                {
                    //LEFT-SIDE on screen
                    centerLeft = new Vector2(x, y);
                    x = center.Width;
                    break;
                }
            }
        }
        
        for (int x = center.Width - 1; x >= 0; x--)
        {
            for (int y = 0; y < center.Height; y++)
            {
                if (center.Map[x, y] == 0)
                {
                    //RIGHT-SIDE on screen
                    centerRight = new Vector2(x, y);
                    x = -1;
                    break;
                }
            }
        }
        
        for (int y = 0; y < center.Height; y++)
        {
            for (int x = 0; x < center.Width; x++)
            {
                if (center.Map[x, y] == 0)
                {
                    //DOWN-SIDE on screen
                    centerDown = new Vector2(x, y);
                    y = center.Height;
                    break;
                }
            }
        }
        
        for (int y = center.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < center.Width; x++)
            {
                if (center.Map[x, y] == 0)
                {
                    //UP-SIDE on screen
                    centerUp = new Vector2(x, y);
                    y = -1;
                    break;
                }
            }
        }
        
        for (int x = 0; x < right.Width; x++)
        {
            for (int y = 0; y < right.Height; y++)
            {
                if (right.Map[x, y] == 0)
                {
                    //LEFT-SIDE on screen
                    rightLeft = new Vector2(x, y);
                    x = right.Width;
                    break;
                }
            }
        }


        for (int x = left.Width - 1; x >= 0; x--)
        {
            for (int y = 0; y < left.Height; y++)
            {
                if (left.Map[x, y] == 0)
                {
                    //RIGHT-SIDE on screen
                    leftRight = new Vector2(x, y);
                    x = -1;
                    break;
                }
            }
        }


        for (int y = 0; y < up.Height; y++)
        {
            for (int x = 0; x < up.Width; x++)
            {
                if (up.Map[x, y] == 0)
                {
                    //DOWN-SIDE on screen
                    upDown = new Vector2(x, y);
                    y = center.Height;
                    break;
                }
            }
        }

        
        for (int y = down.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < down.Width; x++)
            {
                if (down.Map[x, y] == 0)
                {
                    //UP-SIDE on screen
                    downUp = new Vector2(x, y);
                    y = -1;
                    break;
                }
            }
        }

        #endregion

        /* Concatenate all maps into one big map.
         * Keep in mind our map axis is inverted (because our (0,0) point is top left corner.
         * Take a normal matrix with (i,j) coords. In our case, we have (x,y) coords,
         * where x -> width and y -> height. 
         * This basically means that what we would normally think of when saying "up"
         * is "left" in our concatenated map.
         * 
         * Axis looks like this:
         *      /\ y                -------> y
         *      |                   |
         *      |    x        =>    | 
         *      ----->              \/ x
         *      
         *  Screen uses the normal map coordinates.
         */

        concatenatedMap = new int[this.width * 3, this.height * 3];

        //Concatenate all maps.
        for(int x = 0; x< this.width * 3; x++)
        {
            for(int y = 0; y< this.height * 3; y++)
            {
                if (x < this.width && y < this.height)
                {
                    concatenatedMap[x, y] = 1;
                }
                else if (x < this.width && y >= this.height && y < this.height * 2)
                {
                    //Up in matrix coord system.
                    concatenatedMap[x, y] = left.Map[x % this.width, y % this.height];
                }
                else if (x < this.width && y >= this.height * 2 && y < this.height * 3)
                {
                    concatenatedMap[x, y] = 1;
                }
                else if(x >= this.width && x < this.width * 2 && y < this.height)
                {
                    //Left in matrix coord system.
                    concatenatedMap[x, y] = down.Map[x % this.width, y % this.height];
                }
                else if(x >= this.width && x < this.width * 2 && y >= this.height && y < this.height * 2)
                {
                    concatenatedMap[x, y] = center.Map[x % this.width, y % this.height];
                }
                else if (x >= this.width && x < this.width * 2 && y >= this.height * 2)
                {
                    //Right in matrix coord system.
                    concatenatedMap[x, y] = up.Map[x % this.width, y % this.height];
                }
                else if (x >= this.width * 2 && y < this.height)
                {
                    concatenatedMap[x, y] = 1;
                }
                else if (x >= this.width * 2 && y >= this.height && y < this.height * 2)
                {
                    //Down in matrix coord system.
                    concatenatedMap[x, y] = right.Map[x % this.width, y % this.height];
                }
                else if (x >= this.width * 2 && y >= this.height * 2)
                {
                    concatenatedMap[x, y] = 1;
                }
            }
        }


        //Display tunnel connection points by setting map value at given x, y to 2.
        centerUp = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(centerUp, "center").x, ConvertCoordsFromSingleToConcatenatedMap(centerUp, "center").y);
        centerDown = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(centerDown, "center").x, ConvertCoordsFromSingleToConcatenatedMap(centerDown, "center").y);
        centerLeft = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(centerLeft, "center").x, ConvertCoordsFromSingleToConcatenatedMap(centerLeft, "center").y);
        centerRight = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(centerRight, "center").x, ConvertCoordsFromSingleToConcatenatedMap(centerRight, "center").y);
        leftRight = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(leftRight, "left").x, ConvertCoordsFromSingleToConcatenatedMap(leftRight, "left").y);
        rightLeft = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(rightLeft, "right").x, ConvertCoordsFromSingleToConcatenatedMap(rightLeft, "right").y);
        upDown = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(upDown, "up").x, ConvertCoordsFromSingleToConcatenatedMap(upDown, "up").y);
        downUp = new Vector2(ConvertCoordsFromSingleToConcatenatedMap(downUp, "down").x, ConvertCoordsFromSingleToConcatenatedMap(downUp, "down").y);

        int groundTunnelValue = -1;
        concatenatedMap[(int)centerUp.x,(int) centerUp.y] = groundTunnelValue;
        concatenatedMap[(int) centerDown.x, (int) centerDown.y] = groundTunnelValue;
        concatenatedMap[(int) centerLeft.x, (int) centerLeft.y] = groundTunnelValue;
        concatenatedMap[(int) centerRight.x, (int) centerRight.y] = groundTunnelValue;
        concatenatedMap[(int) leftRight.x, (int) leftRight.y] = groundTunnelValue;
        concatenatedMap[(int) rightLeft.x, (int) rightLeft.y] = groundTunnelValue;
        concatenatedMap[(int) upDown.x, (int) upDown.y] = groundTunnelValue;
        concatenatedMap[(int) downUp.x, (int) downUp.y] = groundTunnelValue;

        //Draw tunnel to connect adjacent points.

        #region Draw Tunnels to connect adjacent map grids

        

        Vector2 temp = centerUp;

        //CenterUp
        while(temp.y < upDown.y)
        {
            temp.y++;
            concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
        }
        
        if(upDown.x < temp.x)
        {
            do
            {
                temp.x--;
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;

            } while (upDown.x < temp.x);
        }
        else if (upDown.x > temp.x)
        {
            do
            {
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
                temp.x++;
            } while (upDown.x > temp.x);
        }

        //CenterDown
        temp = centerDown;

        while (temp.y > downUp.y)
        {
            temp.y--;
            concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
        }
        if (downUp.x < temp.x)
        {
            do
            {
                temp.x--;
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;

            } while (downUp.x < temp.x);
        }
        else if (downUp.x > temp.x)
        {
            do
            {
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
                temp.x++;
            } while (downUp.x > temp.x);
        }

        //CenterLeft
        temp = centerLeft;

        while(temp.x > leftRight.x)
        {
            temp.x--;
            concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
        }
        if(leftRight.y < temp.y)
        {
            do
            {
                temp.y--;
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
            } while (leftRight.y < temp.y);
         
        }
        else if(leftRight.y > temp.y)
        {
            do
            {
                temp.y++;
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
            } while (leftRight.y > temp.y);

        }

        //CenterRight
        temp = centerRight;

        while (temp.x < rightLeft.x)
        {
            temp.x++;
            concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
        }
        if (rightLeft.y < temp.y)
        {
            do
            {
                temp.y--;
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
            } while (rightLeft.y < temp.y);

        }
        else if (rightLeft.y > temp.y)
        {
            do
            {
                temp.y++;
                concatenatedMap[(int)temp.x, (int)temp.y] = groundTunnelValue;
            } while (rightLeft.y > temp.y);

        }

        #endregion


        //Run Cellular Automata on the concatenated map 3 times to smooth out tunnels.
        concatenatedMap = ApplyCellularAutomata(concatenatedMap, 1, 4);

        //Break down concatenated map.
        for (int x = 0; x < this.width * 3; x++)
        {
            for (int y = 0; y < this.height * 3; y++)
            {
                if (x < this.width && y >= this.height && y < this.height * 2)
                {
                    //Up in matrix coord system.
                    left.Map[x % this.width, y % this.height] = concatenatedMap[x, y];
                }
                else if (x >= this.width && x < this.width * 2 && y < this.height)
                {
                    //Left in matrix coord system.
                    down.Map[x % this.width, y % this.height] = concatenatedMap[x, y];
                }
                else if (x >= this.width && x < this.width * 2 && y >= this.height && y < this.height * 2)
                {
                    center.Map[x % this.width, y % this.height] = concatenatedMap[x, y];
                }
                else if (x >= this.width && x < this.width * 2 && y >= this.height * 2)
                {
                    //Right in matrix coord system.
                    up.Map[x % this.width, y % this.height] = concatenatedMap[x, y];
                }
                else if (x >= this.width * 2 && y >= this.height && y < this.height * 2)
                {
                    //Down in matrix coord system.
                    right.Map[x % this.width, y % this.height] = concatenatedMap[x, y];
                }
            }
        }

    }


    private Vector2 ConvertCoordsFromSingleToConcatenatedMap(Vector2 coords, string singleType)
    {
        if (singleType == "center")
        {
            return new Vector2(coords.x + this.width, coords.y + this.height);

        }
        else if (singleType == "up")
        {
            return new Vector2(coords.x + this.width, coords.y + this.height * 2);
        }
        else if( singleType == "down")
        {
            return new Vector2(coords.x + this.width, coords.y);
        }
        else if( singleType == "left")
        {
            return new Vector2(coords.x , coords.y + this.height);
        }
        else if(singleType == "right")
        {
            return new Vector2(coords.x + this.width * 2, coords.y + this.height);
        }
        else
        {
            return new Vector2(-1, -1);
        }
    }

    private ForestGrid GenerateNewMapGrid(string seed)
    {
        //If no custom seed was provided, generate new forest with new random seed.
        ForestGrid newForest = new ForestGrid(seed, this.width, this.height, this.iterationsCount, this.majority);
        newForest.Map = ApplyCellularAutomata(GenerateNoiseGrid(newForest.Map, noiseDensity, seed), iterationsCount, majority);
        EliminateUnreachableAreas(newForest.Map, newForest.WalkableArea);

        //Add forest to dictionary of maps.
        this.listOfForests.Add(seed, newForest);

        return newForest;
    }
    private void DisplayMapOnScreen(ForestGrid forest, MeshGenerator mesh)
    {
        //Display it on screen.
        mesh.GenerateMesh(forest.Map, 2);
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
        /*
        if ( centerMap != null)
        {
            for (int x = 0; x < width ; x++)
            {
                for (int y = 0; y < height ; y++)
                {
                    if (centerMap.Map[x, y] == 0)
                    {
                        Gizmos.color = Color.white;
                    }
                    else if (centerMap.Map[x, y] == 1)
                    {
                        Gizmos.color = Color.black;
                    }
                    else if (centerMap.Map[x, y] == 2)
                    {
                        Gizmos.color = Color.red;
                    }

                    Vector3 pos = new Vector3(-5 * width + x + .5f, -height / 2 + y + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }

            }

        } */

        
        if (concatenatedMap != null)
        {
            for (int x = 0; x < width * 3; x++)
            {
                for (int y = 0; y < height * 3; y++)
                {
                    if(concatenatedMap[x, y] == 0)
                    {
                        Gizmos.color = Color.white;
                    }
                    else if (concatenatedMap[x, y] == 1)
                    {
                        Gizmos.color = Color.black;
                    }
                    else if (concatenatedMap[x, y] == 2)
                    {
                        Gizmos.color = Color.red;
                    }
                    
                    Vector3 pos = new Vector3(-5* width  + x + .5f, -height/2 + y + .5f, 0 );
                    Gizmos.DrawCube(pos, Vector3.one);
                }

            }

        }


    }

    private bool IsInMapBounds(int[,] map, int x, int y)
    {
        int _width, _height;

        _width = map.GetLength(0);
        _height = map.GetLength(1);

        if(x < 0 || y < 0 || x >= _width || y >= _height)
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
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                {
                    tempMap[x, y] = noiseMap[x, y];
                }
            }

            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                {
                    //Check Moore neighbours - if majority are ground, map[i][j] becomes ground. Same for wall.
                    int noOfNeighboursWalls = 0;

                    //Go through neighbours.
                    for (int nx = x - 1; nx <= x + 1; nx++)
                    {
                        for (int ny = y - 1; ny <= y + 1; ny++)
                        {
                            //Check if it's within map bounds.
                            if (IsInMapBounds( noiseMap,nx, ny))
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
        if(!IsInMapBounds(map, x,y) || map[x,y] == 1)
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
        /* This method finds the closest tile to a set (x,y) that is not ground.
         * It returns the (x,y) map grid coordinates of the found tile.
         * If no ground tile is found, it returns (-1,-1).
         */

        Vector2 notGroundReturn = new Vector2(-1, -1);

        if(!IsInMapBounds(map, x,y) || checkedTiles[x,y] == 1)
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

