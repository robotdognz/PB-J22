using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        text.text = "Dungeon size: " + SettingsSingleton.instance.dungeonSize;
    }

}
