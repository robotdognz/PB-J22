using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchemy.Music;

public class TitleScreenLogic : MonoBehaviour
{
    [HideInInspector] int sceneIndex = 1;
    [SerializeField] Toggle[] dungeonSizeToggles;
    [SerializeField] Toggle[] dungeonThemeToggles;
    [SerializeField] Toggle[] enemyDifficultyToggles;
    [SerializeField] Toggle[] playerStrengthToggles;

    [HideInInspector] public SettingsSingleton settingsSingleton;

    private void Awake()
    {
        settingsSingleton = FindObjectOfType<SettingsSingleton>();

        // set default settings
        // settingsSingleton.dungeonSize = 5;
        // settingsSingleton.dungeonType = DungeonType.Forest;
        // settingsSingleton.enemyProbability = 0.5f;
        // settingsSingleton.enemyLevel = 1;
        // settingsSingleton.playerLevel = 1;

        // save settings between runs

        // dungeon size
        int dungeonSizeIndex = PlayerPrefs.GetInt("DungeonSize", 0);
        SetDungeonSize(dungeonSizeIndex);
        if (dungeonSizeToggles != null && dungeonSizeToggles.Length > dungeonSizeIndex)
        {
            Debug.Log("DungeonSize: " + dungeonSizeIndex);
            dungeonSizeToggles[dungeonSizeIndex].SetIsOnWithoutNotify(true);
        }

        // dungeon theme
        int dungeonThemeIndex = PlayerPrefs.GetInt("DungeonTheme", 0);
        SetDungeonSize(dungeonThemeIndex);
        if (dungeonThemeToggles != null && dungeonThemeToggles.Length > dungeonThemeIndex)
        {
            Debug.Log("DungeonTheme: " + dungeonThemeIndex);
            dungeonThemeToggles[dungeonThemeIndex].SetIsOnWithoutNotify(true);
        }

        // enemy difficulty
        int enemyDifficultyIndex = PlayerPrefs.GetInt("EnemyDifficulty", 0);
        SetDungeonSize(enemyDifficultyIndex);
        if (enemyDifficultyToggles != null && enemyDifficultyToggles.Length > enemyDifficultyIndex)
        {
            Debug.Log("EnemyDifficulty: " + enemyDifficultyIndex);
            enemyDifficultyToggles[enemyDifficultyIndex].SetIsOnWithoutNotify(true);
        }

        // player strength
        int playerStrengthIndex = PlayerPrefs.GetInt("PlayerStrength", 0);
        SetDungeonSize(playerStrengthIndex);
        if (playerStrengthToggles != null && playerStrengthToggles.Length > playerStrengthIndex)
        {
            Debug.Log("PlayerStrength: " + playerStrengthIndex);
            playerStrengthToggles[playerStrengthIndex].SetIsOnWithoutNotify(true);
        }
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    // dungeon size
    private void SetDungeonSize(int index)
    {
        switch (index)
        {
            case 0:
                settingsSingleton.dungeonSize = 5;
                break;
            case 1:
                settingsSingleton.dungeonSize = 10;
                break;
            case 2:
                settingsSingleton.dungeonSize = 15;
                break;
            default:
                break;

        }
    }
    public void SetDungeonSize0()
    {
        int index = 0;
        PlayerPrefs.SetInt("DungeonSize", index);
        SetDungeonSize(index);
    }
    public void SetDungeonSize1()
    {
        int index = 1;
        PlayerPrefs.SetInt("DungeonSize", index);
        SetDungeonSize(index);
    }
    public void SetDungeonSize2()
    {
        int index = 2;
        PlayerPrefs.SetInt("DungeonSize", index);
        SetDungeonSize(index);
    }

    // dungeon theme
    private void SetDungeonTheme(int index)
    {
        switch (index)
        {
            case 0:
                settingsSingleton.dungeonType = DungeonType.Forest;
                break;
            case 1:
                settingsSingleton.dungeonType = DungeonType.Castle;
                break;
            case 2:
                settingsSingleton.dungeonType = DungeonType.MidnightDesert;
                break;
            case 3:
                settingsSingleton.dungeonType = DungeonType.Sewers;
                break;
            default:
                break;

        }
    }
    public void SetDungeonTheme0()
    {
        int index = 0;
        PlayerPrefs.SetInt("DungeonTheme", index);
        SetDungeonTheme(index);
    }
    public void SetDungeonTheme1()
    {
        int index = 1;
        PlayerPrefs.SetInt("DungeonTheme", index);
        SetDungeonTheme(index);
    }
    public void SetDungeonTheme2()
    {
        int index = 2;
        PlayerPrefs.SetInt("DungeonTheme", index);
        SetDungeonTheme(index);
    }
    public void SetDungeonTheme3()
    {
        int index = 3;
        PlayerPrefs.SetInt("DungeonTheme", index);
        SetDungeonTheme(index);
    }

    // enemy difficulty
    private void SetEnemyDifficulty(int index)
    {
        switch (index)
        {
            case 0:
                settingsSingleton.enemyProbability = 0.5f;
                settingsSingleton.enemyLevel = 1;
                break;
            case 1:
                settingsSingleton.enemyProbability = 0.75f;
                settingsSingleton.enemyLevel = 4;
                break;
            case 2:
                settingsSingleton.enemyProbability = 0.9f;
                settingsSingleton.enemyLevel = 8;
                break;
            default:
                break;

        }
    }
    public void SetEnemyDifficulty0()
    {
        int index = 0;
        PlayerPrefs.SetInt("EnemyDifficulty", index);
        SetEnemyDifficulty(index);
    }
    public void SetEnemyDifficulty1()
    {
        int index = 1;
        PlayerPrefs.SetInt("EnemyDifficulty", index);
        SetEnemyDifficulty(index);
    }
    public void SetEnemyDifficulty2()
    {
        int index = 2;
        PlayerPrefs.SetInt("EnemyDifficulty", index);
        SetEnemyDifficulty(index);
    }

    // player strength
    private void SetPlayerStrength(int index)
    {
        switch (index)
        {
            case 0:
                settingsSingleton.playerLevel = 1;
                break;
            case 1:
                settingsSingleton.playerLevel = 4;
                break;
            case 2:
                settingsSingleton.playerLevel = 8;
                break;
            default:
                break;

        }
    }
    public void SetPlayerStrength0()
    {
        int index = 0;
        PlayerPrefs.SetInt("PlayerStrength", index);
        SetPlayerStrength(index);
    }
    public void SetPlayerStrength1()
    {
        int index = 1;
        PlayerPrefs.SetInt("PlayerStrength", index);
        SetPlayerStrength(index);
    }
    public void SetPlayerStrength2()
    {
        int index = 2;
        PlayerPrefs.SetInt("PlayerStrength", index);
        SetPlayerStrength(index);
    }
}
