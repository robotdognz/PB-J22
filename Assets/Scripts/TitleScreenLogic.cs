using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Music;

public class TitleScreenLogic : MonoBehaviour
{
    [SerializeField] int sceneIndex = 1;
    public SettingsSingleton settingsSingleton;

    private void Start() {
        settingsSingleton = FindObjectOfType<SettingsSingleton>();
        // set default settings
        settingsSingleton.dungeonSize = 5;
        settingsSingleton.dungeonType = DungeonType.Forest;
        settingsSingleton.enemyProbability = 0.5f;
        settingsSingleton.enemyLevel = 1;
        settingsSingleton.playerLevel = 1;

        // todo: make the title screen remember choices between runs
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    // dungeon sizes
    public void SetDungeonSize01()
    {
        settingsSingleton.dungeonSize = 5;
    }
    public void SetDungeonSize02()
    {
        settingsSingleton.dungeonSize = 10;
    }
    public void SetDungeonSize03()
    {
        settingsSingleton.dungeonSize = 15;
    }

    // dungeon themes
    public void SetDungeonTheme01()
    {
        settingsSingleton.dungeonType = DungeonType.Forest;
    }
    public void SetDungeonTheme02()
    {
        settingsSingleton.dungeonType = DungeonType.Castle;
    }
    public void SetDungeonTheme03()
    {
        settingsSingleton.dungeonType = DungeonType.MidnightDesert;
    }
    public void SetDungeonTheme04()
    {
        settingsSingleton.dungeonType = DungeonType.Sewers;
    }

    // enemy difficulty
    public void SetDifficulty01()
    {
        settingsSingleton.enemyProbability = 0.5f;
        settingsSingleton.enemyLevel = 1;
    }
    public void SetDifficulty02()
    {
        settingsSingleton.enemyProbability = 0.75f;
        settingsSingleton.enemyLevel = 4;
    }
    public void SetDifficulty03()
    {
        settingsSingleton.enemyProbability = 0.9f;
        settingsSingleton.enemyLevel = 8;
    }

    // player strength (arcane focus)
    public void SetPlayer01()
    {
        settingsSingleton.playerLevel = 1;
    }
    public void SetPlayer02()
    {
        settingsSingleton.playerLevel = 4;
    }
    public void SetPlayer03()
    {
        settingsSingleton.playerLevel = 8;
    }
}
