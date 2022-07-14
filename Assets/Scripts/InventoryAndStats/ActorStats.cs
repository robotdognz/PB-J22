using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alchemy.Stats
{
    public enum DecisionStyle
    {
        PlayerControlled,
        AI
    }

    public class ActorStats : MonoBehaviour
    {
        public StatHolder Stats;

        public string ActorName = "Alchemist";

        public DecisionStyle DecisionMaker;

        public int CurrentLevel; // Players current Level
        public int CurrentHealth { get; private set; } // Players current Health (not normalized)
        public int CurrentStamina { get; private set; } // Players current Stamina (not normalized)

        public List<Combat.Skill> Skills = new List<Combat.Skill>();

        public Sprite NormalSprite;
        public Sprite DamagedSprite;
        public Sprite DeadSprite;

        public List<Combat.InstancedStatusEffect> StatusEffects = new List<Combat.InstancedStatusEffect>();
        public int CurrentEXP = 0;

        public void InitializeActor()
        {

            if (Stats.StartingSkills.Length >= 0)
                foreach (Combat.Skill Skill in Stats.StartingSkills)
                    Skills.Add(Skill);

            if (!string.IsNullOrEmpty(Stats.DefaultName))
            {
                ActorName = Stats.DefaultName;
            }

            if (Stats.NormalSprite)
            {
                NormalSprite = Stats.NormalSprite;
                DamagedSprite = Stats.NormalSprite;
                DeadSprite = Stats.NormalSprite;

                if (Stats.DeadSprite)
                {
                    DeadSprite = Stats.DeadSprite;

                    if (Stats.DamagedSprite)
                    {
                        DamagedSprite = Stats.DamagedSprite;
                    }
                }

                foreach (SpriteRenderer R in GetComponentsInChildren<SpriteRenderer>())
                {
                    R.sprite = NormalSprite;
                }
            }

            if (DecisionMaker == DecisionStyle.PlayerControlled)
            {
                if (SettingsSingleton.instance.WasPlayerKilled)
                    Inventory.Inventory.Items.Clear();
                else
                {
                    SettingsSingleton.instance.WasPlayerKilled = true;
                    Skills = Inventory.Inventory.SkillsTransfer.ToList();
                }

                foreach (Inventory.ItemInstance Item in Stats.Drops)
                {
                    Inventory.Inventory.AddItem(Item.Base, Item.Count);
                }
            }

            ResetStats();
        }

        protected AudioSource Src;

        protected virtual void Awake()
        {
            if (GetComponent<AudioSource>())
                Src = GetComponent<AudioSource>();
            else
                Src = gameObject.AddComponent<AudioSource>();

            if (!GetComponent<Enemy>())
                InitializeActor();
        }

        public void SetHealth(int Value) => CurrentHealth = Value;

        public void UseSkill(int Skill, ActorStats Target)
        {
            UseSkill(Skills[Skill], Target);
        }

        public void UseSkill(Combat.Skill Skill, ActorStats Target)
        {
            Combat.OutputDamage Damage = Skill.Damage(Stats, CurrentLevel, Target);

            Combat.BattleManager.Instance.ClearATB(this);
            ModifyStamina(Skill.StaminaCost);

            if (Target.IsWeakTo(Skill.DamageElement))
            {
                Damage.Damage *= 1.6f;
                Damage.WasCrit = true;
                Damage.WasWeak = true;
            }

            StartCoroutine(SpawnEffect(Skill.IndicatorDelay, Skill.Effect, Target, Damage));
        }

        private IEnumerator SpawnEffect(float Delay, GameObject Effect, ActorStats Target, Combat.OutputDamage Damage)
        {
            if (Effect)
            {
                Instantiate(Effect, Target.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(Delay);
            }

            foreach (Combat.StatusEffect E in Damage.Effects)
            {
                Debug.Log(E.Name);
            }

            if (Damage.Effects.Length > 0)
            {
                foreach (Combat.StatusEffect E in Damage.Effects)
                {
                    Combat.InstancedStatusEffect InstEffect = E.GenerateInstance();

                    bool HadEffect = false;

                    if (Target.StatusEffects.Count > 0)
                    {
                        foreach (Combat.InstancedStatusEffect E2 in Target.StatusEffects)
                        {
                            if (E2.Effect == E)
                            {
                                HadEffect = true;

                                if (E2.TurnsRemaining < InstEffect.TurnsRemaining)
                                {
                                    E2.TurnsRemaining = InstEffect.TurnsRemaining;
                                }

                                break;
                            }
                        }
                    }

                    if (!HadEffect)
                    {
                        Debug.Log($"Added {E.Name}");
                        Target.StatusEffects.Add(InstEffect);
                    }
                }
            }

            Target.ModifyHealth(Mathf.RoundToInt(Damage.Damage));

            if (Damage.Damage != 0)
            {
                Combat.BattleManager.ShowDamagePopup(Target.transform, Damage.Damage, Damage.WasCrit, Damage.WasWeak);
            }
        }

        public void ProcessTurn()
        {
            switch (DecisionMaker)
            {
                case DecisionStyle.AI:
                    {
                        Combat.Skill Skill = Skills[Random.Range(0, Skills.Count)];

                        if (Skill.m_Damage >= 0)
                        {
                            UseSkill(Skill, this);
                        }
                        else
                        {
                            UseSkill(Skill, Combat.UIManager.Instance.PlayerStats);
                        }
                    }
                    break;
            }            
        }

        /// <summary>
        /// Resets player stats (but not level)
        /// </summary>
        public void ResetStats()
        {
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
        }

        /// <summary>
        /// Returns true if the final health value is less than 0
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public bool ModifyHealth(int Amount)
        {
            CurrentHealth += Amount;

            try
            {
                if (CurrentHealth <= 0)
                {
                    GetComponent<SpriteRenderer>().sprite = DeadSprite;

                    if (Stats.DeathSound)
                        Src.PlayOneShot(Stats.DeathSound);

                    return true;
                }
                else
                {
                    if (Amount < 0)
                        if (Stats.HurtSound)
                            Src.PlayOneShot(Stats.HurtSound);

                    if (HealthPercent <= 0.2f)
                    {
                        GetComponent<SpriteRenderer>().sprite = DamagedSprite;
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().sprite = NormalSprite;
                    }
                }
            } catch { }

            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

            if (Combat.UIManager.Instance)
                Combat.UIManager.Instance.OnDamagePlayer();

            return false;
        }

        public void ModifyStamina(int Amount)
        {
            CurrentStamina -= Amount;

            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);

            if (Combat.UIManager.Instance)
                Combat.UIManager.Instance.OnDamagePlayer();
        }

        public int MaxHealth => Mathf.RoundToInt(Stats.HealthOverLevel.Evaluate(CurrentLevel));

        public int MaxStamina => Mathf.RoundToInt(Stats.StaminaOverLevel.Evaluate(CurrentLevel));

        /// <summary>
        /// The current percentage of the players health, shown as a value ranging from 0 to 1
        /// </summary>
        public float HealthPercent => (float)CurrentHealth / MaxHealth;

        /// <summary>
        /// The current percentage of the players stamina, shown as a value ranging from 0 to 1
        /// </summary>
        public float StaminaPercent => (float)CurrentStamina / MaxStamina;


        public int Agility => Mathf.RoundToInt(Stats.AgilityOverLevel.Evaluate(CurrentLevel));

        public int Luck => Mathf.RoundToInt(Stats.LuckOverLevel.Evaluate(CurrentLevel));

        public int Attack => Mathf.RoundToInt(Stats.AttackOverLevel.Evaluate(CurrentLevel));

        public bool IsWeakTo(Combat.Element Element) => Element != Combat.Element.None && Stats.IsWeakTo.Contains(Element);
    }
}