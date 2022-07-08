using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Toggle))]
public class PlayerPrefsToggle : MonoBehaviour
{
    public string Key;
    public int DefaultValue = 0;

    private void Awake()
    {
        GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(Key, GetComponent<Toggle>().isOn ? 1 : 0) == 1;
        GetComponent<Toggle>().onValueChanged.AddListener((bool Value) =>
        {
            PlayerPrefs.SetInt(Key, Value ? 1 : 0);
        });
    }
}
