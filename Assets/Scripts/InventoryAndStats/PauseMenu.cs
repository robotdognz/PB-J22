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
            return Instance.Menu.activeSelf || Instance.GetComponent<DialogueManager>().DialogueScreen.activeSelf || WasUIExitThisFrame;
        }
    }

    public UnityEngine.UI.Button ContinueButton;
    public UnityEngine.UI.Button ItemsButton;
    private static PauseMenu Instance;
    public GameObject Menu;
    public Transform ItemSpawn;
    public GameObject ItemButton;
    private List<GameObject> Buttons = new List<GameObject>();

    private static bool WasUIExitThisFrame;

    [Space]

    public UnityEngine.UI.Text NameAndLevel;
    public UnityEngine.UI.Text HealthValue;
    public UnityEngine.UI.Image HealthBar;
    public UnityEngine.UI.Image EXPBar;

    public void ClosePauseMenu()
    {
        Menu.SetActive(false);
        WasUIExitThisFrame = true;
    }

    private int SelectedItem = 0;

    public void RefreshItems()
    {
        if (Buttons.Count > 0)
            foreach (GameObject Button in Buttons)
                Destroy(Button);

        Buttons.Clear();

        int Index = 0;

        if (Inventory.SortedItems.Count <= 0)
        {
            ItemsButton.Select();
        }

        foreach (ItemInstance I in Inventory.SortedItems)
        {
            GameObject Btn = Instantiate(ItemButton, ItemSpawn);
            Btn.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = $"{I.Base.ItemName} x{I.Count}";
            Btn.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = $"{I.Base.ItemDescription}";
            Btn.name = $"{Index}";
            Btn.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                I.Base.UseItem(PlayerMovement.Instance.GetComponent<Alchemy.Stats.ActorStats>());
                I.Count--;

                if (I.Count <= 0)
                {
                    Inventory.Items.Remove(I);
                }

                SelectedItem = int.Parse(Btn.name);

                RefreshItems();
            });
            Buttons.Add(Btn);

            if (Inventory.SortedItems.Count > 0)
            {
                if (SelectedItem == Mathf.Clamp(Index, 0, Inventory.SortedItems.Count - 1))
                {
                    Btn.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                }
            }

            Index++;
        }
    }

    public void LoadMenu()
    {
        // tell the title screen to display the quit message
        SettingsSingleton.instance.titleScreenState = SettingsSingleton.TitleScreenMessage.Quit;

        // go to title screen
        Time.timeScale = 1;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
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
        if (WasUIExitThisFrame)
            WasUIExitThisFrame = false;

        Time.timeScale = MenuOpen  ? 0 : 1;

        if (InputManager.GetButtonDown("Cancel") && !MenuOpen)
        {
            Menu.SetActive(true);
            ContinueButton.Select();
        }

        #region Player Health and Stats
        Alchemy.Stats.ActorStats Player = PlayerMovement.Instance.GetComponent<Alchemy.Stats.ActorStats>();

        NameAndLevel.text = $"{Player.ActorName}\nLevel {Player.CurrentLevel}";
        HealthBar.fillAmount = (float)Player.CurrentHealth / Player.MaxHealth;
        HealthValue.text = $"Health: {Player.CurrentHealth}/{Player.MaxHealth}";
        EXPBar.fillAmount = Player.CurrentEXP / Player.Stats.EXPOverLevel.Evaluate(Player.CurrentLevel);
        #endregion
    }
}
