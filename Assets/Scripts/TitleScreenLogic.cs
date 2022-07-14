using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchemy.Music;
using System.Runtime.InteropServices;

public class TitleScreenLogic : MonoBehaviour
{
    [HideInInspector] int sceneIndex = 1;
    [Header("Ingredient Toggles")]
    [SerializeField] Toggle[] dungeonSizeToggles;
    [SerializeField] Toggle[] dungeonThemeToggles;
    [SerializeField] Toggle[] enemyDifficultyToggles;
    [SerializeField] Toggle[] playerStrengthToggles;

    [Header("Alchemist Dialogue")]
    [SerializeField] Text alchemistDialogue;
    [SerializeField] [TextArea] string normalDialogue = "";
    [SerializeField] [TextArea] string quitDialogue = "";
    [SerializeField] [TextArea] string loseDialogue = "";
    [SerializeField] [TextArea] string winDialogue = "";

    [HideInInspector] public SettingsSingleton settingsSingleton;

    [DllImport("__Internal")]
    private static extern bool IsMobile();
    [SerializeField] PlayerPrefsToggle mobileToggle;

    private void Awake()
    {
        // restart time (restarting it in the pause menu doesn't work)
        Time.timeScale = 1;

        // get settings
        settingsSingleton = SettingsSingleton.instance;

        // run title screen message
        Debug.Log(settingsSingleton.titleScreenState);
        switch (settingsSingleton.titleScreenState)
        {
            case SettingsSingleton.TitleScreenMessage.Normal:
                // normal dialogue
                alchemistDialogue.text = normalDialogue;
                break;
            case SettingsSingleton.TitleScreenMessage.Quit:
                // quit dialogue
                alchemistDialogue.text = quitDialogue;
                break;
            case SettingsSingleton.TitleScreenMessage.Lose:
                // lose dialogue
                alchemistDialogue.text = loseDialogue;
                break;
            case SettingsSingleton.TitleScreenMessage.Win:
                // win dialogue
                alchemistDialogue.text = winDialogue;
                break;
        }
        settingsSingleton.titleScreenState = SettingsSingleton.TitleScreenMessage.Normal;

        LoadSettings();

        // set mobile controls to on if using a mobile device
        if (isMobile())
        {
            mobileToggle.GetComponent<Toggle>().isOn = true;
        }

    }

    private void LoadSettings()
    {
        // load previous settings

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
        SetDungeonTheme(dungeonThemeIndex);
        if (dungeonThemeToggles != null && dungeonThemeToggles.Length > dungeonThemeIndex)
        {
            // Debug.Log("DungeonTheme: " + dungeonThemeIndex);
            dungeonThemeToggles[dungeonThemeIndex].SetIsOnWithoutNotify(true);
        }

        // enemy difficulty
        int enemyDifficultyIndex = PlayerPrefs.GetInt("EnemyDifficulty", 0);
        SetEnemyDifficulty(enemyDifficultyIndex);
        if (enemyDifficultyToggles != null && enemyDifficultyToggles.Length > enemyDifficultyIndex)
        {
            // Debug.Log("EnemyDifficulty: " + enemyDifficultyIndex);
            enemyDifficultyToggles[enemyDifficultyIndex].SetIsOnWithoutNotify(true);
        }

        // player strength
        int playerStrengthIndex = PlayerPrefs.GetInt("PlayerStrength", 0);
        SetPlayerStrength(playerStrengthIndex);
        if (playerStrengthToggles != null && playerStrengthToggles.Length > playerStrengthIndex)
        {
            // Debug.Log("PlayerStrength: " + playerStrengthIndex);
            playerStrengthToggles[playerStrengthIndex].SetIsOnWithoutNotify(true);
        }
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    [Header("Dungeon Properties")]
    public int SmallDungeonSize = 25;
    public int MediumDungeonSize = 50;
    public int LargeDungeonSize = 150;
    [Space]
    public float EasyEnemyProbability = 0.1f;
    public float MediumEnemyProbability = 0.2f;
    public float HardEnemyProbability = 0.4f;
    [Space]
    public int EasyEnemyLevel = 1;
    public int MediumEnemyLevel = 4;
    public int HardEnemyLevel = 8;
    [Space]
    public int EasyPlayerLevel = 1;
    public int MediumPlayerLevel = 4;
    public int HardPlayerLevel = 8;

    // dungeon size
    private void SetDungeonSize(int index)
    {
        switch (index)
        {
            case 0:
                settingsSingleton.dungeonSize = SmallDungeonSize;
                break;
            case 1:
                settingsSingleton.dungeonSize = MediumDungeonSize;
                break;
            case 2:
                settingsSingleton.dungeonSize = LargeDungeonSize;
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
            case 4:
                settingsSingleton.dungeonType = DungeonType.Maze;
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

    public void SetDungeonTheme4()
    {
        int index = 4;
        PlayerPrefs.SetInt("DungeonTheme", index);
        SetDungeonTheme(index);
    }

    // enemy difficulty
    private void SetEnemyDifficulty(int index)
    {
        switch (index)
        {
            case 0:
                settingsSingleton.enemyProbability = EasyEnemyProbability;
                settingsSingleton.enemyLevel = EasyEnemyLevel;
                break;
            case 1:
                settingsSingleton.enemyProbability = MediumEnemyProbability;
                settingsSingleton.enemyLevel = MediumEnemyLevel;
                break;
            case 2:
                settingsSingleton.enemyProbability = HardEnemyProbability;
                settingsSingleton.enemyLevel = HardEnemyLevel;
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
                settingsSingleton.playerLevel = EasyPlayerLevel;
                break;
            case 1:
                settingsSingleton.playerLevel = MediumPlayerLevel;
                break;
            case 2:
                settingsSingleton.playerLevel = HardPlayerLevel;
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

    public bool isMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
             return IsMobile();
#endif
        return false;
    }
}
