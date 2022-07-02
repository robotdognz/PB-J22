using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inventory;

namespace Alchemy.Stats
{
    public enum Stat
    {
        Health,
        Stamina,
        Attack,
        Arcana,
        Agility,
        Luck
    }

    [CreateAssetMenu(fileName = "StatBlock", menuName = "Create StatBlock")]
    public class StatHolder : ScriptableObject
    {
        public AnimationCurve HealthOverLevel = new AnimationCurve(new Keyframe(0, 250), new Keyframe(99, 5500));
        public AnimationCurve StaminaOverLevel = new AnimationCurve(new Keyframe(0, 100), new Keyframe(99, 1000));
        [Space]
        public AnimationCurve AttackOverLevel = new AnimationCurve(new Keyframe(0, 5), new Keyframe(99, 30));
        public AnimationCurve ArcanaOverLevel = new AnimationCurve(new Keyframe(0, 5), new Keyframe(99, 50));
        public AnimationCurve AgilityOverLevel = new AnimationCurve(new Keyframe(0, 10), new Keyframe(99, 125));
        public AnimationCurve LuckOverLevel = new AnimationCurve(new Keyframe(0, 75), new Keyframe(99, 90));
        [Space]
        public AnimationCurve EXPOverLevel = new AnimationCurve(new Keyframe(0, 20), new Keyframe(99, 150));
        [Space]
        public int EXPGained = 5;
        [Space]
        public List<Combat.Element> IsWeakTo;
        [Space]
        public Combat.Skill[] StartingSkills;
        [Space]
        public ItemInstance[] Drops;
    }
}