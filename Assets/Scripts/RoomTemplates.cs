using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] topRooms;
    public GameObject[] rightRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;

    public int roomCount = 10;

    // rooms should have references to their neighbors, graph structure. This will have lots of uses I can think of when I'm not absolutely wrecked.
}
