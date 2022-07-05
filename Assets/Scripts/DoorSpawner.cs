using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    [SerializeField] List<Room> parentRooms;

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

    public List<Room> GetParentRooms()
    {
        return parentRooms;
    }
}
