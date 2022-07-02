using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // current doors
    [SerializeField] bool doorTop;
    [SerializeField] bool doorRight;
    [SerializeField] bool doorBottom;
    [SerializeField] bool doorLeft;

    // required rooms
    [HideInInspector] public bool top;
    [HideInInspector] public bool right;
    [HideInInspector] public bool bottom;
    [HideInInspector] public bool left;

    [SerializeField] List<Door> childDoors;

    private void Awake()
    {
        // setup required rooms
        top = doorTop;
        right = doorRight;
        bottom = doorBottom;
        left = doorLeft;
    }

    private void Start()
    {
        RoomTemplates templates = FindObjectOfType<RoomTemplates>();
        templates.rooms.Add(this);
    }

    public bool IsComplete()
    {
        if (!top && !right && !bottom && !left)
        {
            return true;
        }
        return false;
    }

    public void RemoveTopDoor()
    {
        doorTop = false;
        top = false;
    }

    public void RemoveRightDoor()
    {
        doorRight = false;
        right = false;
    }

    public void RemoveBottomDoor()
    {
        doorBottom = false;
        bottom = false;
    }

    public void RemoveLeftDoor()
    {
        doorLeft = false;
        left = false;
    }

    public void AddDoor(Door door)
    {
        if (!childDoors.Contains(door))
        {
            childDoors.Add(door);
        }
    }

}
