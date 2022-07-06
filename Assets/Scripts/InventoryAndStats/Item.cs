using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Combat;
using Alchemy.Stats;

namespace Alchemy.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Create Item (Generic)")]
    public class Item : ScriptableObject
    {
        public AudioClip Sound;
        public string ItemName = "Item";
        [Space]
        public bool RestoreHPAsPercent = false;
        public float HealthRestore = 0;
        public bool RestoreSPAsPercent = false;
        public float StaminaRestore = 0;
        [Space]
        public Skill SkillToLearn;
        public StatusEffectValue[] Effects;
        public StatusEffect[] Removes;

        public void UseItem(ActorStats Target)
        {
            if (Target)
            {
                FindObjectOfType<AudioSource>().PlayOneShot(Sound, 0.6f);

                Debug.Log($"Use {ItemName}");
                if (SkillToLearn)
                {
                    Target.Skills.Add(SkillToLearn);
                    DialogueManager.ShowMessage($"You learnt how to use {SkillToLearn.DisplayedName}!");
                }

                Target.ModifyHealth(Mathf.RoundToInt(RestoreHPAsPercent ? HealthRestore * Target.MaxHealth : HealthRestore));
                Target.ModifyStamina(-Mathf.RoundToInt(RestoreSPAsPercent ? StaminaRestore * Target.MaxStamina : StaminaRestore));

                List<StatusEffect> StatusEffects = new List<StatusEffect>();

                if (Effects != null)
                {
                    if (Target && Effects.Length > 0)
                    {
                        foreach (StatusEffectValue V in Effects)
                        {
                            int Roll = Random.Range(0, 100);

                            Debug.Log($"Rolled {Roll + Target.Luck}, needed {V.Chance}");

                            if (Roll + Target.Luck <= V.Chance || V.Certain)
                            {
                                StatusEffects.Add(V.Effect);
                            }
                        }
                    }
                }

                if (Removes != null)
                {
                    foreach (StatusEffect Effect in Removes)
                    {
                        foreach (InstancedStatusEffect E in Target.StatusEffects)
                        {
                            if (E.Effect == Effect)
                            {
                                Target.StatusEffects.Remove(E);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}