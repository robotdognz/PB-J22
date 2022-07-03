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
        DungeonManager templates = FindObjectOfType<DungeonManager>();
        templates.rooms.Add(this);

        if (templates.IsPositionValid(transform.position))
        {
            templates.SpawnedCoords.Add(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        }
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
        //door.transform.parent = transform;

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
                foreach (Door door in FindObjectsOfType<Door>())
                {
                    door.isLocked = true;
                    door.CloseDoor();
                }

                Invoke("BattleStarted", 0.6f);
            }
            else
            {
                PlayerMovement.PreviousRoom = this;
            }
        }
    }

    public void BattleStarted()
    {
        Debug.Log("Battle!");
        EnemyLayout enemyWave = enemies.GetComponent<EnemyLayout>();
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

        bool boss = enemyWave.isBoss;

        BattleStarter.StartBattle(actors, !boss); // if it is a boss, then you can't flee
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


                if (enemies)
                {
                    // was this the boss?
                    bool boss = enemies.GetComponent<EnemyLayout>().isBoss;
                    if (boss)
                    {
                        Debug.Log("You win!");
                        // win condition
                    }

                    // clear enemies
                    RemoveEnemies();
                }

                // deactivate doors
                foreach (Door door in FindObjectsOfType<Door>()) // Changed code to fix an issue where they just wouldn't open again for some reason
                {
                    door.EndCombat();
                }

                PlayerMovement.PreviousRoom = this;
                break;
            case BattleEndResult.Defeat:
                PlayerMovement.PreviousRoom = null;
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                break;
            case BattleEndResult.Fled:
                PlayerMovement.Instance.EnablePlayer();
                EnemyLayout enemyWave = enemies.GetComponent<EnemyLayout>();

                foreach (Enemy E in enemyWave.GetEnemies())
                {
                    E.EnableEnemy();
                }

                // deactivate doors
                foreach (Door door in FindObjectsOfType<Door>()) // Changed code to fix an issue where they just wouldn't open again for some reason
                {
                    door.EndCombat();
                }

                StartCoroutine(MovePlayerToPreviousRoom());
                break;
        }
    }

    private IEnumerator MovePlayerToPreviousRoom()
    {
        while (PlayerMovement.Instance.transform.position != PlayerMovement.PreviousRoom.transform.position)
        {
            PlayerMovement.Instance.transform.position = Vector3.MoveTowards(PlayerMovement.Instance.transform.position, PlayerMovement.PreviousRoom.transform.position, (PlayerMovement.Instance.MoveSpeed * 2) * Time.deltaTime);
            yield return null;
        }
    }

    public void AddEnemies(GameObject enemyLayout)
    {
        enemies = enemyLayout;
    }

    public List<Enemy> GetIndividualEnemies()
    {
        if (enemies != null)
        {
            return enemies.GetComponent<EnemyLayout>().GetEnemies();
        }
        return null;
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
