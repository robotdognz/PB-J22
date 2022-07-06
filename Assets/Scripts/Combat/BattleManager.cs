using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Alchemy.Stats;
using Alchemy.Music;

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
        private bool Initialized = false;

        public Battler[] Battlers
        {
            get
            {
                 // This is the final version of the code. I've commented it out so I can test easily
                List<Battler> Bs = new List<Battler>();
                int I = 0;
                foreach (ActorStats Actor in BattleStarter.Battlers)
                {
                    if (I == 0)
                        UIManager.Instance.PlayerStats = Actor;
                    Bs.Add(new Battler() { Stats = Actor });
                    I++;
                }
                return Bs.ToArray();
            }
        }

        public Battler[] ThisCombat { get; private set; }
        public static ActorStats CurrentTurn;

        private void Awake()
        {
            Instance = this;
            Initialized = false;
            BattleEnded = false;
        }

        public void Init()
        {
            ThisCombat = Battlers;
            UIManager.Instance.Init();
            Initialized = true;
        }

        public GameObject DamageIndicatorBase;

        public static void ShowDamagePopup(Transform Target, float Damage, bool WasCrit = false, bool WasWeak = false)
        {
            GameObject DmgInd = Instantiate(Instance.DamageIndicatorBase, GameObject.Find("WorldCanvas").transform);
            UnityEngine.UI.Text Txt = DmgInd.GetComponent<UnityEngine.UI.Text>();
            Txt.text = $"{(WasCrit ? WasWeak ? "WEAK" : "CRIT " : "")}{(int)Damage}";
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

        private bool AllEnemiesDead
        {
            get
            {
                foreach (Battler B in ThisCombat)
                {
                    if (B.Stats != UIManager.Instance.PlayerStats)
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
            if (Initialized)
            {
                if (ThisCombat.Length > 0 && !BattleEnded)
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

                    if (AllEnemiesDead)
                    {
                        Initialized = false;
                        UIManager.Instance.PlayerStats.StatusEffects.Clear();

                        MusicManager.SetTrack(Track.Victory);
                        UIManager.Instance.AftermathScreen.SetActive(true);
                        UIManager.Instance.SetMenu(6);
                        UIManager.Instance.DoAftermath();
                        BattleEnded = true;
                    }
                    if (PlayerDead)
                    {
                        Initialized = false;
                        UIManager.Instance.PlayerStats.StatusEffects.Clear();

                        UIManager.Instance.Darkinator.sprite = null;
                        EndBattle(BattleEndResult.Defeat);
                    }
                }
            }
        }

        private static bool BattleEnded = false;

        public void FinishVictory()
        {
            EndBattle(BattleEndResult.Victory);
        }

        public static void EndBattle(BattleEndResult Result)
        {
            BattleEnded = true;
            UIManager.Instance.StartCoroutine(EndBattleFadeout(Result));
        }

        private static IEnumerator EndBattleFadeout(BattleEndResult Result)
        {
            if (Result == BattleEndResult.Defeat)
            {
                MusicManager.SetTrack(Track.GameOver);

                UIManager.Instance.Darkinator.gameObject.SetActive(true);
                while (UIManager.Instance.Darkinator.color.a < 1)
                {
                    UIManager.Instance.Darkinator.color = new Color(UIManager.Instance.Darkinator.color.r, UIManager.Instance.Darkinator.color.g, UIManager.Instance.Darkinator.color.b, UIManager.Instance.Darkinator.color.a + (Time.deltaTime / 2));
                    yield return null;
                }
            }

            SceneManager.UnloadSceneAsync("Combat");
            BattleStarter.OnBattleEnd.Invoke(Result);
        }
    }
}