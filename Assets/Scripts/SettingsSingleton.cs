using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Music;

public class SettingsSingleton : MonoBehaviour
{
    // singleton
    public static SettingsSingleton instance { get; private set; }

    // dungeon settings
    public int dungeonSize = 10;
    public float enemyProbability = 0.5f;
    public int enemyLevel = 1;
    public DungeonType dungeonType = DungeonType.Forest;

    private void Awake()
    {
        // setup singleton
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
}