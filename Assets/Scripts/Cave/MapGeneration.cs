using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using Extreme.Statistics.Distributions;

public class MapGeneration : MonoBehaviour
{
    private System.Random rand = new System.Random();
    public static int roomWidth = 10, roomHeight = 10;

    int[,] map;

    int[,] prevMove;

    private int noOfRooms = 1, roomPos, row, column;
    private bool stopCondition;
    public static Vector2 firstRoomPos, lastRoomPos;

    public GameObject portalEntryPrefab, portalExitPrefab, portalForestPrefab ,hiddenRoomDoorPrefab ,environmentPrefab, mainMapRef, minimapRef, baseRoom, hiddenRoom;
    public int minNoRooms;
    public static bool readyForPlayer = false;

    private Dictionary<Vector2, GameObject> gameMap = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, Room> gameObjMap = new Dictionary<Vector2, Room>();

    public static GameObject entryPortalRef;

    private void Init()
    {
        map = new int[4, 4]{
                                {0, 0, 0, 0},
                                {0, 0, 0, 0},
                                {0, 0, 0, 0},
                                {0, 0, 0, 0}
                            };

        prevMove = new int[4, 4]{
                                {-1, -1, -1, -1},
                                {-1, -1, -1, -1},
                                {-1, -1, -1, -1},
                                {-1, -1, -1, -1}
                            };
        noOfRooms = 1;
        stopCondition = false;
        gameMap.Clear();
        gameObjMap.Clear();

    }

    void Start()
    {
        Init();
        //GenerateMapGrid();
        GameManagerScript.instance.dungeonMapRef = this.gameObject;
    }

    private void Update()
    {
        if(GameManagerScript.instance.dungeonInUse && GameManagerScript.instance.dungeonNeedsRegeneration)
        {
            Debug.Log("Generating dungeon...");

            GameManagerScript.instance.Reset();
            GameManagerScript.instance.dungeonInUse = true;

            GenerateNewMap(); 
        }
    }

