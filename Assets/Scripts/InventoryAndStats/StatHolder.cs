using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Stats
{
    [CreateAssetMenu(fileName = "StatBlock", menuName = "Create StatBlock")]
    public class StatHolder : ScriptableObject
    {
        public AnimationCurve HealthOverLevel = new AnimationCurve(new Keyframe(0, 250), new Keyframe(99, 5500));
        public AnimationCurve StaminaOverLevel = new AnimationCurve(new Keyframe(0, 100), new Keyframe(99, 1000));
        public AnimationCurve AttackOverLevel = new AnimationCurve(new Keyframe(0, 5), new Keyframe(99, 30));
    }

    public static class PlayerStats
    {
        private static StatHolder Stats = Resources.Load<StatHolder>("StatBlocks/Player");

        public static string PlayerName = "Alchemist";

        public static int CurrentLevel { get; set; } // Players current Level
        public static int CurrentHealth { get; private set; } // Players current Health (not normalized)
        public static int CurrentStamina { get; private set; } // Players current Stamina (not normalized)

        /// <summary>
        /// Resets player stats (but not level)
        /// </summary>
        public static void ResetStats()
        {
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
        }

        /// <summary>
        /// Returns true if the final health value is less than 0
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public static bool ModifyHealth(int Amount)
        {
            CurrentHealth += Amount;

            if (CurrentHealth <= 0)
                return true;

            return false;
        }

        public static int MaxHealth
        {
            get
            {
                return Mathf.RoundToInt(Stats.HealthOverLevel.Evaluate(CurrentLevel));
            }
        }

        public static int MaxStamina
        {
            get
            {
                return Mathf.RoundToInt(Stats.StaminaOverLevel.Evaluate(CurrentLevel));
            }
        }

        /// <summary>
        /// The current percentage of the players health, shown as a value ranging from 0 to 1
        /// </summary>
        public static float HealthPercent { get { return (float)CurrentHealth / MaxHealth; } }

        /// <summary>
        /// The current percentage of the players stamina, shown as a value ranging from 0 to 1
        /// </summary>
        public static float StaminaPercent { get { return (float)CurrentStamina / MaxStamina; } }
    }
}