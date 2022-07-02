using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // current doors
    [SerializeField] bool doorTop;
    [SerializeField] bool doorRight;
    [SerializeField] bool doorBottom;
    [SerializeField] bool doorLeft;

    // required rooms
    [HideInInspector] public bool top;
    [HideInInspector] public bool right;
    [HideInInspector] public bool bottom;
    [HideInInspector] public bool left;

    [SerializeField] List<Door> childDoors;

    private GameObject enemies = null;

    float roomDiameter = 3.5f;

    private void Awake()
    {
        // setup required rooms
        top = doorTop;
        right = doorRight;
        bottom = doorBottom;
        left = doorLeft;
    }

    private void Start()
    {
        RoomTemplates templates = FindObjectOfType<RoomTemplates>();
        templates.rooms.Add(this);
    }

    public bool IsComplete()
    {
        if (!top && !right && !bottom && !left)
        {
            return true;
        }
        return false;
    }

    public void RemoveTopDoor()
    {
        doorTop = false;
        top = false;
    }

    public void RemoveRightDoor()
    {
        doorRight = false;
        right = false;
    }

    public void RemoveBottomDoor()
    {
        doorBottom = false;
        bottom = false;
    }

    public void RemoveLeftDoor()
    {
        doorLeft = false;
        left = false;
    }

    public void AddDoor(Door door)
    {
        if (!childDoors.Contains(door))
        {
            childDoors.Add(door);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerBody = other.gameObject.GetComponentInParent<Rigidbody2D>();
            // snap camera to this room
            Vector3 cameraTemp = Camera.main.transform.position;
            cameraTemp.x = transform.position.x;
            cameraTemp.y = transform.position.y;
            Camera.main.transform.position = cameraTemp;

            // if the room has enemies, snap player to this room (so they are't inside the door when it closes)
            if (enemies != null)
            {
                Vector2 playerTemp = other.transform.position;

                if (Mathf.Abs(playerTemp.x - transform.position.x) > Mathf.Abs(playerTemp.y - transform.position.y))
                {
                    // snap on x axis to edge of room
                    if (playerTemp.x < transform.position.x)
                    {
                        // Debug.Log("x snap, to left side of room");
                        playerTemp.x = transform.position.x - roomDiameter;
                        playerTemp.y = playerBody.transform.position.y;
                        playerBody.transform.position = playerTemp;
                    }
                    else
                    {
                        // Debug.Log("x snap, to right side of room");
                        playerTemp.x = transform.position.x + roomDiameter;
                        playerTemp.y = playerBody.transform.position.y;
                        playerBody.transform.position = playerTemp;
                    }
                }
                else
                {
                    // snap on y axis to edge of room
                    if (playerTemp.y < transform.position.y)
                    {
                        // Debug.Log("y snap, to bottom of room");
                        playerTemp.x = playerBody.transform.position.x;
                        playerTemp.y = transform.position.y - roomDiameter;
                        playerBody.transform.position = playerTemp;

                    }
                    else
                    {
                        // Debug.Log("y snap, to top of room");
                        playerTemp.x = playerBody.transform.position.x;
                        playerTemp.y = transform.position.y + roomDiameter;
                        playerBody.transform.position = playerTemp;
                    }
                }

                // activate doors
                foreach (Door door in childDoors)
                {
                    door.isLocked = true;
                    door.CloseDoor();
                }
            }
        }
    }

    public void RoomCleared()
    {
        Debug.Log("Room Cleared");
        RemoveEnemies();
        // deactivate doors
        foreach (Door door in childDoors)
        {
            door.EndCombat();
        }
    }

    public void AddEnemies(GameObject enemyLayout)
    {
        enemies = enemyLayout;
        enemies.GetComponent<EnemyLayout>().EnemiesDefeated += RoomCleared;
    }

    public bool HasEnemies()
    {
        return enemies != null;
    }

    public void RemoveEnemies()
    {
        Destroy(enemies);
        enemies = null;
    }

}
