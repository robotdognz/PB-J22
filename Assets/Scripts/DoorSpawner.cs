using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    [SerializeField] List<Room> parentRooms;
    [HideInInspector] public bool isDestroyed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DoorSpawner"))
        {
            // only destroy one of the two door spawners
            DoorSpawner otherDoorSpawner = other.gameObject.GetComponent<DoorSpawner>();
            if (!otherDoorSpawner.isDestroyed)
            {
                isDestroyed = true;
                otherDoorSpawner.AddRooms(parentRooms); // pass the surviving spawner this ones parents
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    public void AddRooms(List<Room> rooms)
    {
        parentRooms.AddRange(rooms);
    }

    public List<Room> GetParentRooms()
    {
        return parentRooms;
    }
}
