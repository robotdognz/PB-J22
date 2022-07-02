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
        public static ActorStats[] Battlers;
        private static string BattleSceneName = "Combat";

        public static UnityAction<BattleEndResult> OnBattleEnd = new UnityAction<BattleEndResult>((BattleEndResult R)=> 
        { 
            Debug.Log($"Battle ended with result: {R}!");
            // Music.MusicManager.SetTrack(Music.Track.Explore);
        });

        /// <summary>
        /// Make sure the player is listed as the first actor in the list. Otherwise you could have some funky issues!-
        /// </summary>
        /// <param name="Actors"></param>
        public static void StartBattle(List<ActorStats> Actors)
        {
            // Music.MusicManager.SetTrack(Music.Track.Battle); // Switch to Battle music without forgetting the dungeon type
            SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);

            foreach (ActorStats Actor in Actors)
            {
                Puppet P = Object.Instantiate(Resources.Load<GameObject>("Puppet"), SceneManager.GetSceneByName(BattleSceneName).GetRootGameObjects()[1].transform).GetComponent<Puppet>();
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
            }
        }
    }
}