using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;

    // public GameObject topRoomClosed;
    // public GameObject rightRoomClosed;
    // public GameObject bottomRoomClosed;
    // public GameObject leftRoomClosed;
    public GameObject[] topRooms;
    public GameObject[] rightRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;


    [SerializeField] int dungeonSize = 20;
    private int roomCount;

    int rand;

    // public GameObject GetTopRoom()
    // {
    //     rand = Random.Range(0, templates.topRooms.Length);

    // }

    private void Start()
    {
        roomCount = dungeonSize - 1;
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
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms)
        {
            if (room.IsComplete())
            {
                // add sprite onto room
            }
            if (room.top && topWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(topWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made top wall");
                room.top = false;
            }
            if (room.right && rightWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(rightWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made right wall");
                room.right = false;
            }
            if (room.bottom && bottomWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(bottomWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made bottom wall");
                room.bottom = false;
            }
            if (room.left && leftWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(leftWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made left wall");
                room.left = false;
            }

            // add sprite onto room
        }
    }
}
