using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBody : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] float AutocloseDistance = 4;

    private bool IsDoorClosed = true;

    private void Update()
    {
        if (Vector2.Distance(PlayerMovement.Instance.transform.position, transform.position) > AutocloseDistance)
        {
            if (door.AutoClose && !IsDoorClosed)
            {
                IsDoorClosed = true;
                door.CloseDoor();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"TIRGER");

        if (other.CompareTag("Player"))
        {
            IsDoorClosed = false;
            door.OpenDoor();
        }
    }
}
