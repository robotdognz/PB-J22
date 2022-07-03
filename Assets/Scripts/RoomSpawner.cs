using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public enum Direction { Top, Right, Bottom, Left };
    public Direction openingDirection;

    private DungeonManager dungeonManager;
    public bool spawned = false;
    int rand;

    private void Start()
    {
        dungeonManager = FindObjectOfType<DungeonManager>();

        Invoke("Spawn", 0.001f);
    }

    void Spawn()
    {
        if (!spawned && dungeonManager.GetRemainingRooms() > 0)
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
            }
            spawned = true;
            dungeonManager.DecrementRemainingRooms();

            // spawn enemies in room
            if (Random.value <= dungeonManager.enemyProbability) // this shouldn't be hard coded
            {
                rand = Random.Range(0, dungeonManager.enemyLayouts.Length);
                GameObject enemies = Instantiate(dungeonManager.enemyLayouts[rand], transform.position, Quaternion.identity);
                if (spawnedRoom != null)
                {
                    spawnedRoom.GetComponentInChildren<Room>().AddEnemies(enemies); //.enemies = enemies;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint") || other.CompareTag("Room"))
        {
            if (other.GetComponent<Room>() != null)
            {
                Room room = other.GetComponent<Room>();
                // this spawn point was created on top of an existing room
                // tell the existing room about this room
                switch (openingDirection)
                {
                    case Direction.Top:
                        room.top = false;
                        break;
                    case Direction.Right:
                        room.right = false;
                        break;
                    case Direction.Bottom:
                        room.bottom = false;
                        break;
                    case Direction.Left:
                        room.left = false;
                        break;
                }
            }
            spawned = true;
        }
    }
}
