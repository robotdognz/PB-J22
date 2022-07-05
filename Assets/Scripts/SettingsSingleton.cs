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

    // win or lose? used by title screen
    public TitleScreenMessage titleScreenState = TitleScreenMessage.Normal;

    public enum TitleScreenMessage 
    { 
        Normal,
        Quit,
        Lose,
        Win
    }

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
    }
}
