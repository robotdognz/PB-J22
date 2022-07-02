using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float CameraSpeed;

    private void Awake()
    {
        transform.position = FindObjectOfType<PlayerMovement>().transform.position - Vector3.forward * 10;
    }

    private void Update()
    {
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Room.TargetPos, CameraSpeed * Time.deltaTime);
    }
}
