using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenLogic : MonoBehaviour
{
    private void Start()
    {
        // tell the title screen to display the win message
        SettingsSingleton.instance.titleScreenState = SettingsSingleton.TitleScreenMessage.Win;
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
