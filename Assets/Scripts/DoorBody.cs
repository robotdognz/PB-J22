using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBody : MonoBehaviour
{
    [SerializeField] Door door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"TIRGER");

        if (other.CompareTag("Player"))
        {
            door.OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (door.AutoClose)
                door.CloseDoor();
        }
    }
}
