using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy;

public class Enemy : MonoBehaviour
{
    bool isDisabled = false;
    SpriteRenderer sRenderer;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (isDisabled)
    //     {
    //         return;
    //     }

    //     if (other.CompareTag("Player"))
    //     {
    //         Debug.Log("Combat starts!");

    //         // temp kill the enemy immediately, replace this with Sky's combat system
    //         // Kill();

    //         DisableEnemy();
    //         other.GetComponentInParent<PlayerMovement>().DisablePlayer();

    //         Alchemy.Stats.ActorStats player = other.GetComponentInParent<Alchemy.Stats.ActorStats>();
    //         Alchemy.Stats.ActorStats me = GetComponent<Alchemy.Stats.ActorStats>();
    //         if (me != null && player != null)
    //         {
    //             List<Alchemy.Stats.ActorStats> actors = new List<Alchemy.Stats.ActorStats>();
    //             actors.Add(player);
    //             actors.Add(me);
    //             BattleStarter.StartBattle(actors);
    //         }
    //     }

    // }

    public void DisableEnemy()
    {
        isDisabled = true;
        sRenderer.enabled = false;
    }

    public void EnableEnemy()
    {
        isDisabled = false;
        sRenderer.enabled = true;
    }

    public void Kill()
    {
        GetComponentInParent<EnemyLayout>().KillEnemy(this);
    }
}
