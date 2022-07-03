using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;
using Alchemy.Music;

public class DungeonManager : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [Tooltip("Set this to true to use the below settings, otherwise the game will decide")]
    public bool overwriteSettings = false;
    [Range(4, 30)] public int dungeonSize = 20;
    [Range(0f, 1f)] public float enemyProbability = 0.5f;
    [Range(1, 10)] public int enemyLevel = 1;
    public DungeonType dungeonType = DungeonType.Forest;
    // Dungeon theme

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

    private void Awake()
    {
        if (!overwriteSettings)
        {
            // TODO: replace dungeon settings with ones from the singleton
            dungeonSize = SettingsSingleton.instance.dungeonSize;
            enemyProbability = SettingsSingleton.instance.enemyProbability;
            enemyLevel = SettingsSingleton.instance.enemyLevel;
            dungeonType = SettingsSingleton.instance.dungeonType;
        }

        // set dungeon type
        MusicStarter musicStater = FindObjectOfType<MusicStarter>();
        musicStater.DungeonType = dungeonType;

        roomCount = dungeonSize - 1; // minus one to account for first room
        rooms = new List<Room>();
    }

    public int GetRemainingRooms()
    {
        return roomCount;
    }

    public void DecrementRemainingRooms()
    {
        roomCount--;
        if (roomCount <= 0)
        {
            Invoke("CloseRooms", 0.005f);
            Invoke("BuildDoors", 0.08f);
            Invoke("SetupEnd", 0.01f);
        }
    }

    public void CloseRooms()
    {
        // Debug.Log("Close up rooms");

        // remove spawn points
        RoomSpawner[] spawnPoints = FindObjectsOfType<RoomSpawner>();
        for (int i = spawnPoints.Length - 1; i >= 0; i--)
        {
            Destroy(spawnPoints[i].gameObject);
        }

        // close off rooms and add sprites
        foreach (Room room in rooms)
        {
            if (room.top && topWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(topWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                // Debug.Log("Made top wall");
                room.RemoveTopDoor();
            }
            if (room.right && rightWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(rightWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                // Debug.Log("Made right wall");
                room.RemoveRightDoor();
            }
            if (room.bottom && bottomWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(bottomWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                // Debug.Log("Made bottom wall");
                room.RemoveBottomDoor();
            }
            if (room.left && leftWall != null)
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
    }

    public void SetupEnd()
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
