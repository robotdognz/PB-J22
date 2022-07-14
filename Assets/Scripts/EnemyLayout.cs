using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Stats;
using Alchemy.Combat;

public class EnemyLayout : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies;
    [SerializeField] GameObject enemyMarker;

    public bool isBoss = false;

    private void Start()
    {
        if (isBoss)
        {
            // assign random weakness to enemy
            foreach (Enemy enemy in enemies)
            {
                Element weakness = (Element)Random.Range(1, 4);
                ActorStats bossStats = enemy.gameObject.GetComponent<ActorStats>();
                bossStats.Stats.IsWeakTo.Clear();
                bossStats.Stats.IsWeakTo.Add(weakness);
            }
        }
    }

    public void ActivateMarker()
    {
        if (enemyMarker != null)
        {
            enemyMarker.SetActive(true);
        }
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}
