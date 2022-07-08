using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileControlRig : MonoBehaviour
{
    public string Horizontal = "Horizontal";
    public string Vertical = "Vertical";
    public string Pause = "Cancel";
    public string Interact = "Interact";
    public string Back = "Back";
    [Space]
    public GameObject HorizontalScreen;
    public GameObject VerticalScreen;
    public Camera MainRenderer;
    [Space]
    public GameObject[] MobileControls;

    private void Update()
    {
        Resolution R = new Resolution();
        R.width = MainRenderer.pixelWidth;
        R.height = MainRenderer.pixelHeight;

        int Ratio = Mathf.RoundToInt(((float)R.width / R.height) * 10);
        bool Vertical = R.height > R.width;

        Debug.Log($"Resolution: {R.width}x{R.height}\nRatio: {Ratio}\nIsVertical: {Vertical}");

        VerticalScreen.SetActive(Vertical);
        HorizontalScreen.SetActive(!Vertical);

        foreach (GameObject G in MobileControls)
        {
            G.SetActive(PlayerPrefs.GetInt("Mobile", 0) == 1);
        }
    }

    public void SetHorizontal(float Value)
    {
        InputManager.SetAxis(Horizontal, Value);
    }

    public void SetVertical(float Value)
    {
        InputManager.SetAxis(Vertical, Value);
    }

    public void SetPause(bool Value)
    {
        InputManager.SetButton(Pause, Value);
    }

    public void SetInteract(bool Value)
    {
        InputManager.SetButton(Interact, Value);
    }

    public void SetBack(bool Value)
    {
        InputManager.SetButton(Back, Value);
    }
}
