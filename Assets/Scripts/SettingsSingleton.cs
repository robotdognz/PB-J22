using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Music;

public class SettingsSingleton : MonoBehaviour
{
    // singleton
    public static SettingsSingleton instance { get; private set; }

    // dungeon settings
    public int dungeonSize = 5;
    public float enemyProbability = 0.5f;
    public int enemyLevel = 1;
    public int playerLevel = 1;
    public DungeonType dungeonType = DungeonType.Forest;

    private void Awake()
    {
        // setup singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        dungeonSize = 5;
        dungeonType = DungeonType.Forest;
        enemyProbability = 0.5f;
        enemyLevel = 1;
        playerLevel = 1;
    }

    // dungeon sizes
    public void SetDungeonSize01()
    {
        dungeonSize = 5;
    }
    public void SetDungeonSize02()
    {
        dungeonSize = 10;
    }
    public void SetDungeonSize03()
    {
        dungeonSize = 15;
    }

    // dungeon themes
    public void SetDungeonTheme01()
    {
        dungeonType = DungeonType.Forest;
    }
    public void SetDungeonTheme02()
    {
        dungeonType = DungeonType.Castle;
    }
    public void SetDungeonTheme03()
    {
        dungeonType = DungeonType.MidnightDesert;
    }
    public void SetDungeonTheme04()
    {
        dungeonType = DungeonType.Sewers;
    }

    // enemy difficulty
    public void SetDifficulty01()
    {
        enemyProbability = 0.5f;
        enemyLevel = 1;
    }
    public void SetDifficulty02()
    {
        enemyProbability = 0.75f;
        enemyLevel = 4;
    }
    public void SetDifficulty03()
    {
        enemyProbability = 0.9f;
        enemyLevel = 8;
    }

    // player strength (arcane focus)
    public void SetPlayer01()
    {
        playerLevel = 1;
    }
    public void SetPlayer02()
    {
        playerLevel = 4;
    }
    public void SetPlayer03()
    {
        playerLevel = 8;
    }

}
