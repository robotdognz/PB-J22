using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy;
using Alchemy.Combat;
using Alchemy.Stats;

public class EnemyEncounter : MonoBehaviour
{
    public bool Randomize;
    public EnemyPalette Palette;
    [Space]
    public ActorStats[] Enemies;

    private void Awake()
    {
        foreach (ActorStats Actor in Enemies)
        {
            if (Randomize)
            {
                Actor.Stats = Palette.GetEnemy;
            }
            Actor.InitializeActor();
        }
    }

    public void StartEncounter()
    {
        List<ActorStats> Actors = new List<ActorStats>() { PlayerMovement.Instance.GetComponent<ActorStats>() };

        PlayerMovement.Instance.DisablePlayer();

        foreach (ActorStats Enemy in Enemies)
        {
            Enemy.GetComponent<Enemy>().DisableEnemy();
            Actors.Add(Enemy);
        }

        BattleStarter.OnBattleEnd += CombatEnd;
        BattleStarter.StartBattle(Actors, false);
    }

    private void CombatEnd(BattleEndResult Result)
    {
        switch (Result)
        {
            default:
                PlayerMovement.Instance.EnablePlayer();
                break;
            case BattleEndResult.Fled:
                StartEncounter();
                break;
        }
    }
}
