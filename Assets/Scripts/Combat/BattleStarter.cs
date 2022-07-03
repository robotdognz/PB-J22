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
            Debug.Log($"Battle ended with result: {R}!");
            Music.MusicManager.SetTrack(Music.Track.Explore);
        });

        public static bool CanPlayerFlee = true;

        /// <summary>
        /// Make sure the player is listed as the first actor in the list. Otherwise you could have some funky issues!-
        /// </summary>
        /// <param name="Actors"></param>
        public static void StartBattle(List<ActorStats> Actors, bool AllowFlee = true)
        {
            CanPlayerFlee = AllowFlee;
            Music.MusicManager.SetTrack(Music.Track.Battle); // Switch to Battle music without forgetting the dungeon type
            Actors[0].StartCoroutine(BEGIN(Actors)); // Use the player to start the battle coroutine because silly
        }

        private static IEnumerator BEGIN(List<ActorStats> Actors)
        {
            Music.MusicManager.Initialize();
            yield return new WaitForSecondsRealtime(0.1f);
            Music.MusicManager.SetTrack(Music.Track.Battle); // Switch to Battle music without forgetting the dungeon type

            AsyncOperation Op = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

            while (!Op.isDone)
                yield return null;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Combat"));

            List<ActorStats> Bs = new List<ActorStats>();

            foreach (ActorStats Actor in Actors)
            {
                Puppet P = new GameObject($"{Actor.name}_PUPPET").AddComponent<Puppet>();
                P.gameObject.AddComponent<SpriteRenderer>();

                P.TrackedCoords = Actor.transform.position;

                P.ActorName = Actor.ActorName;
                P.CurrentLevel = Actor.CurrentLevel;
                P.DamagedSprite = Actor.DamagedSprite;
                P.DeadSprite = Actor.DeadSprite;
                P.DecisionMaker = Actor.DecisionMaker;
                P.NormalSprite = Actor.NormalSprite;
                P.Skills = Actor.Skills;
                P.Stats = Actor.Stats;
                P.StatusEffects = Actor.StatusEffects;
                P.TrackedCoords = Actor.transform.position;

                P.ResetStats();

                P.transform.localScale = Vector3.one * 0.25f;
                P.GetComponent<SpriteRenderer>().sprite = P.NormalSprite;

                Bs.Add(P);
            }

            Battlers = Bs.ToArray();

            SceneManager.GetActiveScene().GetRootGameObjects()[0].SetActive(true);

            while (BattleManager.Instance == null)
                yield return null;

            BattleManager.Instance.Init();
        }
    }
}