using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchemy.Stats;

namespace Alchemy.Combat
{
    [System.Serializable]
    public class Menu
    {
        public GameObject ThisMenu;
        public int ParentMenu;
    }

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public ActorStats PlayerStats;
        public int CurrentMenu { get; private set; }
        [Header("Menus")]
        public Menu[] Menus;

        [Header("Stats")]
        public Text ActorNameLabel;
        public Image ActorPortrait;
        [Space]
        public float ValueLerpSpeed = 0.5f; // It would take two seconds for the value to go from 0 to 1 by default 
        public Text HitpointsLabel;
        public Image HitpointsImage;
        [Space]
        public Text StaminaLabel;
        public Image StaminaImage;
        [Space]
        public Text ATBStatus;
        public Image ATBBar;

        [Header("Skills")]
        public Transform SkillsRoot;
        public GameObject SkillButton;
        private List<GameObject> SkillButtons = new List<GameObject>();
        [Space]
        public Transform TargetsRoot;
        public GameObject TargetsButton;
        private List<GameObject> TargetButtons = new List<GameObject>();

        public void RefreshSkillsList()
        {
            if (SkillButtons.Count > 0)
                foreach (GameObject Btn in SkillButtons)
                    Destroy(Btn);

            SkillButtons.Clear();

            foreach (Skill S in PlayerStats.Skills)
            {
                Button Btn = Instantiate(SkillButton, SkillsRoot).GetComponentInChildren<Button>();
                Btn.onClick.AddListener(() =>
                {
                    BattleManager.PlayerLoadedSkill = S;
                    SetMenu(5);
                    RefreshTargetsList();
                });
                if (PlayerStats.CurrentStamina < S.StaminaCost)
                    Btn.enabled = false;

                Btn.GetComponentInChildren<Text>().text = S.DisplayedName;
                Btn.transform.GetChild(0).GetComponent<Image>().sprite = S.Icon;
                SkillButtons.Add(Btn.gameObject);
            }
        }

        public void RefreshTargetsList()
        {
            if (TargetButtons.Count > 0)
                foreach (GameObject Btn in TargetButtons)
                    Destroy(Btn);

            foreach (ActorStats Actor in FindObjectsOfType<ActorStats>())
            {
                Button Btn = Instantiate(TargetsButton, TargetsRoot).GetComponentInChildren<Button>();
                Btn.onClick.AddListener(() => 
                {
                    BattleManager.PlayerLoadedTarget = Actor;
                    PlayerStats.UseSkill(BattleManager.PlayerLoadedSkill, Actor);
                    SetMenu(0);
                });
                if (Actor.CurrentHealth <= 0)
                    Btn.interactable = false;
                Btn.GetComponentInChildren<Text>().text = Actor == PlayerStats ? "Self" : Actor.ActorName;
                if (Actor == PlayerStats)
                {
                    Btn.transform.SetAsFirstSibling();
                }
                Btn.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
                TargetButtons.Add(Btn.gameObject);
            }
        }

        public void SetATBBarValue(float Value)
        {
            if (Value >= 1)
            {
                ATBStatus.text = "Decision!";
                if (CurrentMenu == 0)
                    CurrentMenu = 1;
            }
            else
            {
                ATBStatus.text = $"Charging... [{Mathf.RoundToInt(Value * 100)}%]";
            }

            UpdateUI();

            ATBBar.fillAmount = Value;
        }

        private IEnumerator UpdateStatLabels()
        {
            while (true) 
            {
                float HealthValue = PlayerStats.HealthPercent;
                float StaminaValue = PlayerStats.StaminaPercent;

                HitpointsImage.fillAmount = Mathf.MoveTowards(HitpointsImage.fillAmount, HealthValue, Time.deltaTime * ValueLerpSpeed);
                StaminaImage.fillAmount = Mathf.MoveTowards(StaminaImage.fillAmount, StaminaValue, Time.deltaTime * ValueLerpSpeed);

                HitpointsLabel.text = $"{Mathf.RoundToInt(HitpointsImage.fillAmount * PlayerStats.MaxHealth)}/{PlayerStats.MaxHealth}";
                StaminaLabel.text = $"{Mathf.RoundToInt(StaminaImage.fillAmount * PlayerStats.MaxStamina)}/{PlayerStats.MaxStamina}";

                if (HitpointsImage.fillAmount == HealthValue && StaminaImage.fillAmount == StaminaValue)
                    break;

                yield return null;
            }
        }

        private void Awake()
        {
            ResetPlayer(); // This is only to be used until the battle system is done
            Instance = this;
            StartCoroutine(UpdateStatLabels());
        }

        public void ModLevel(int Amount)
        {
            PlayerStats.CurrentLevel += Amount;

            UpdateUI();
            StartCoroutine(UpdateStatLabels());
        }

        public void ResetPlayer()
        {
            PlayerStats.ResetStats();

            StartCoroutine(UpdateStatLabels());
        }

        public void PlayerRest()
        {
            PlayerStats.UseSkill(Resources.Load<Skill>("Skills/Rest"), PlayerStats);
        }

        public void OnDamagePlayer()
        {
            StartCoroutine(UpdateStatLabels());
        }

        public void SetMenu(int Menu)
        {
            CurrentMenu = Menu;

            UpdateUI();
        }

        public void OnCancel()
        {
            int OldMenu = CurrentMenu;
            CurrentMenu = Menus[OldMenu].ParentMenu;

            UpdateUI();
        }

        private void UpdateUI()
        {
            ActorNameLabel.text = $"{PlayerStats.ActorName}\nLevel {PlayerStats.CurrentLevel}";

            for (int I = 0; I < Menus.Length; I++)
                if (I == CurrentMenu)
                    Menus[I].ThisMenu.SetActive(true);
                else
                    Menus[I].ThisMenu.SetActive(false);
        }
    }
}