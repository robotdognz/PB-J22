using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Combat starts!");

            // temp kill the enemy immediately, replace this with Sky's combat system
            Kill();
        }

    }

    public void Kill()
    {
        GetComponentInParent<EnemyLayout>().KillEnemy(this);
    }
}
