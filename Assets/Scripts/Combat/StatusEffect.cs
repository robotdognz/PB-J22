using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Combat
{
    [System.Serializable]
    public class InstancedStatusEffect
    {
        public StatusEffect Effect;
        public int TurnsRemaining;

        public InstancedStatusEffect(StatusEffect Effect, int Turns)
        {
            this.Effect = Effect;
            TurnsRemaining = Turns;
        }
    }

    [System.Serializable]
    public class StatusEffectValue
    {
        public StatusEffect Effect;
        [Range(0, 100)] public int Chance;
        [Tooltip("This effect is guaranteed")]
        public bool Certain;
    }

    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Create Status Effect")]
    public class StatusEffect : ScriptableObject
    {
        public string Name;
        public Sprite Overlay;
        [Space]
        public bool StopsMovement;
        [Space]
        public int HealthDrainPerTurn;
        public int StaminaDrainPerTurn;
        [Space]
        public int MinimumDuration = 1;
        public int MaximumDuration = 4;

        public InstancedStatusEffect GenerateInstance()
        {
            return new InstancedStatusEffect(this, Random.Range(MinimumDuration, MaximumDuration));
        }
    }
}