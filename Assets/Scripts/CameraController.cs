using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Vector3 Target = new Vector3(0, 0, -10);
    public static bool FollowX = false;
    public static bool FollowY = false;
    public static Vector2 FollowCamMin;
    public static Vector2 FollowCamMax;

    public Vector3 Position
    {
        get
        {
            return new Vector3(FollowX ? Mathf.Clamp(PlayerMovement.Instance.transform.position.x, FollowCamMin.x, FollowCamMax.x) : Target.x, FollowY ? Mathf.Clamp(PlayerMovement.Instance.transform.position.y, FollowCamMin.y, FollowCamMax.y) : Target.y, -10);
        }
    }
    public bool IgnoreRoomSystem = false;
    public float CameraSpeed;

    private void Awake()
    {
        transform.position = FindObjectOfType<PlayerMovement>().transform.position - Vector3.forward * 10;
    }

    private void Update()
    {
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, IgnoreRoomSystem ? Position : Room.TargetPos, CameraSpeed * Time.deltaTime);
    }
}
