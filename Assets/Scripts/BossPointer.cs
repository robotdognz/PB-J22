using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPointer : MonoBehaviour
{
    [SerializeField] GameObject pointer;

    private void Update()
    {
        Vector3 difference = new Vector3(DungeonManager.bossPosition.x, DungeonManager.bossPosition.y) - pointer.transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        pointer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);


    }
}
