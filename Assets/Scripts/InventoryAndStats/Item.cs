using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Combat;
using Alchemy.Stats;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Alchemy.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Create Item (Generic)")]
    public class Item : ScriptableObject
    {
        public AudioClip Sound;
        public string ItemName = "Item";
        [TextArea] public string ItemDescription = "";
        public int AutoBalanceThreshold = 10;
        [Space]
        public bool ConsumeOnAcquire;
        [Space]
        public bool RestoreHPAsPercent = false;
        public float HealthRestore = 0;
        public bool RestoreSPAsPercent = false;
        public float StaminaRestore = 0;
        [Space]
        public Skill SkillToLearn;
        public bool isDungeonSkill;
        [TextArea] public string DungeonSkillDialogue = "Dungeon Skill";
        public DungeonManager.DungeonSkillType dungeonSkillType;

        
        public StatusEffectValue[] Effects;
        public StatusEffect[] Removes;

        private UnityEngine.UI.Button LastSelected;

        public UnityAction action;

        public void Activate()
        {
            if (action != null)
            {
                action!.Invoke();
                action = new UnityAction(() => { });
            }
        }

        public void UseItem(ActorStats Target)
        {
            if (Target)
            {
                FindObjectOfType<AudioSource>().PlayOneShot(Sound, 0.6f);

                Debug.Log($"Use {ItemName}");
                if (SkillToLearn)
                {
                    if (!Target.Skills.Contains(SkillToLearn))
                    {
                        Target.Skills.Add(SkillToLearn);

                        if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>())
                            LastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>();
                        else
                            LastSelected = null;

                        DialogueManager.OnDialogueClose += () =>
                        {
                            if (PauseMenu.MenuOpen)
                                if (LastSelected)
                                    LastSelected.Select();
                                else
                                    FindObjectOfType<PauseMenu>().ContinueButton.Select();
                        };

                        DialogueManager.ShowMessage($"You find a crumpled lolly wrapper at the bottom of the chest, on the inner side is a strange symbol...\n\nYou have learned {SkillToLearn.DisplayedName}!");
                    }
                    else
                    {
                        DialogueManager.ShowMessage($"You attempt to scrutinize the scroll for more knowledge...\nAlas, there was nothing left to learn about {SkillToLearn.DisplayedName}...");
                    }
                }
                else if (isDungeonSkill)
                {
                    if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>())
                        LastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>();
                    else
                        LastSelected = null;

                    DialogueManager.OnDialogueClose += () =>
                    {
                        if (PauseMenu.MenuOpen)
                            if (LastSelected)
                                LastSelected.Select();
                            else
                                FindObjectOfType<PauseMenu>().ContinueButton.Select();
                    };

                    DialogueManager.ShowMessage($"{DungeonSkillDialogue}");
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