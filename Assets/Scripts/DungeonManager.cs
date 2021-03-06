using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;
using Alchemy.Music;
using Alchemy.Combat;
using Alchemy.Inventory;
using UnityEngine.Events;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;
    // dungeon skills
    public enum DungeonSkillType
    {
        Cartography,
        Boss_Sense,
        Scroll_Sense
    }
    public static DungeonSkillType currentDungeonSkill;
    public static bool hasCartographer;
    public static bool hasBossSense;
    public static bool hasScrollSense;

    public static EnemyPalette ActiveEnemyPalette;
    public Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();

    public EnemyPalette Forest;
    public EnemyPalette MidnightDesert;
    public EnemyPalette Castle;
    public EnemyPalette Sewers;
    public EnemyPalette Maze;

    [Header("Dungeon Settings")]
    [Tooltip("Set this to true to use the below settings, otherwise the game will decide")]
    public bool overwriteSettings = false;
    public int dungeonSize = 20;
    [Range(0f, 1f)] public float enemyProbability = 0.5f;
    [Range(0f, 1f)] public float chestProbability = 0.5f;
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
    public GameObject[] startRoomChestLayouts;
    public ItemInstance[] setChestItems;
    public Item[] fixedChestItems;

    // doors
    [SerializeField] GameObject door;

    // arrows
    [SerializeField] Color bossArrowColor;
    [SerializeField] Color scrollArrowColor;
    [SerializeField] GameObject dungeonArrowPrefab;

    // wrapper dropping
    static public CandyTrailRoom currentRoomWrappers { get; set; }

    // dungeon generation
    private int roomCount;
    int rand;

    // keep track of rooms and final dungeon
    public List<Room> rooms { get; private set; }
    public static Vector2Int topLeft { get; private set; }
    public static Vector2Int bottomRight { get; private set; }
    public static Vector3 bossPosition { get; private set; }
    public static Vector3 currentRoom { get; set; }

    public bool IsPositionValid(Vector2 Position)
    {
        return !spawnedRooms.ContainsKey(new Vector2Int((int)Position.x, (int)Position.y));
    }

    private int roomCountLastSecond;
    private float roomCountSampleTick;


    private void Start()
    {
        Instance = this;

        hasCartographer = false;
        hasBossSense = false;
        hasScrollSense = false;

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
            case DungeonType.Maze:
                ActiveEnemyPalette = Maze;
                break;
        }

        roomCount = dungeonSize - 1; // minus one to account for first room
        rooms = new List<Room>();

        // // set player level
        // FindObjectOfType<PlayerMovement>().GetComponent<ActorStats>().CurrentLevel = playerLevel;
        FindObjectOfType<PlayerMovement>().GetComponent<ActorStats>().ResetStats(); // Makes sure player's health and stamina are maxed at higher levels

        // Build(); // build the dungeon
        Invoke("Build", 0.1f);
    }

    public void Build()
    {
        Debug.Log("Build dungeon");
        ConstructRooms();
        CloseRooms();
        BuildDoors();
        SetupEnemiesAndChests();
        SetupBoss();
        SetupFirstRoom();
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

        // setup fields to keep track of edges of map
        topLeft = new Vector2Int((int)rooms[0].transform.position.x, (int)rooms[0].transform.position.y);
        bottomRight = new Vector2Int((int)rooms[0].transform.position.x, (int)rooms[0].transform.position.y);

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
                        Room spawnedRoomPoint = spawnedRoom.GetComponentInChildren<Room>();
                        spawnedRooms.Add(new Vector2Int((int)spawnedRoom.transform.position.x, (int)spawnedRoom.transform.position.y), spawnedRoomPoint);
                        rooms.Add(spawnedRoomPoint);
                        spawnedRoomPoint.Init();
                        UpdateMapFields(spawnedRoomPoint);

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

    public void CloseRooms()
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

        // activate doors in first room on map
        rooms[0].EnableDoorsOnMap();
    }

    public void SetupEnemiesAndChests()
    {
        int totalValidRooms = rooms.Count - 2; // -2 to account for start room and boss room


        int roomsToAddEnemies = Mathf.RoundToInt(totalValidRooms * enemyProbability);
        Debug.Log("Adding enemies to " + roomsToAddEnemies + " of " + totalValidRooms + " total valid rooms");
        int roomsToAddChests = Mathf.RoundToInt((float)(totalValidRooms - roomsToAddEnemies) * chestProbability);
        Debug.Log("Adding chests to " + roomsToAddChests + " of " + (totalValidRooms - roomsToAddEnemies) + " total valid rooms");

        // spawn enemies
        while (roomsToAddEnemies > 0)
        {
            int randomRoomIndex = Random.Range(1, rooms.Count - 2);
            Room room = rooms[randomRoomIndex];

            if (!room.HasEnemies())
            {
                // add enemies
                rand = Random.Range(0, enemyLayouts.Length);
                GameObject enemies = Instantiate(enemyLayouts[rand], room.transform.position, Quaternion.identity);
                enemies.transform.parent = room.transform.parent.transform;
                room.AddEnemies(enemies);

                // set their level
                List<Enemy> roomEnemies = room.GetIndividualEnemies();
                if (roomEnemies != null)
                {
                    foreach (Enemy enemy in roomEnemies)
                    {
                        enemy.gameObject.GetComponent<ActorStats>().CurrentLevel = enemyLevel;
                    }
                }

                roomsToAddEnemies--;
            }
        }

        // spawn chests
        int fixedChestItemsIndex = 0;

        while (roomsToAddChests > 0 || fixedChestItemsIndex < fixedChestItems.Length)
        {
            int randomRoomIndex = Random.Range(1, rooms.Count - 2);
            Room room = rooms[randomRoomIndex];

            if (!room.HasChests() && !room.HasEnemies())
            {
                // spawn the chest
                rand = Random.Range(0, chestLayouts.Length);
                GameObject chestObject = Instantiate(chestLayouts[rand], room.transform.position, Quaternion.identity);
                chestObject.transform.parent = room.transform.parent.transform;
                room.AddChests(chestObject);

                // setup chest contents
                Chest[] chests = chestObject.GetComponentsInChildren<Chest>();

                foreach (Chest chest in chests)
                {
                    // make loot for chest
                    List<ItemInstance> Loot = new List<ItemInstance>();
                    if (fixedChestItems != null && fixedChestItems.Length > 0 && fixedChestItemsIndex < fixedChestItems.Length)
                    {
                        // create specific items that should always be in the map
                        Item item = fixedChestItems[fixedChestItemsIndex];
                        Debug.Log("Created specific chest at " + room.transform.position);
                        fixedChestItemsIndex++;

                        room.AddArrowToRoom(scrollArrowColor, DungeonSkillType.Scroll_Sense, 0.5f);
                        Loot.Add(new ItemInstance(item, 1, false));
                    }
                    else
                    {
                        // create dynamic chest
                        Loot = setChestItems.ToList();
                        chest.isDynamic = true;
                    }

                    // add loot to chest
                    chest.LootTable = Loot.ToArray();
                }

                roomsToAddChests--;
            }
        }
    }

    public void SetupBoss()
    {
        Room bossRoom = rooms[rooms.Count - 1];

        // clear out the end room
        // remove any enemies
        if (bossRoom.HasEnemies())
        {
            bossRoom.RemoveEnemies();
            Debug.Log("Removed enemies from boss room");
        }
        // remove any chests
        GameObject finalRoom = bossRoom.gameObject.transform.parent.gameObject;
        Chest chestInBossRoom = finalRoom.GetComponentInChildren<Chest>();
        if (chestInBossRoom)
        {
            GameObject chestLayout = chestInBossRoom.gameObject.transform.parent.gameObject;
            Destroy(chestLayout);
            Debug.Log("Removed chest from boss room");
        }

        // add boss to end room
        GameObject boss = Instantiate(bossLayout, bossRoom.transform.position, Quaternion.identity);

        // setup boss level
        List<Enemy> bossWave = boss.GetComponent<EnemyLayout>().GetEnemies();
        foreach (Enemy enemy in bossWave)
        {
            enemy.gameObject.GetComponent<ActorStats>().CurrentLevel = enemyLevel;
        }

        // add the boss to the room and setup arrow
        bossRoom.AddEnemies(boss);
        bossRoom.AddArrowToRoom(bossArrowColor, DungeonSkillType.Boss_Sense, 0.75f);
    }

    private void SetupFirstRoom()
    {
        Room room = rooms[0];
        GameObject chestObject = null;

        switch (playerLevel)
        {
            case 1:
                // create 1 chest in start room
                chestObject = Instantiate(startRoomChestLayouts[0], room.transform.position, Quaternion.identity);
                break;
            case 2:
                // create 2 chests in start room
                chestObject = Instantiate(startRoomChestLayouts[1], room.transform.position, Quaternion.identity);
                break;
            case 0:
            default:
                // no chests in start room
                return;
        }

        // add chests to room
        chestObject.transform.parent = room.transform.parent.transform;
        room.AddChests(chestObject);

        // setup chest contents
        Chest[] chests = chestObject.GetComponentsInChildren<Chest>();
        foreach (Chest chest in chests)
        {
            // make loot for chest
            List<ItemInstance> Loot = new List<ItemInstance>();

            // create dynamic chest
            Loot = setChestItems.ToList();
            chest.isDynamic = true;

            // add loot to chest
            chest.LootTable = Loot.ToArray();
        }
    }



    public void LearnDungeonSkill()
    {
        Debug.Log("Event as... " + currentDungeonSkill);
        switch (currentDungeonSkill)
        {
            case DungeonSkillType.Cartography:
                hasCartographer = true;
                break;
            case DungeonSkillType.Boss_Sense:
                hasBossSense = true;
                DungeonPointer[] arrows = FindObjectsOfType<DungeonPointer>();
                foreach (DungeonPointer arrow in arrows)
                {
                    if (arrow.type == DungeonSkillType.Boss_Sense)
                    {
                        arrow.Enable();
                    }
                }
                break;
            case DungeonSkillType.Scroll_Sense:
                hasScrollSense = true;
                arrows = FindObjectsOfType<DungeonPointer>();
                foreach (DungeonPointer arrow in arrows)
                {
                    if (arrow.type == DungeonSkillType.Scroll_Sense)
                    {
                        arrow.Enable();
                    }
                }
                break;
            default:
                break;
        }
    }

    public static void DropWrappers(Vector3 position)
    {
        if (currentRoomWrappers)
        {
            currentRoomWrappers.DropWrapper(position);
        }
    }

    private void UpdateMapFields(Room room)
    {
        if ((int)room.transform.position.x < topLeft.x)
        {
            topLeft = new Vector2Int((int)room.transform.position.x, topLeft.y);
        }
        if ((int)room.transform.position.y > topLeft.y)
        {
            topLeft = new Vector2Int(topLeft.x, (int)room.transform.position.y);
        }

        if ((int)room.transform.position.x > bottomRight.x)
        {
            bottomRight = new Vector2Int((int)room.transform.position.x, bottomRight.y);
        }
        if ((int)room.transform.position.y < bottomRight.y)
        {
            bottomRight = new Vector2Int(bottomRight.x, (int)room.transform.position.y);
        }
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
