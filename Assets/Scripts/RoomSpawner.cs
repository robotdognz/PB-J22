using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public enum Direction { Top, Right, Bottom, Left };
    public Direction openingDirection;

    private RoomTemplates templates;
    public bool spawned = false;

    private void Start()
    {
        templates = FindObjectOfType<RoomTemplates>();

        Invoke("Spawn", 0.5f);
    }

    void Spawn()
    {
        if (!spawned)
        {
            int rand;
            switch (openingDirection)
            {
                case Direction.Top:
                    // need to spawn a room with top door
                    rand = Random.Range(0, templates.topRooms.Length);
                    Instantiate(templates.topRooms[rand], transform.position, Quaternion.identity);
                    break;
                case Direction.Right:
                    // need to spawn a room with right door
                    rand = Random.Range(0, templates.rightRooms.Length);
                    Instantiate(templates.rightRooms[rand], transform.position, Quaternion.identity);
                    break;
                case Direction.Bottom:
                    // need to spawn a room with bottom door
                    rand = Random.Range(0, templates.bottomRooms.Length);
                    Instantiate(templates.bottomRooms[rand], transform.position, Quaternion.identity);
                    break;
                case Direction.Left:
                    // need to spawn a room with left door
                    rand = Random.Range(0, templates.leftRooms.Length);
                    Instantiate(templates.leftRooms[rand], transform.position, Quaternion.identity);
                    break;
            }
            spawned = true;
        }
        Debug.Log("Something");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint")) // && other.GetComponent<RoomSpawner>().spawned == true
        {
            if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                // make a blocked off room?
                Destroy(gameObject);
            }
            spawned = true;
        }
    }
}
