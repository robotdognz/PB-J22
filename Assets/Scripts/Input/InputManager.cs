using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputSettings
{
    public string Name;
    public float Value;
    public bool WasPressedThisFrame;

    public InputSettings(string Name)
    {
        this.Name = Name;
        Value = 0;
        WasPressedThisFrame = true;
    }
}

public static class InputManager
{
    private static Dictionary<string, InputSettings> Inputs = new Dictionary<string, InputSettings>();

    #region Get Functions
    public static bool GetButton(string Key)
    {
        if (!Inputs.ContainsKey(Key))
        {
            Inputs.Add(Key, new InputSettings(Key));
        }

        return Input.GetButton(Key) || Inputs[Key].Value != 0;
    }

    public static bool GetButtonDown(string Key)
    {
        if (!Inputs.ContainsKey(Key))
        {
            Inputs.Add(Key, new InputSettings(Key));
        }

        bool Down = Input.GetButtonDown(Key) || (Inputs[Key].Value != 0 && Inputs[Key].WasPressedThisFrame == false);
        Inputs[Key].WasPressedThisFrame = true;
        return Down;
    }

    public static bool GetButtonUp(string Key)
    {
        if (!Inputs.ContainsKey(Key))
        {
            Inputs.Add(Key, new InputSettings(Key));
        }

        bool Up = Input.GetButtonUp(Key) || (Inputs[Key].Value != 0 && Inputs[Key].WasPressedThisFrame == true);
        Inputs[Key].WasPressedThisFrame = false;
        return Up;
    }

    public static float GetAxis(string Key)
    {
        if (!Inputs.ContainsKey(Key))
        {
            Inputs.Add(Key, new InputSettings(Key));
        }

        return Mathf.Clamp(Input.GetAxisRaw(Key) + Inputs[Key].Value, -1, 1);
    }
    #endregion

    #region Set Functions
    public static void SetButton(string Key, bool Value)
    {
        if (!Inputs.ContainsKey(Key))
        {
            Inputs.Add(Key, new InputSettings(Key));
        }

        Inputs[Key].Value = Value ? 1 : 0;
    }

    public static void SetAxis(string Key, float Value)
    {
        if (!Inputs.ContainsKey(Key))
        {
            Inputs.Add(Key, new InputSettings(Key));
        }

        Inputs[Key].Value = Value;
    }
    #endregion
}
