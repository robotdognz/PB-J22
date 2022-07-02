using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] List<Room> parentRooms;

    public void AddRooms(List<Room> rooms)
    {
        parentRooms.AddRange(rooms);
    }
}
