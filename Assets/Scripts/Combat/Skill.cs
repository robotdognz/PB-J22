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
        // Wind,
        Earth
    }

    public struct OutputDamage
    {
        public float Damage;
        public bool WasCrit;
        public bool WasWeak;
        public StatusEffect[] Effects;
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "New Skill", menuName = "Create Skill")]
    public class Skill : ScriptableObject
    {
        public string DisplayedName = "Skill";
        [TextArea] public string Description = "Type your description here!";
        public Sprite Icon;
        [Space]
        public Stat BasedStat = Stat.Arcana;
        public int StaminaCost = 2;
        [Space]
        public bool CanCrit = true;
        public float CritMultiplier = 1.3f;
        [Range(0, 1)] public float Variance = 0.2f;
        [Tooltip("If this is a healing skill, set this to a negative!")]
        public float m_Damage = 1;
        public Element DamageElement;
        [Space]
        public float IndicatorDelay = 0;
        public GameObject Effect;
        [Space]
        public StatusEffectValue[] StatusEffects;

        public OutputDamage Damage(StatHolder Stats, int Level = 1, ActorStats Target = null)
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

            Dmg += Random.Range(Dmg * Variance, Dmg * -Variance);

            if (Random.Range(0, 100) <= Stats.LuckOverLevel.Evaluate(Level) && CanCrit)
            {
                Dmg *= CritMultiplier;
                WasCrit = true;
            }

            List<StatusEffect> Effects = new List<StatusEffect>();

            if (StatusEffects != null)
            {
                if (Target && StatusEffects.Length > 0)
                {
                    foreach (StatusEffectValue V in StatusEffects)
                    {
                        int Roll = Random.Range(0, 100);

                        Debug.Log($"Rolled {Roll + Target.Luck}, needed {V.Chance}");

                        if (Roll + Target.Luck <= V.Chance || V.Certain)
                        {
                            Effects.Add(V.Effect);
                        }
                    }
                }
            }

            foreach (StatusEffect E in Effects)
            {
                Debug.Log(E.Name);
            }

            return new OutputDamage() { Damage = Dmg, WasCrit = WasCrit, Effects = Effects.ToArray() };
        }
    }
}