    private void DrawMap()
    {
        GameObject portalInst1, portalInst2;
        bool temp;

        for (int i = 0; i <= 3; i++)
            for (int j = 0; j <= 3; j++)
            {
                // Starting from top left corner. 
                temp = false;

                //Check if left neighbour room is empty = type 0.
                if( j < 3 && map[i,j + 1] == 0)
                {
                    temp = true;
                }
                DrawAndGenerateRoom(map[i, j], i * roomHeight, j * roomWidth, temp);
            }

        //Instantiate entry point.
        GameManagerScript.instance.latestPlayerEntryPoint = Instantiate(portalEntryPrefab, new Vector2(-firstRoomPos.x * roomWidth, firstRoomPos.y), Quaternion.identity) as GameObject;
        GameManagerScript.instance.latestPlayerEntryPoint.transform.SetParent(GameManagerScript.instance.latestGeneratedEnvironmentDungeon.transform, false);

        //Instantiate portal to next dungeon room.
        portalInst1 = Instantiate(portalExitPrefab, new Vector2(-lastRoomPos.x * roomWidth , -lastRoomPos.y * roomHeight), Quaternion.identity) as GameObject;
        portalInst1.transform.SetParent(GameManagerScript.instance.latestGeneratedEnvironmentDungeon.transform, false);

        //Instantiate portal to forest.
        //portalInst2 = Instantiate(portalForestPrefab, GameManagerScript.instance.latestGeneratedEnvironmentDungeon.transform.GetChild(4).transform.position, Quaternion.identity) as GameObject;
        portalInst2 = Instantiate(portalForestPrefab, new Vector2(-lastRoomPos.x * roomWidth + 2, -lastRoomPos.y * roomHeight), Quaternion.identity) as GameObject;
        portalInst2.transform.SetParent(GameManagerScript.instance.latestGeneratedEnvironmentDungeon.transform, false);

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


    private void DrawAndGenerateRoom(int roomType, int x, int y, bool LeftIsEmpty)
    {
        GameObject room;
        Room roomObj;

        // Room (0,0) grid coordinates is top left corner. 
        Vector2 position = new Vector2(-y, -x);

        room = DrawRoom(roomType, x, y);

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
        Room newRoom = new Room(roomType, position);
        GenerateRoomTemplate(newRoom);

        gameObjMap.Add(position, newRoom);

        UpdateRoomBorder(room, newRoom.getMap(), newRoom, LeftIsEmpty);

    }


    private void UpdateRoomBorder(GameObject room, float[,] layout, Room roomObj, bool leftIsEmpty)
    {
        Room right, left;
        bool placedRoomRight = false, placedRoomLeft = false;

        gameObjMap.TryGetValue(new Vector2(roomObj.getPosition().x + 10, roomObj.getPosition().y), out right);
        gameObjMap.TryGetValue(new Vector2(roomObj.getPosition().x - 10, roomObj.getPosition().y), out left);

        


        foreach (Transform child in room.transform)
        {
         
            int i = Mathf.Abs((int)(-4.5 + child.localPosition.y));
            int j = Mathf.Abs((int)(4.5 + child.localPosition.x));


            //Only delete if tile is on the border.

            if (child.tag == "WallTile" ) 
            {
                
                //Place door for each entry to empty room.


                //Mark the room as having a hidden door....for the left it does not work, so maybe do this after all rooms have been generated.
                
                if (right != null && right.getType() == 0 && !placedRoomRight && j == (roomWidth - 1) && layout[i, j] == 0 && roomObj.getType() != 0 && !right.hasHiddenDoor)
                {
                    //Place door.
                    GameObject door = Instantiate(hiddenRoomDoorPrefab, new Vector3( child.position.x + 1, child.position.y + 0.5f, child.position.z), Quaternion.identity) as GameObject;
                    door.transform.SetParent(room.transform, true);

                    placedRoomRight = true;
                
                }

                //if (leftIsEmpty && !placedRoomLeft && j == 0 && (layout[i, j] == 0 || roomObj.getType() == 0) )
                if (leftIsEmpty && !placedRoomLeft && j == 0 && layout[i, j] == 0 && roomObj.getType() != 0)
                {
                    //Place door.
                    GameObject door = Instantiate(hiddenRoomDoorPrefab, new Vector3(child.position.x - 1 , child.position.y - 0.5f, child.position.z), Quaternion.identity) as GameObject;
                    door.transform.SetParent(room.transform, true);

                    placedRoomLeft = true;

                }

                if(layout[i, j] == 0)
                {
                    //Destroy child.
                    Destroy(child.gameObject);
                }
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
        right = getRoom(new Vector2(pos.x + 1*roomWidth, pos.y));
        left = getRoom(new Vector2(pos.x - 1*roomWidth, pos.y));

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
            int pos1 = rand.Next(2, roomHeight - 1);
            int pos2 = rand.Next(2, roomHeight - 1);

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
        Room right, left, down;
        int random;

        //Look left and right to detect already existent entries.
        right = getRoom(new Vector2(pos.x + 1*roomWidth, pos.y));
        left = getRoom(new Vector2(pos.x - 1 * roomWidth, pos.y));
        down = getRoom(new Vector2(pos.x, pos.y - 1*roomHeight));

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
        Room right, left, up;
        int random;

        //Look left and right to detect already existent entries.
        right = getRoom(new Vector2(pos.x + 1*roomWidth, pos.y));
        left = getRoom(new Vector2(pos.x - 1 * roomWidth, pos.y));
        up = getRoom(new Vector2(pos.x, pos.y + 1*roomHeight));

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
        right = getRoom(new Vector2(pos.x + 1 * roomWidth, pos.y));
        left = getRoom(new Vector2(pos.x - 1 * roomWidth, pos.y));
        up = getRoom(new Vector2(pos.x, pos.y + 1 * roomHeight));
        down = getRoom(new Vector2(pos.x, pos.y - 1 * roomHeight));

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
        random = rand.Next(1, 20);

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

    public GameObject DrawRoom(int roomType, int x, int y)
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

        //Instantiate the base room template for non empty rooms.
        if(roomType != 0 )
        {
            room = Instantiate(baseRoom, position, Quaternion.identity) as GameObject;
        }
        else
        {
            room = Instantiate(hiddenRoom, position, Quaternion.identity) as GameObject;
        }
        
        room.transform.SetParent(GameManagerScript.instance.latestGeneratedEnvironmentDungeon.transform.GetChild(0).gameObject.transform, false);

        gameMap.Add(position, room);

        return room;
    }

    

    private void GenerateMapGrid()
    {

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

            if(row == 3)
            {
                lastRoomPos = new Vector2(column, row);
            }

            //Check where we came from.
            if (map[row, column] == 0)
            {
                map[row, column] = 1;
            }

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

        //PrintMap();
        DrawMap();

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


    private void GenerateNewMap()
    {
        //Reset map matrix and delete all rooms.
        Init();

        GameObject spawnPoint;


        spawnPoint = GameManagerScript.instance.latestGeneratedEnvironmentDungeon.transform.GetChild(3).gameObject;
        
        //Instantiate new environment.
        GameManagerScript.instance.latestGeneratedEnvironmentDungeon = Instantiate(environmentPrefab, spawnPoint.transform.position, Quaternion.identity) as GameObject;
        GameManagerScript.instance.latestGeneratedEnvironmentDungeon.transform.SetParent(GameManagerScript.instance.dungeonMapRef.transform, true);

        //Generate new room
        GenerateMapGrid();

        Debug.Log("Dungeon generated, ready for player...");
        GameManagerScript.instance.dungeonReadyForPlayer = true;

        //Show new map on the minimap view.
        minimapRef.transform.position = new Vector3(minimapRef.transform.position.x, minimapRef.transform.position.y - 4 * roomHeight, minimapRef.transform.position.z);

        
    }


}
