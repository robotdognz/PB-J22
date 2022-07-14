using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy;
using Alchemy.Inventory;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    public static UnityAction PlayerEnter;

    // starting doors for room
    [SerializeField] DoorSpawner doorTop;
    [SerializeField] DoorSpawner doorRight;
    [SerializeField] DoorSpawner doorBottom;
    [SerializeField] DoorSpawner doorLeft;

    // currently required rooms
    public bool top;
    public bool right;
    public bool bottom;
    public bool left;

    [SerializeField] GameObject mapRoom;

    [SerializeField] List<Door> childDoors;

    private GameObject enemies = null;
    private GameObject chests = null;
    private DungeonPointer roomArrow = null;
    [SerializeField] GameObject dungeonArrowPrefab;

    float roomDiameter = 3.5f;

    private string winScene = "WinScreen"; // scene to load when beating final boss

    public static void ResetPlayerEnter()
    {
        PlayerEnter = null;
    }

    protected virtual void OnPlayerEnter()
    {
        // draw this room and it's doors on the map
        mapRoom.GetComponent<SpriteRenderer>().color = Color.cyan;
        EnableDoorsOnMap();

        // store current room position
        DungeonManager.currentRoom = transform.position;

        // trigger event
        PlayerEnter.Invoke();
    }

    protected virtual void OnPlayerExit()
    {
        if (DungeonManager.hasCartographer)
        {
            mapRoom.GetComponent<SpriteRenderer>().color = Color.gray;
        }
        else
        {
            mapRoom.GetComponent<SpriteRenderer>().color = Color.black;
            DisableDoorsOnMap();
        }
    }

    public void Init()
    {
        // setup required neighbors for this room
        top = doorTop;
        right = doorRight;
        bottom = doorBottom;
        left = doorLeft;
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
        if (doorTop)
        {
            Destroy(doorTop.gameObject);
            doorTop = null;
        }
        top = false;
    }

    public void RemoveRightDoor()
    {
        if (doorRight)
        {
            Destroy(doorRight.gameObject);
            doorRight = null;
        }
        right = false;
    }

    public void RemoveBottomDoor()
    {
        if (doorBottom)
        {
            Destroy(doorBottom.gameObject);
            doorBottom = null;
        }
        bottom = false;
    }

    public void RemoveLeftDoor()
    {
        if (doorLeft)
        {
            Destroy(doorLeft.gameObject);
            doorLeft = null;
        }
        left = false;
    }

    public void AddDoor(Door door)
    {
        if (!childDoors.Contains(door))
        {
            childDoors.Add(door);
        }
    }

    public DoorSpawner GetDoorSpawner(int index)
    {
        DoorSpawner result = null;
        switch (index)
        {
            case 0:
                result = doorTop;
                break;
            case 1:
                result = doorRight;
                break;
            case 2:
                result = doorBottom;
                break;
            case 3:
                result = doorLeft;
                break;
            default:
                break;

        }
        return result;
    }

    public static Vector3 TargetPos { get; private set; }

    public void EnableDoorsOnMap()
    {
        // draw doors on map
        foreach (Door door in childDoors)
        {
            door.EnableDoorOnMap();
        }
    }
    public void DisableDoorsOnMap()
    {
        // draw doors on map
        foreach (Door door in childDoors)
        {
            door.DisableDoorOnMap();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // snap camera to this room
            TargetPos = transform.position + -Vector3.forward * 10;

            // tell the dungeon manager this is the current candy room
            DungeonManager.currentRoomWrappers = transform.parent.GetComponentInChildren<CandyTrailRoom>();

            if (roomArrow != null)
            {
                roomArrow.gameObject.SetActive(false);
            }

            // if the room has enemies, snap player to this room (so they are't inside the door when it closes)
            if (enemies != null)
            {
                if (DungeonManager.hasCartographer)
                {
                    EnemyLayout layout = enemies.GetComponent<EnemyLayout>();
                    layout.ActivateMarker(); // mark this room on the map
                }

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
            else
            {
                PlayerMovement.PreviousRoom = this;
            }

            OnPlayerEnter();
        }
    }

    protected void OnTriggerExit2D(Collider2D Other)
    {
        if (Other.CompareTag("Player"))
        {
            if (roomArrow != null)
            {
                if (ChestsCleared())
                {
                    RemoveArrow();
                }
                else
                {
                    roomArrow.gameObject.SetActive(true);
                }
            }

            OnPlayerExit();
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

        BattleStarter.StartBattle(actors, true, boss); // this is where you can disable flee
        BattleStarter.OnBattleEnd += BattleEnded;
    }

    public void WinGame()
    {
        Room.ResetPlayerEnter();
        UnityEngine.SceneManagement.SceneManager.LoadScene(winScene);
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
                        // win condition
                        Debug.Log("You win!");
                        Invoke("WinGame", .2f);
                        break;
                    }

                    // clear enemies
                    RemoveEnemies();
                }

                // deactivate doors
                foreach (Door door in childDoors)
                {
                    door.EndCombat();
                }

                PlayerMovement.PreviousRoom = this;
                break;
            case BattleEndResult.Defeat:
                PlayerMovement.PreviousRoom = null;
                // tell the title screen to display the lose message
                SettingsSingleton.instance.titleScreenState = SettingsSingleton.TitleScreenMessage.Lose;
                // load title screen
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
                foreach (Door door in childDoors)
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

    // enemies
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

    // chests
    public void AddChests(GameObject chestLayout)
    {
        chests = chestLayout;
    }
    public bool HasChests()
    {
        return chests != null;
    }
    public void RemoveChests()
    {
        Destroy(chests);
        chests = null;
    }
    public bool ChestsCleared()
    {
        if (chests != null)
        {
            Chest[] individualChests = chests.GetComponentsInChildren<Chest>();
            foreach (Chest chest in individualChests)
            {
                if (!chest.isUsed)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }




    // arrow
    public void AddArrowToRoom(Color color, DungeonManager.DungeonSkillType type, float scale)
    {
        DungeonPointer arrow = Instantiate(dungeonArrowPrefab, transform.position, Quaternion.identity).GetComponent<DungeonPointer>(); ;
        arrow.SetColor(color);
        arrow.SetScale(scale);
        arrow.SetType(type);
        arrow.SetPointingTo(transform.position);
        arrow.Disable();
        roomArrow = arrow;
    }
    public bool HasArrow()
    {
        return roomArrow != null;
    }
    public void RemoveArrow()
    {
        Destroy(roomArrow.gameObject);
        roomArrow = null;
    }

}
