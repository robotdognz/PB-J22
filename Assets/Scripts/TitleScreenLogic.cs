using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenLogic : MonoBehaviour
{
    [SerializeField] int sceneIndex = 1;

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
