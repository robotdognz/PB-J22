using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        Destroy(gameObject);
    }
}
