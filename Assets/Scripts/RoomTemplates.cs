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
            Invoke("FinalCheck", 0.1f);
        }
    }

    public void FinalCheck()
    {
        Debug.Log("Final Check");

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
                // room.top = false;
                room.RemoveTopDoor();
            }
            if (room.right && rightWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(rightWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made right wall");
                // room.right = false;
                room.RemoveRightDoor();
            }
            if (room.bottom && bottomWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(bottomWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made bottom wall");
                // room.bottom = false;
                room.RemoveBottomDoor();
            }
            if (room.left && leftWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(leftWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made left wall");
                // room.left = false;
                room.RemoveLeftDoor();
            }

            // TODO: add sprite onto room
        }

        // TODO: add player to start room

        // TODO: add end/boss to end room
    }
}
