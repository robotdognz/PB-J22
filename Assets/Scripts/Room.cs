using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] bool doorTop;
    [SerializeField] bool doorRight;
    [SerializeField] bool doorBottom;
    [SerializeField] bool doorLeft;

    // door sides, aka required rooms
    public bool top;
    public bool right;
    public bool bottom;
    public bool left;

    SpriteRenderer sRenderer;

    private void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        RoomTemplates templates = FindObjectOfType<RoomTemplates>();
        templates.rooms.Add(this);
    }


    private void Update()
    {
        if (IsComplete())
        {
            sRenderer.color = new Color(255, 0, 0);
        }
    }

    public bool IsComplete()
    {
        if (!top && !right && !bottom && !left)
        {
            return true;
        }
        return false;
    }


}
