using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inventory;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Menu;
    public Transform ItemSpawn;
    public GameObject ItemButton;
    private List<GameObject> Buttons = new List<GameObject>();

    public void RefreshItems()
    {
        if (Buttons.Count > 0)
            foreach (GameObject Button in Buttons)
                Destroy(Button);

        Buttons.Clear();

        foreach (ItemInstance I in Inventory.Items)
        {
            GameObject Btn = Instantiate(ItemButton, ItemSpawn);
            Btn.GetComponentInChildren<UnityEngine.UI.Text>().text = $"{I.Base.ItemName} x{I.Count}";
            Buttons.Add(Btn);
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        Time.timeScale = Menu.activeSelf ? 0 : 1;

        if (Input.GetButtonDown("Cancel"))
        {
            Menu.SetActive(true);
        }
    }
}
