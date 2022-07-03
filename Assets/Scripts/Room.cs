using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy;

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

    public static Vector3 TargetPos { get; private set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // snap camera to this room
            TargetPos = transform.position + -Vector3.forward * 10;

            // if the room has enemies, snap player to this room (so they are't inside the door when it closes)
            if (enemies != null)
            {
                Rigidbody2D playerBody = other.gameObject.GetComponentInParent<Rigidbody2D>();
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

                Invoke("BattleStarted", 0.6f);
            }
        }
    }

    public void BattleStarted()
    {
        Debug.Log("Battle!");
        EnemyLayout enemyWave = enemies.GetComponent<EnemyLayout>(); ;
        List<Enemy> enemiesInWave = enemyWave.GetEnemies();

        List<Alchemy.Stats.ActorStats> actors = new List<Alchemy.Stats.ActorStats>();

        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
        actors.Add(player.GetComponent<Alchemy.Stats.ActorStats>());
        player.GetComponent<PlayerMovement>().DisablePlayer();

        foreach (Enemy enemy in enemiesInWave)
        {
            Alchemy.Stats.ActorStats current = enemy.gameObject.GetComponent<Alchemy.Stats.ActorStats>();
            if (current != null)
            {
                actors.Add(current);
            }
            enemy.DisableEnemy();
        }

        BattleStarter.StartBattle(actors);
        BattleStarter.OnBattleEnd += BattleEnded;
    }

    public void BattleEnded(BattleEndResult result)
    {
        // Sky here, just added a condition so that if it's a game over the game will reload. Might add a GameOver screen if there's enough time!

        switch (result) 
        {
            default:
                // restore player
                GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
                player.GetComponent<PlayerMovement>().EnablePlayer();

                // clear enemies
                RemoveEnemies();

                // deactivate doors
                foreach (Door door in childDoors)
                {
                    door.EndCombat();
                }
                break;
            case BattleEndResult.Defeat:
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                break;
        }
    }

    public void AddEnemies(GameObject enemyLayout)
    {
        enemies = enemyLayout;
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
