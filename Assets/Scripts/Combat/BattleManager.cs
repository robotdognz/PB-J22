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
                    Actor.ResetStats();
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

        public GameObject DamageIndicatorBase;

        public static void ShowDamagePopup(Transform Target, float Damage, bool WasCrit = false)
        {
            GameObject DmgInd = Instantiate(Instance.DamageIndicatorBase, GameObject.Find("WorldCanvas").transform);
            UnityEngine.UI.Text Txt = DmgInd.GetComponent<UnityEngine.UI.Text>();
            Txt.text = $"{(WasCrit ? "<i>CRIT</i> " : "")}{(int)Damage}";
            Txt.color = Damage >= 0 ? Color.green : WasCrit ? Color.red : Color.yellow;
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

        private bool AllEnemiesDead
        {
            get
            {
                foreach (Battler B in ThisCombat)
                {
                    if (B.Stats.CurrentHealth > 0)
                        return false;
                }

                return true;
            }
        }

        private bool PlayerDead
        {
            get
            {
                return UIManager.Instance.PlayerStats.CurrentHealth <= 0;
            }
        }

        private void Update()
        {
            if (ThisCombat.Length > 0)
            {
                bool AnyATB1 = false;

                foreach (Battler B in ThisCombat)
                {
                    B.CurrentATB += (((Time.deltaTime / 50) * B.Stats.Agility) * (Random.Range(B.Stats.Luck / 8, 90) / 25)) * BattleSpeed;

                    if (B.Stats == UIManager.Instance.PlayerStats)
                    {
                        UIManager.Instance.SetATBBarValue(B.CurrentATB);
                    }

                    if (B.CurrentATB >= 1 && B.Stats.CurrentHealth > 0)
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