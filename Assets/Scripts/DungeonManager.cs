using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;
using Alchemy.Music;
using Alchemy.Combat;
using Alchemy.Inventory;

public class DungeonManager : MonoBehaviour
{
    public static EnemyPalette ActiveEnemyPalette;
    public Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();

    public EnemyPalette Forest;
    public EnemyPalette MidnightDesert;
    public EnemyPalette Castle;
    public EnemyPalette Sewers;

    [Header("Dungeon Settings")]
    [Tooltip("Set this to true to use the below settings, otherwise the game will decide")]
    public bool overwriteSettings = false;
    public int dungeonSize = 20;
    [Range(0f, 1f)] public float enemyProbability = 0.5f;
    [Range(1, 10)] public int enemyLevel = 1;
    [Range(1, 10)] public int playerLevel = 1;
    public DungeonType dungeonType = DungeonType.Forest;

    [Header("Required Fields")]
    // filler walls to block holes
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;

    // rooms to use for random generation
    public GameObject[] topRooms;
    public GameObject[] rightRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;

    // enemies
    public GameObject[] enemyLayouts;
    public GameObject bossLayout;

    // chests
    public GameObject[] chestLayouts;
    public Item[] chestItems;

    // doors
    [SerializeField] GameObject door;

    // dungeon generation
    private int roomCount;
    int rand;

    // keep track of rooms
    public List<Room> rooms;

    public bool IsPositionValid(Vector2 Position)
    {
        return !spawnedRooms.ContainsKey(new Vector2Int((int)Position.x, (int)Position.y));
    }

    private int roomCountLastSecond;
    private float roomCountSampleTick;


    private void Start()
    {
        if (!overwriteSettings)
        {
            // replace dungeon settings with ones from the singleton
            dungeonSize = SettingsSingleton.instance.dungeonSize;
            enemyProbability = SettingsSingleton.instance.enemyProbability;
            enemyLevel = SettingsSingleton.instance.enemyLevel;
            playerLevel = SettingsSingleton.instance.playerLevel;
            dungeonType = SettingsSingleton.instance.dungeonType;
        }

        // set dungeon type
        MusicStarter musicStater = FindObjectOfType<MusicStarter>();
        musicStater.DungeonType = dungeonType;
        musicStater.RefreshGraphics();

        switch (dungeonType)
        {
            case DungeonType.Forest:
                ActiveEnemyPalette = Forest;
                break;
            case DungeonType.MidnightDesert:
                ActiveEnemyPalette = MidnightDesert;
                break;
            case DungeonType.Castle:
                ActiveEnemyPalette = Castle;
                break;
            case DungeonType.Sewers:
                ActiveEnemyPalette = Sewers;
                break;
        }

        roomCount = dungeonSize - 1; // minus one to account for first room
        rooms = new List<Room>();

        // set player level
        FindObjectOfType<PlayerMovement>().GetComponent<ActorStats>().CurrentLevel = playerLevel;
        FindObjectOfType<PlayerMovement>().GetComponent<ActorStats>().ResetStats(); // Makes sure player's health and stamina are maxed at higher levels

        // Build(); // build the dungeon
        Invoke("Build", 0.1f);
    }

    public void Build()
    {
        Debug.Log("Build dungeon");
        ConstructRooms();
        CloseRoomsAndInitEnemies();
        BuildDoors();
        SetupLevelEnd();
        Invoke("RemoveLoadingScreen", 1f);
        PlayerMovement.PreviousRoom = rooms[0];
    }

