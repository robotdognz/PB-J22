using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
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

    // game object
    [SerializeField] GameObject player;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject door;

    // dungeon generation
    [SerializeField] int dungeonSize = 20;
    private int roomCount;
    int rand;

    // keep track of rooms
    public List<Room> rooms;

    private void Awake()
    {
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
            Invoke("CloseRooms", 0.1f);
            Invoke("BuildDoors", 0.2f);
            Invoke("SetupPlayer", 0.3f);
        }
    }

    public void CloseRooms()
    {
        Debug.Log("Close up rooms");

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
                Debug.Log("Made top wall");
                room.RemoveTopDoor();
            }
            if (room.right && rightWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(rightWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made right wall");
                room.RemoveRightDoor();
            }
            if (room.bottom && bottomWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(bottomWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made bottom wall");
                room.RemoveBottomDoor();
            }
            if (room.left && leftWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(leftWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made left wall");
                room.RemoveLeftDoor();
            }

            // TODO: add sprite onto room
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

    public void SetupPlayer()
    {
        // add player to start room
        // GameObject newPlayer = Instantiate(player, rooms[0].transform.position, Quaternion.identity);

        // add end/boss to end room
        GameObject newBoss = Instantiate(boss, rooms[rooms.Count-1].transform.position, Quaternion.identity);
    }
}
