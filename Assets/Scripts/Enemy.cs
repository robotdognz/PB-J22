using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;

public class Enemy : MonoBehaviour
{
    SpriteRenderer sRenderer;
    [SerializeField] bool isBoss = false;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();

        if (!isBoss)
        {
            // Get a random enemy from the current enemy palette. Allows different dungeons to have different enemies
            GetComponent<ActorStats>().Stats = DungeonManager.ActiveEnemyPalette.GetEnemy;
        }
        GetComponent<ActorStats>().InitializeActor();
        GetComponent<ActorStats>().ResetStats();
    }

    public void DisableEnemy()
    {
        foreach (SpriteRenderer R in GetComponentsInChildren<SpriteRenderer>())
        {
            R.enabled = false;
        }
    }

    public void EnableEnemy()
    {
        foreach (SpriteRenderer R in GetComponentsInChildren<SpriteRenderer>())
        {
            R.enabled = true;
        }
    }
}
