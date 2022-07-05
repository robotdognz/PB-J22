using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchemy.Stats;
using Alchemy.Inventory;

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
        public Button FleeButton;
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

        [Header("Targets")]
        public Transform TargetsRoot;
        public GameObject TargetsButton;
        private List<GameObject> TargetButtons = new List<GameObject>();

        [Header("Status Effects")]
        public GameObject StatusEffectImage;
        public Transform StatusEffects;
        private List<GameObject> SpawnedIcons = new List<GameObject>();

        [Header("Items")]
        public Transform ItemsRoot;
        public GameObject ItemButton;
        private List<GameObject> ItemButtons = new List<GameObject>();

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

        public void DoAftermath()
        {
            StartCoroutine(Aftermath());
        }

        private IEnumerator Aftermath()
        {
            /*
            int TotalEXP = 0;
            foreach (Battler B in BattleManager.Instance.Battlers)
            {
                if (B.Stats != PlayerStats)
                {
                    TotalEXP += B.Stats.Stats.EXPGained;
                }
            }
            int StartEXP = PlayerStats.CurrentEXP;
            float EXP = StartEXP;
            float Overflow = 0;

            if (EXP + TotalEXP > PlayerStats.Stats.EXPOverLevel.Evaluate(PlayerStats.CurrentLevel))
            {
                Overflow = EXP + TotalEXP - PlayerStats.Stats.EXPOverLevel.Evaluate(PlayerStats.CurrentLevel);
            }

            while (EXP < StartEXP + TotalEXP)
            {
                EXP = Mathf.MoveTowards(EXP, StartEXP + TotalEXP, Time.deltaTime * 10);
                if (EXP > PlayerStats.Stats.EXPOverLevel.Evaluate(PlayerStats.CurrentLevel))
                {
                    PlayerStats.CurrentLevel++;
                    PlayerStats.CurrentEXP = 0;
                    StartEXP = 0;
                    TotalEXP = Mathf.RoundToInt(Overflow);
                    LevelUpAnnounce.SetActive(true);
                }
                PlayerStats.CurrentEXP = Mathf.RoundToInt(EXP);
                LevelProgress.text = $"Level Progress: {Mathf.RoundToInt(EXP)}/{Mathf.RoundToInt(PlayerStats.Stats.EXPOverLevel.Evaluate(PlayerStats.CurrentLevel))}EXP";
                LevelUpBar.fillAmount = EXP / PlayerStats.Stats.EXPOverLevel.Evaluate(PlayerStats.CurrentLevel);
                yield return null;
            }

            while (CurrentMenu == 6)
                yield return null;
            */

            yield return null;

            SetMenu(7);

            List<ItemInstance> Items = new List<ItemInstance>();
            foreach (Battler B in BattleManager.Instance.ThisCombat)
            {
                if (B.Stats != PlayerStats)
                {
                    foreach (ItemInstance Item in B.Stats.Stats.Drops)
                    {
                        float GetProbability = Random.Range(0f, 1);

                        Debug.Log($"Rolled {GetProbability} - Items drop probablility is {Item.DropProbability}");
                        
                        if (GetProbability <= Item.DropProbability) // Allows us to setup proper loot tables
                        {
                            bool GotItem = false;

                            foreach (ItemInstance Itm in Items)
                            {
                                if (Itm.Base == Item.Base)
                                {
                                    Itm.Count += Item.Count;
                                    GotItem = true;
                                    break;
                                }
                            }

                            if (!GotItem)
                                Items.Add(Item.CreateInstance());
                        }
                    }
                }
            }

            foreach (ItemInstance Item in Items)
            {
                if (Item != null)
                {
                    GameObject Obj = Instantiate(GainedItemObject, ItemsGained);
                    Obj.GetComponent<Text>().text = $"{Item.Count}x {Item.Base.ItemName}";

                    Inventory.Inventory.AddItem(Item.Base, Item.Count);
                }
            }
        }

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

        public void RefreshItemsList()
        {
            if (ItemButtons.Count > 0)
                foreach (GameObject Btn in ItemButtons)
                    Destroy(Btn);

            ItemButtons.Clear();

            foreach (ItemInstance Item in Inventory.Inventory.Items)
            {
                Button Btn = Instantiate(ItemButton, ItemsRoot).GetComponentInChildren<Button>();
                Btn.onClick.AddListener(() => 
                {
                    Item.Base.UseItem(PlayerStats);
                    StartCoroutine(UpdateStatLabels());
                    Item.Count--;

                    if (Item.Count <= 0)
                    {
                        Inventory.Inventory.Items.Remove(Item);
                    }

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
                        }
                        catch { }
                    }
                    BattleManager.Instance.ClearATB(PlayerStats);
                    SetMenu(0);
                });

                Btn.GetComponentInChildren<Text>().text = $"{Item.Base.ItemName} x{Item.Count}";
                Btn.transform.GetChild(0).GetComponent<Image>().color = Color.clear;

                ItemButtons.Add(Btn.gameObject);
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
                            List<InstancedStatusEffect> ToRemove = new List<InstancedStatusEffect>();

                            foreach (InstancedStatusEffect Effect in PlayerStats.StatusEffects)
                            {
                                Effect.TurnsRemaining--;
                                if (Effect.TurnsRemaining <= 0)
                                {
                                    ToRemove.Add(Effect);
                                }
                                else
                                {
                                    PlayerStats.ModifyHealth(-Effect.Effect.HealthDrainPerTurn);
                                    PlayerStats.ModifyStamina(Effect.Effect.StaminaDrainPerTurn);
                                }
                            }

                            foreach (InstancedStatusEffect E in ToRemove)
                            {
                                PlayerStats.StatusEffects.Remove(E);
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
            ResetPlayer();
            
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

            FleeButton.interactable = BattleStarter.CanPlayerFlee;
        }
    }
}