    public void ConstructRooms()
    {
        // setup first room
        if (rooms.Count == 0)
        {
            Room firstRoom = FindObjectOfType<Room>();
            rooms.Add(firstRoom);
        }
        rooms[0].Init();
        spawnedRooms.Add(new Vector2Int((int)rooms[0].transform.position.x, (int)rooms[0].transform.position.y), rooms[0]);

        // setup spawn queue
        Queue<RoomSpawner> spawnQueue = new Queue<RoomSpawner>();
        // get initial spawners and queue them
        RoomSpawner[] initialSpawners = rooms[0].transform.parent.GetComponentsInChildren<RoomSpawner>();
        foreach (RoomSpawner spawner in initialSpawners)
        {
            spawnQueue.Enqueue(spawner);
        }

        // Debug.Log("Rooms to build: " + spawnQueue.Count);

        // generate dungeon
        while (spawnQueue.Count > 0)
        {
            RoomSpawner currentSpawner = spawnQueue.Dequeue(); // get the next room spawner

            if (GetRemainingRooms() > 0)
            {
                // more rooms need to be made
                if (IsPositionValid(currentSpawner.transform.position))
                {
                    // this spawner can make a valid room
                    GameObject spawnedRoom = currentSpawner.Spawn(); // make the room
                    if (spawnedRoom != null)
                    {
                        // store this room and set it up
                        spawnedRooms.Add(new Vector2Int((int)spawnedRoom.transform.position.x, (int)spawnedRoom.transform.position.y), spawnedRoom.GetComponentInChildren<Room>());
                        rooms.Add(spawnedRoom.GetComponentInChildren<Room>());
                        spawnedRoom.GetComponentInChildren<Room>().Init();

                        // spawn enemies in room
                        if (Random.value <= enemyProbability)
                        {
                            rand = Random.Range(0, enemyLayouts.Length);
                            GameObject enemies = Instantiate(enemyLayouts[rand], spawnedRoom.transform.position, Quaternion.identity);
                            spawnedRoom.GetComponentInChildren<Room>().AddEnemies(enemies);
                        }
                        else if (Random.value <= .5f) // spawn chests in room, temp implementation. probability shouldn't be hard coded
                        {
                            // spawn the chest
                            rand = Random.Range(0, chestLayouts.Length);
                            GameObject chestObject = Instantiate(chestLayouts[rand], spawnedRoom.transform.position, Quaternion.identity);
                            chestObject.transform.parent = spawnedRoom.transform;

                            // setup chest contents
                            Chest chest = chestObject.GetComponentInChildren<Chest>();

                            rand = Random.Range(0, chestItems.Length);
                            Item item = chestItems[rand];
                            ItemInstance loot = new ItemInstance(item, 1);

                            ItemInstance[] lootTable = new ItemInstance[] { loot };
                            chest.LootTable = lootTable;
                        }

                        // decrement. obviously
                        DecrementRemainingRooms();

                        // get the new spawners
                        RoomSpawner[] tempSpawners = spawnedRoom.GetComponentsInChildren<RoomSpawner>();
                        // enqueue them
                        foreach (RoomSpawner spawner in tempSpawners)
                        {
                            spawnQueue.Enqueue(spawner);
                        }
                    }
                }
            }

            // update existing rooms
            if (!IsPositionValid(currentSpawner.transform.position))
            {
                Room tempRoom = null;
                spawnedRooms.TryGetValue(new Vector2Int((int)currentSpawner.transform.position.x, (int)currentSpawner.transform.position.y), out tempRoom);
                // this spawn point is on top of a room, tell that room it has a neighbor
                RoomSpawner.Direction openingDirection = currentSpawner.openingDirection;
                switch (openingDirection)
                {
                    case RoomSpawner.Direction.Top:
                        tempRoom.top = false;
                        break;
                    case RoomSpawner.Direction.Right:
                        tempRoom.right = false;
                        break;
                    case RoomSpawner.Direction.Bottom:
                        tempRoom.bottom = false;
                        break;
                    case RoomSpawner.Direction.Left:
                        tempRoom.left = false;
                        break;
                }
            }

            // remove the spawner
            Destroy(currentSpawner.gameObject);
        }


        // return to start conditions if dungeon failed
        if (rooms.Count < dungeonSize - 1)
        {
            Debug.Log("Build failed with " + rooms.Count + " rooms");
            roomCount = dungeonSize - 1; // minus one to account for first room

            // remove all rooms except for the original
            while (rooms.Count > 1)
            {
                Destroy(rooms[rooms.Count - 1].transform.parent.gameObject);
                rooms.RemoveAt(rooms.Count - 1);
            }
            // remove all enemies
            EnemyLayout[] enemies = FindObjectsOfType<EnemyLayout>();
            foreach (EnemyLayout enemy in enemies)
            {
                Destroy(enemy.gameObject);
            }
            // restore start room spawners
            foreach (RoomSpawner spawner in initialSpawners)
            {
                GameObject temp = Instantiate(spawner.gameObject, spawner.transform.position, Quaternion.identity);
                temp.transform.parent = rooms[0].transform.parent.transform;
            }
            // clear location checking dictionary
            spawnedRooms = new Dictionary<Vector2Int, Room>();

            ConstructRooms();
        }
        else
        {
            Debug.Log("Build succeeded with " + rooms.Count + " rooms");
        }
    }

