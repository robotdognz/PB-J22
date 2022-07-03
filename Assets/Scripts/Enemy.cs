using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;

public class Enemy : MonoBehaviour
{
    SpriteRenderer sRenderer;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();

        // Get a random enemy from the current enemy palette. Allows different dungeons to have different enemies
        GetComponent<ActorStats>().Stats = DungeonManager.ActiveEnemyPalette.GetEnemy;
        GetComponent<ActorStats>().ResetStats();
        GetComponent<ActorStats>().InitializeActor();
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
