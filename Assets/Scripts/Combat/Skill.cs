using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;

namespace Alchemy.Combat
{
    public enum Element
    {
        None,
        Fire,
        Ice,
        Wind,
        Earth
    }

    public struct OutputDamage
    {
        public float Damage;
        public bool WasCrit;
    }

    [CreateAssetMenu(fileName = "New Skill", menuName = "Create Skill")]
    public class Skill : ScriptableObject
    {
        public string DisplayedName = "Skill";
        [TextArea] public string Description = "Type your description here!";
        public Sprite Icon;
        [Space]
        public Stat BasedStat = Stat.Arcana;
        [Space]
        [Tooltip("If this is a healing skill, set this to a negative!")]
        public bool CanCrit = true;
        public float CritMultiplier = 1.3f;
        public float m_Damage = 1;
        public Element DamageElement;

        public OutputDamage Damage(StatHolder Stats, int Level = 1)
        {
            float Dmg = m_Damage;
                
            switch (BasedStat)
            {
                case Stat.Agility:
                    Dmg *= Stats.AgilityOverLevel.Evaluate(Level);
                    break;
                case Stat.Arcana:
                    Dmg *= Stats.ArcanaOverLevel.Evaluate(Level);
                    break;
                case Stat.Attack:
                    Dmg *= Stats.AttackOverLevel.Evaluate(Level);
                    break;
                case Stat.Health:
                    Dmg *= Stats.HealthOverLevel.Evaluate(Level);
                    break;
                case Stat.Luck:
                    Dmg *= Stats.LuckOverLevel.Evaluate(Level);
                    break;
                case Stat.Stamina:
                    Dmg *= Stats.StaminaOverLevel.Evaluate(Level);
                    break;
            }

            bool WasCrit = false;

            if (Random.Range(0, 100) <= Stats.LuckOverLevel.Evaluate(Level) && CanCrit)
            {
                Dmg *= CritMultiplier;
                WasCrit = true;
            }

            return new OutputDamage() { Damage = Dmg, WasCrit = WasCrit };
        }
    }
}