using System.Collections;
using System.Collections.Generic;
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
        
        public void UseSkill(int Skill, ActorStats Target)
        {
            UseSkill(Skills[Skill], Target);
        }

        public void UseSkill(Combat.Skill Skill, ActorStats Target)
        {
            Combat.OutputDamage Damage = Skill.Damage(Stats, CurrentLevel);

            StartCoroutine(SpawnEffect(Skill.IndicatorDelay, Skill.Effect, Target, Damage));
            Combat.BattleManager.Instance.ClearATB(this);
        }

        private IEnumerator SpawnEffect(float Delay, GameObject Effect, ActorStats Target, Combat.OutputDamage Damage)
        {
            Instantiate(Effect, Target.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(Delay);
            Target.ModifyHealth(Mathf.RoundToInt(Damage.Damage));

            Combat.BattleManager.ShowDamagePopup(Target.transform, Damage.Damage, Damage.WasCrit);
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

            if (CurrentHealth <= 0)
                return true;

            Combat.UIManager.Instance.OnDamagePlayer();

            return false;
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