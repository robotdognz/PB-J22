using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Alchemy.Stats;

public static class BattleSystem
{
    public static ActorStats[] Battlers;
    private static string BattleSceneName = "Combat";

    public static void StartBattle(List<ActorStats> Actors)
    {
        Battlers = Actors.ToArray();
        SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
    }
}