    public void CloseRoomsAndInitEnemies()
    {
        Debug.Log("Close up rooms: " + rooms.Count);

        // close off rooms and init enemies
        foreach (Room room in rooms)
        {
            if (room.top)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(topWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                // Debug.Log("Made top wall");
                room.RemoveTopDoor();
            }
            if (room.right)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(rightWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                // Debug.Log("Made right wall");
                room.RemoveRightDoor();
            }
            if (room.bottom)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(bottomWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                // Debug.Log("Made bottom wall");
                room.RemoveBottomDoor();
            }
            if (room.left)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(leftWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                // Debug.Log("Made left wall");
                room.RemoveLeftDoor();
            }

            // get the enemies from the room
            List<Enemy> roomEnemies = room.GetIndividualEnemies();
            // set their level
            if (roomEnemies != null)
            {
                foreach (Enemy enemy in roomEnemies)
                {
                    enemy.gameObject.GetComponent<ActorStats>().CurrentLevel = enemyLevel;
                }
            }
        }
    }

    public void BuildDoors()
    {
        // find valid door spawners and remove invalid ones
        List<DoorSpawner> validDoorSpawners = new List<DoorSpawner>();
        Dictionary<Vector2Int, DoorSpawner> validDoorSpawnerCheck = new Dictionary<Vector2Int, DoorSpawner>();
        foreach (Room room in rooms)
        {
            for (int i = 0; i < 4; i++)
            {
                DoorSpawner ds = room.GetDoorSpawner(i);
                if (ds)
                {
                    Vector2Int dsLocation = new Vector2Int((int)ds.transform.position.x, (int)ds.transform.position.y);
                    if (!validDoorSpawnerCheck.TryAdd(dsLocation, ds))
                    {
                        // there is already a door spawner at this location

                        // add the parent rooms of the current door spawner to the existing one
                        DoorSpawner existingDoor = null;
                        validDoorSpawnerCheck.TryGetValue(dsLocation, out existingDoor);
                        existingDoor.AddRooms(ds.GetParentRooms());

                        // destroy the current door spawner
                        Destroy(ds.gameObject);
                    }
                    else
                    {
                        validDoorSpawners.Add(ds);
                    }
                }
            }
        }
        // Debug.Log("Valid door spawners: " + validDoorSpawners.Count);

        // add doors
        foreach (DoorSpawner ds in validDoorSpawners)
        {
            // build the door
            GameObject tempDoorObject = Instantiate(door, ds.transform.position, Quaternion.identity);
            Door tempDoor = tempDoorObject.GetComponent<Door>();

            // setup room-door connections
            List<Room> parentRooms = ds.GetParentRooms();
            // add doors to room
            foreach (Room room in parentRooms)
            {
                room.AddDoor(tempDoor);
            }
            // add rooms to door
            tempDoor.AddRooms(parentRooms);
            // set door type
            tempDoor.isVertical = ds.isVertical;

            // Cleanup
            Destroy(ds.gameObject);
        }
    }

    public void SetupLevelEnd()
    {
        // clear out the end room
        // remove any enemies
        if (rooms[rooms.Count - 1].HasEnemies())
        {
            rooms[rooms.Count - 1].RemoveEnemies();
        }
        // remove any chests
        GameObject finalRoom = rooms[rooms.Count - 1].gameObject.transform.parent.gameObject;
        Chest chestInBossRoom = finalRoom.GetComponentInChildren<Chest>();
        if (chestInBossRoom)
        {
            GameObject chestLayout = chestInBossRoom.gameObject.transform.parent.gameObject;
            Destroy(chestLayout);
            Debug.Log("Removed chest from boss room");
        }

        // add boss to end room
        GameObject enemies = Instantiate(bossLayout, rooms[rooms.Count - 1].transform.position, Quaternion.identity);

        // setup boss level
        List<Enemy> bossWave = enemies.GetComponent<EnemyLayout>().GetEnemies();
        foreach (Enemy enemy in bossWave)
        {
            enemy.gameObject.GetComponent<ActorStats>().CurrentLevel = enemyLevel;
        }

        rooms[rooms.Count - 1].AddEnemies(enemies);
    }

    public void RemoveLoadingScreen()
    {
        GameObject.Find("[DARKINATOR]").SetActive(false);
    }

    public int GetRemainingRooms()
    {
        return roomCount;
    }

    public void DecrementRemainingRooms()
    {
        roomCount--;
    }
}
