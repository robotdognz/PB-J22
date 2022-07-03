using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLayout : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies;

    public bool isBoss = false;

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}
