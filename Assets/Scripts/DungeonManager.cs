using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;
using Alchemy.Music;
using Alchemy.Combat;

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
    // Dungeon theme

    [Header("Randomizer")]
    public bool EnableRandomizer = true;
    public int MinimumSize = 10;
    public int MaximumSize = 50;
    public float MinimumEnemyProbability = 0.25f;
    public float MaximumEnemyProbability = 0.7f;
    public int MinimumEnemyLevel = 0;
    public int MaximumEnemyLevel = 3;

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

        if (EnableRandomizer)
        {
            dungeonSize = Random.Range(MinimumSize, MaximumSize);
            enemyProbability = Random.Range(MinimumEnemyProbability, MaximumEnemyProbability);
            enemyLevel = Random.Range(MinimumEnemyLevel, MaximumEnemyLevel);
            dungeonType = (DungeonType)Random.Range(0, 4);
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

        ConstructDungeon();

        // set player level
        FindObjectOfType<PlayerMovement>().GetComponent<ActorStats>().CurrentLevel = playerLevel;
    }

    public void ConstructDungeon()
    {
        Debug.Log("Build dungeon");
        // setup spawn queue
        Queue<RoomSpawner> spawnQueue = new Queue<RoomSpawner>();

        // get initial spawners and queue them
        RoomSpawner[] initialSpawners = FindObjectsOfType<RoomSpawner>();
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
                        RoomSpawner[] tempSpawners = spawnedRoom.GetComponentsInChildren<RoomSpawner>(); // get the new spawners
                        // Debug.Log("Spawners found in room: " + tempSpawners.Length);
                        foreach (RoomSpawner spawner in tempSpawners) // enqueue them
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

        FinishBuild();
    }

    public void FinishBuild()
    {
        CloseRoomsAndAddEnemies();
        SetupLevelEnd();
        Invoke("BuildDoors", 1f);
    }

    public int GetRemainingRooms()
    {
        return roomCount;
    }

    public void DecrementRemainingRooms()
    {
        roomCount--;
    }

    public void CloseRoomsAndAddEnemies()
    {
        // Debug.Log("Close up rooms");

        // close off rooms and add sprites
        foreach (Room room in rooms)
        {
            if (room.top)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(topWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made top wall");
                room.RemoveTopDoor();
            }
            if (room.right)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(rightWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made right wall");
                room.RemoveRightDoor();
            }
            if (room.bottom)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(bottomWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made bottom wall");
                room.RemoveBottomDoor();
            }
            if (room.left)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(leftWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made left wall");
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
        // add doors
        DoorSpawner[] doorSpawners = FindObjectsOfType<DoorSpawner>();
        for (int i = doorSpawners.Length - 1; i >= 0; i--)
        {
            // get the door spawner
            DoorSpawner ds = doorSpawners[i];

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
            //add rooms to door
            tempDoor.AddRooms(parentRooms);

            // Cleanup
            Destroy(ds.gameObject);
        }

        GameObject.Find("[DARKINATOR]").SetActive(false);
    }

    public void SetupLevelEnd()
    {
        // add end/boss to end room
        if (rooms[rooms.Count - 1].HasEnemies())
        {
            rooms[rooms.Count - 1].RemoveEnemies();
        }
        GameObject enemies = Instantiate(bossLayout, rooms[rooms.Count - 1].transform.position, Quaternion.identity);

        // setup boss level
        List<Enemy> bossWave = enemies.GetComponent<EnemyLayout>().GetEnemies();
        foreach (Enemy enemy in bossWave)
        {
            enemy.gameObject.GetComponent<ActorStats>().CurrentLevel = enemyLevel;
        }

        rooms[rooms.Count - 1].AddEnemies(enemies);
    }
}
