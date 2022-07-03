using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;

namespace Alchemy.Combat
{
    [CreateAssetMenu(fileName = "New Enemy Palette", menuName = "Create Enemy Palette")]
    public class EnemyPalette : ScriptableObject
    {
        public StatHolder[] Enemies;
        public StatHolder GetEnemy
        {
            get
            {
                return Enemies[Random.Range(0, Enemies.Length)];
            }
        }
    }
}