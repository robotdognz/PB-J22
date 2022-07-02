using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Alchemy.Stats;

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

        public static UnityAction<BattleEndResult> OnBattleEnd = new UnityAction<BattleEndResult>((BattleEndResult R)=> { Debug.Log($"Battle ended with result: {R}!"); });

        /// <summary>
        /// Make sure the player is listed as the first actor in the list. Otherwise you could have some funky issues!
        /// </summary>
        /// <param name="Actors"></param>
        public static void StartBattle(List<ActorStats> Actors)
        {
            Battlers = Actors.ToArray();
            SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
        }
    }
}