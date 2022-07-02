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
        [Space]
        public GameObject StatusEffectImage;
        public Transform StatusEffects;
        private List<GameObject> SpawnedIcons = new List<GameObject>();

        [Header("Aftermath")]
        public Image Darkinator;
        public GameObject AftermathScreen;
        [Space]
        public Text LevelProgress;
        public Image LevelUpBar;
        public GameObject LevelUpAnnounce;
        [Space]
        public Transform ItemsGained;
        public GameObject GainedItemObject;

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

            foreach (Battler B in BattleManager.Instance.Battlers)
            {
                ActorStats Actor = B.Stats;
                Button Btn = Instantiate(TargetsButton, TargetsRoot).GetComponentInChildren<Button>();
                Btn.onClick.AddListener(() => 
                {
                    BattleManager.PlayerLoadedTarget = Actor;
                    PlayerStats.UseSkill(BattleManager.PlayerLoadedSkill, Actor);

                    if (PlayerStats.StatusEffects.Count > 0)
                    {
                        try
                        {
                            foreach (InstancedStatusEffect Effect in PlayerStats.StatusEffects)
                            {
                                Effect.TurnsRemaining--;
                                if (Effect.TurnsRemaining <= 0)
                                {
                                    PlayerStats.StatusEffects.Remove(Effect);
                                }
                                else
                                {
                                    PlayerStats.ModifyHealth(-Effect.Effect.HealthDrainPerTurn);
                                    PlayerStats.ModifyStamina(Effect.Effect.StaminaDrainPerTurn);
                                }
                            }
                        } catch { }
                    }

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
                {
                    bool FreezePlayer = false;

                    foreach (InstancedStatusEffect Effect in PlayerStats.StatusEffects)
                    {
                        Debug.Log($"{Effect.Effect.Name}\nStops Movement: {Effect.Effect.StopsMovement}");

                        if (Effect.Effect.StopsMovement)
                        {
                            FreezePlayer = true;
                            break;
                        }
                    }

                    Debug.Log($"Is player sleep: {FreezePlayer}");

                    if (!FreezePlayer)
                    {
                        CurrentMenu = 1;
                    }
                    else
                    {
                        if (PlayerStats.StatusEffects.Count > 0)
                        {
                            foreach (InstancedStatusEffect Effect in PlayerStats.StatusEffects)
                            {
                                Effect.TurnsRemaining--;
                                if (Effect.TurnsRemaining <= 0)
                                {
                                    PlayerStats.StatusEffects.Remove(Effect);
                                }
                                else
                                {
                                    PlayerStats.ModifyHealth(-Effect.Effect.HealthDrainPerTurn);
                                    PlayerStats.ModifyStamina(Effect.Effect.StaminaDrainPerTurn);
                                }
                            }
                        }

                        BattleManager.Instance.ClearATB(PlayerStats);
                    }
                }
            }
            else
            {
                ATBStatus.text = $"Charging... [{Mathf.RoundToInt(Value * 100)}%]";
            }

            UpdateUI();

            ATBBar.fillAmount = Value;
        }

        public void Flee()
        {
            BattleManager.EndBattle(BattleEndResult.Fled);
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
            Instance = this;
        }

        public void Init()
        {
            ResetPlayer(); // This is only to be used until the battle system is done
            
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

            foreach (GameObject G in SpawnedIcons)
            {
                Destroy(G);
            }

            SpawnedIcons.Clear();

            foreach (InstancedStatusEffect StatusEffect in PlayerStats.StatusEffects)
            {
                GameObject Ico = Instantiate(StatusEffectImage, StatusEffects);
                Ico.transform.GetChild(0).GetComponent<Image>().sprite = StatusEffect.Effect.Overlay;
                Ico.GetComponentInChildren<Text>().text = $"{StatusEffect.TurnsRemaining}";
                SpawnedIcons.Add(Ico);
            }
        }
    }
}