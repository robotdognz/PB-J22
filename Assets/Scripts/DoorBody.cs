using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBody : MonoBehaviour
{
    [SerializeField] Door door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            door.OpenDoor();
        }
    }
}
