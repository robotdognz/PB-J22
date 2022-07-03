using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Notify();  // delegate

public class EnemyLayout : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies;

    public event Notify EnemiesDefeated; // event

    public void KillEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Destroy(enemy.gameObject);

        }

        if (enemies.Count == 0)
        {
            // tell room to deactivate
            EnemiesDefeated?.Invoke();
        }
    }

    public void KillAll()
    {
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}
