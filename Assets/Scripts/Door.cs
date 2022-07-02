using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject doorBody;
    [SerializeField] List<Room> parentRooms;

    public bool isLocked = false;

    public void AddRooms(List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            if (!parentRooms.Contains(room))
            {
                parentRooms.Add(room);
            }
        }
    }

    public void CloseDoor()
    {
        doorBody.SetActive(true);
    }

    public void OpenDoor()
    {
        if (isLocked)
        {
            return;
        }
        doorBody.SetActive(false);
    }
}
