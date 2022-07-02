using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    public bool isDestroyed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DoorSpawner"))
        {
            // only destroy one of the two door spawners
            if (!other.gameObject.GetComponent<DoorSpawner>().isDestroyed)
            {
                isDestroyed = true;
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
