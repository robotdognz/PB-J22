using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;

    public GameObject[] topRooms;
    public GameObject[] rightRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;

    [SerializeField] int roomCount = 20;

    public int GetRemainingRooms()
    {
        return roomCount;
    }

    public void DecrementRemainingRooms()
    {
        roomCount--;
        if (roomCount <= 0)
        {
            Invoke("FinalCheck", 0.5f);
        }
    }

    public void FinalCheck()
    {
        Debug.Log("Final Check");
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms)
        {
            // if (room.IsComplete())
            // {
            //     continue;
            // }
            if (room.top && topWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(topWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made top wall");
            }
            if (room.right && rightWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(rightWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made right wall");
            }
            if (room.bottom && bottomWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(bottomWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made bottom wall");
            }
            if (room.left && leftWall != null)
            {
                GameObject roomParent = room.gameObject.transform.parent.gameObject;
                GameObject newWall = Instantiate(leftWall, roomParent.transform.position, Quaternion.identity);
                newWall.transform.parent = roomParent.transform;
                Debug.Log("Made left wall");
            }
        }
    }

    // rooms should have references to their neighbors, graph structure. This will have lots of uses I can think of when I'm not absolutely wrecked.
}
