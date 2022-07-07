using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public enum Direction { Top, Right, Bottom, Left };
    public Direction openingDirection;

    private DungeonManager dungeonManager;
    int rand;

    private void Awake()
    {
        dungeonManager = FindObjectOfType<DungeonManager>();
    }

    public GameObject Spawn()
    {
        GameObject spawnedRoom = null;

        // spawn room
        switch (openingDirection)
        {
            case Direction.Top:
                // need to spawn a room with top door
                rand = Random.Range(0, dungeonManager.topRooms.Length);
                spawnedRoom = Instantiate(dungeonManager.topRooms[rand], transform.position, Quaternion.identity);
                break;
            case Direction.Right:
                // need to spawn a room with right door
                rand = Random.Range(0, dungeonManager.rightRooms.Length);
                spawnedRoom = Instantiate(dungeonManager.rightRooms[rand], transform.position, Quaternion.identity);
                break;
            case Direction.Bottom:
                // need to spawn a room with bottom door
                rand = Random.Range(0, dungeonManager.bottomRooms.Length);
                spawnedRoom = Instantiate(dungeonManager.bottomRooms[rand], transform.position, Quaternion.identity);
                break;
            case Direction.Left:
                // need to spawn a room with left door
                rand = Random.Range(0, dungeonManager.leftRooms.Length);
                spawnedRoom = Instantiate(dungeonManager.leftRooms[rand], transform.position, Quaternion.identity);
                break;
            default:
                return null; // Break the code if the direction is invalid
        }

        return spawnedRoom;
    }
}
