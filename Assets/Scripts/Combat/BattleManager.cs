using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;

namespace Alchemy.Combat
{
    public class Battler
    {
        public ActorStats Stats;
        public float CurrentATB;
    }

    public class BattleManager : MonoBehaviour
    {
        public float BattleSpeed = 1;

        public Battler[] Battlers
        {
            get
            {
                List<Battler> Bs = new List<Battler>();
                foreach (ActorStats Actor in FindObjectsOfType<ActorStats>())
                {
                    Bs.Add(new Battler() { Stats = Actor });
                }
                return Bs.ToArray();
            }
        }

        Battler[] ThisCombat;

        private void Awake()
        {
            ThisCombat = Battlers;
        }

        private void Update()
        {
            if (ThisCombat.Length > 0)
            {
                foreach (Battler B in ThisCombat)
                {
                    B.CurrentATB += ((Time.deltaTime / 50) * B.Stats.Agility);
                    if (B.Stats == UIManager.Instance.PlayerStats)
                    {
                        UIManager.Instance.SetATBBarValue(B.CurrentATB);
                    }
                }
            }
        }
    }
}