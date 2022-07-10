using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLayout : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies;
    [SerializeField] GameObject enemyMarker;

    public bool isBoss = false;

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
