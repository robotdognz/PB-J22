using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenLogic : MonoBehaviour
{
    private void Start()
    {
        // tell the title screen to display the win message
        SettingsSingleton singleton = FindObjectOfType<SettingsSingleton>();
        singleton.titleScreenState = SettingsSingleton.TitleScreenMessage.Win;
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
