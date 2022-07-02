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
        public static BattleManager Instance;
        public static Skill PlayerLoadedSkill;
        public static ActorStats PlayerLoadedTarget;
        public float BattleSpeed = 1;

        public Battler[] Battlers
        {
            get
            {
                /*  // This is the final version of the code. I've commented it out so I can test easily
                List<Battler> Bs = new List<Battler>();
                foreach (ActorStats Actor in BattleSystem.Battlers)
                {
                    Bs.Add(new Battler() { Stats = Actor });
                }
                return Bs.ToArray();
                */

                List<Battler> Bs = new List<Battler>();
                foreach (ActorStats Actor in FindObjectsOfType<ActorStats>())
                {
                    Bs.Add(new Battler() { Stats = Actor });
                }

                return Bs.ToArray();
            }
        }

        Battler[] ThisCombat;
        public static ActorStats CurrentTurn;

        private void Awake()
        {
            Instance = this;
            ThisCombat = Battlers;
        }

        public static void ShowDamagePopup(Transform Target, float Damage, bool WasCrit = false)
        {
            GameObject DmgInd = new GameObject("Indicator");
            UnityEngine.UI.Text Txt = DmgInd.AddComponent<UnityEngine.UI.Text>();
            Txt.text = $"{(WasCrit ? "<i>CRIT</i> " : "")}{Damage}";
            Txt.color = WasCrit ? Color.red : Color.yellow;
            DmgInd.transform.position = Target.position;
            DmgInd.AddComponent<DamageIndicator>();
        }

        public void ClearATB(ActorStats Actor)
        {
            foreach (Battler B in ThisCombat)
            {
                if (B.Stats == Actor)
                    B.CurrentATB = 0;
            }
        }

        UnityEngine.UI.Text Indicator;

        private void Update()
        {
            if (ThisCombat.Length > 0)
            {
                bool AnyATB1 = false;

                foreach (Battler B in ThisCombat)
                {
                    B.CurrentATB += ((Time.deltaTime / 50) * B.Stats.Agility) * BattleSpeed;

                    if (B.Stats == UIManager.Instance.PlayerStats)
                    {
                        UIManager.Instance.SetATBBarValue(B.CurrentATB);
                    }

                    if (B.CurrentATB >= 1)
                    {
                        CurrentTurn = B.Stats;
                        AnyATB1 = true;
                    }
                }

                if (CurrentTurn)
                {
                    CurrentTurn.ProcessTurn();
                    CurrentTurn = null;
                }

                if (AnyATB1)
                    BattleSpeed = 0;
                else
                    BattleSpeed = 1;
            }
        }
    }
}