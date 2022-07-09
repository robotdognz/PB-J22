using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Alchemy.Stats;
using Alchemy.Combat;

namespace Alchemy
{
    public enum BattleEndResult
    {
        Victory,
        Defeat,
        Fled,
        Other
    }

    public static class BattleStarter
    {
        public static ActorStats[] Battlers = new ActorStats[] { };
        private static string BattleSceneName = "Combat";

        public static UnityAction<BattleEndResult> OnBattleEnd = new UnityAction<BattleEndResult>((BattleEndResult R)=> 
        {
            if (UIManager.Instance)
            {
                PlayerMovement.Instance.GetComponent<ActorStats>().SetHealth(UIManager.Instance.PlayerStats.CurrentHealth);
                PlayerMovement.Instance.GetComponent<ActorStats>().CurrentEXP = UIManager.Instance.PlayerStats.CurrentEXP;
                PlayerMovement.Instance.GetComponent<ActorStats>().CurrentLevel = UIManager.Instance.PlayerStats.CurrentLevel;
            }

            Debug.Log($"Battle ended with result: {R}!");
            Music.MusicManager.SetTrack(Music.Track.Explore);
            ClearBattleEnd();
        });

        public static void ClearBattleEnd()
        {
            OnBattleEnd = new UnityAction<BattleEndResult>((BattleEndResult R) =>
            {
                if (UIManager.Instance)
                {
                    PlayerMovement.Instance.GetComponent<ActorStats>().SetHealth(UIManager.Instance.PlayerStats.CurrentHealth);
                    PlayerMovement.Instance.GetComponent<ActorStats>().CurrentEXP = UIManager.Instance.PlayerStats.CurrentEXP;
                    PlayerMovement.Instance.GetComponent<ActorStats>().CurrentLevel = UIManager.Instance.PlayerStats.CurrentLevel;
                }

                Debug.Log($"Battle ended with result: {R}!");
                Music.MusicManager.SetTrack(Music.Track.Explore);
                ClearBattleEnd();
            });
        }

        public static bool CanPlayerFlee = true;

        /// <summary>
        /// Make sure the player is listed as the first actor in the list. Otherwise you could have some funky issues!-
        /// </summary>
        /// <param name="Actors"></param>
        public static void StartBattle(List<ActorStats> Actors, bool AllowFlee = true, bool IsBoss = false)
        {
            CanPlayerFlee = AllowFlee;
            Debug.Log($"Boss Fight: {IsBoss}");
            if (IsBoss)
            {
                Music.MusicManager.SetTrack(Music.Track.BossFight);
            }
            else
            {
                Music.MusicManager.SetTrack(Music.Track.Battle);
            }
            Actors[0].StartCoroutine(BEGIN(Actors)); // Use the player to start the battle coroutine because silly
        }

        private static IEnumerator BEGIN(List<ActorStats> Actors)
        {
            Debug.LogError("BEEGIN");
            Music.MusicManager.Initialize();
            yield return new WaitForSecondsRealtime(0.1f);
            // Music.MusicManager.SetTrack(Music.Track.Battle); // Switch to Battle music without forgetting the dungeon type

            AsyncOperation Op = SceneManager.LoadSceneAsync("Combat", LoadSceneMode.Additive);

            Debug.LogError("Wait for load");
            while (!Op.isDone)
                yield return null;
            Debug.LogError("Loaded");

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Combat"));

            List<ActorStats> Bs = new List<ActorStats>();

            foreach (ActorStats Actor in Actors)
            {
                Puppet P = new GameObject($"{Actor.name}_PUPPET").AddComponent<Puppet>();

                P.gameObject.AddComponent<SpriteRenderer>();
                P.GetComponent<SpriteRenderer>().sortingOrder = 8;
                P.GetComponent<SpriteRenderer>().flipX = Actor.GetComponent<SpriteRenderer>().flipX;

                P.TrackedCoords = Actor.transform.position;

                P.ActorName = Actor.ActorName;
                P.CurrentLevel = Actor.CurrentLevel;
                P.CurrentEXP = Actor.CurrentEXP;
                P.DamagedSprite = Actor.DamagedSprite;
                P.DeadSprite = Actor.DeadSprite;
                P.DecisionMaker = Actor.DecisionMaker;
                P.NormalSprite = Actor.NormalSprite;
                P.Skills = Actor.Skills;
                P.Stats = Actor.Stats;
                P.StatusEffects = Actor.StatusEffects;
                P.TrackedCoords = Actor.transform.position;

                P.ResetStats();

                if (Actor.DecisionMaker == DecisionStyle.PlayerControlled)
                {
                    P.SetHealth(Actor.CurrentHealth);
                }

                P.transform.localScale = Vector3.one * 0.098439f;
                P.GetComponent<SpriteRenderer>().sprite = P.NormalSprite;

                Bs.Add(P);
            }
            Debug.LogError("Added Battlers");

            Battlers = Bs.ToArray();

            Debug.LogError("Get Active Scene");
            SceneManager.GetActiveScene().GetRootGameObjects()[0].SetActive(true);

            while (BattleManager.Instance == null)
                yield return null;

            Debug.LogError("Done Loading");

            BattleManager.Instance.Init();
        }
    }
}