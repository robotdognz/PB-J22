using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inventory;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool MenuOpen
    {
        get
        {
            return Instance.Menu.activeSelf || Instance.GetComponent<DialogueManager>().DialogueScreen.activeSelf;
        }
    }

    private static PauseMenu Instance;
    public GameObject Menu;
    public Transform ItemSpawn;
    public GameObject ItemButton;
    private List<GameObject> Buttons = new List<GameObject>();

    [Space]

    public UnityEngine.UI.Text NameAndLevel;
    public UnityEngine.UI.Text HealthValue;
    public UnityEngine.UI.Image HealthBar;

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
            Btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                I.Base.UseItem(PlayerMovement.Instance.GetComponent<Alchemy.Stats.ActorStats>());
                I.Count--;

                if (I.Count <= 0)
                {
                    Inventory.Items.Remove(I);
                }

                RefreshItems();
            });
            Buttons.Add(Btn);
        }
    }

    public void LoadMenu()
    {
        // tell the title screen to display the quit message
        SettingsSingleton.instance.titleScreenState = SettingsSingleton.TitleScreenMessage.Quit;

        // go to title screen
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ReloadDungeon()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Time.timeScale = MenuOpen  ? 0 : 1;

        if (Input.GetButtonDown("Cancel") && !MenuOpen)
        {
            Menu.SetActive(true);
        }

        #region Player Health and Stats
        Alchemy.Stats.ActorStats Player = PlayerMovement.Instance.GetComponent<Alchemy.Stats.ActorStats>();

        NameAndLevel.text = $"{Player.ActorName}\nLevel {Player.CurrentLevel}";
        HealthBar.fillAmount = (float)Player.CurrentHealth / Player.MaxHealth;
        HealthValue.text = $"Health: {Player.CurrentHealth}/{Player.MaxHealth}";
        #endregion
    }
}
