using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class MinimapSystem : MonoBehaviour
{
    public RawImage Minimap;
    private Camera Cam;

    private void Awake()
    {
        Cam = GetComponent<Camera>();
    }

    private void Update()
    {
        transform.position = Camera.main.transform.position;
        Minimap.texture = Cam.targetTexture;
    }
}
