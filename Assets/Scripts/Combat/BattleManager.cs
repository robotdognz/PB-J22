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
            Instance = this;
            ThisCombat = Battlers;
            Indicator = GameObject.Find("Important_DamagePopup").GetComponent<UnityEngine.UI.Text>();
        }

        public static void ShowDamagePopup(Transform Target, float Damage, bool WasCrit = false)
        {
            GameObject.Find("Important_DamagePopup").transform.position = Target.transform.position;
            GameObject.Find("Important_DamagePopup").GetComponent<UnityEngine.UI.Text>().text = $"{(WasCrit ? "CRIT " : "")}{Mathf.RoundToInt(Damage)}";
            GameObject.Find("Important_DamagePopup").GetComponent<UnityEngine.UI.Text>().color = Damage > 0 ? Color.green : WasCrit ? Color.yellow : Color.red;
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
            Indicator.transform.position += Vector3.up * Time.deltaTime / 2;
            Indicator.color = new Color(Indicator.color.r, Indicator.color.g, Indicator.color.b, Indicator.color.a - Time.deltaTime);

            if (ThisCombat.Length > 0)
            {
                bool AnyATB1 = false;

                foreach (Battler B in ThisCombat)
                {
                    B.CurrentATB += ((Time.deltaTime / 50) * B.Stats.Agility);
                    if (B.Stats == UIManager.Instance.PlayerStats)
                    {
                        UIManager.Instance.SetATBBarValue(B.CurrentATB);
                    }

                    if (B.CurrentATB >= 1)
                    {
                        AnyATB1 = true;
                    }
                }

                if (AnyATB1)
                    BattleSpeed = 0;
                else
                    BattleSpeed = 1;
            }
        }
    }
}