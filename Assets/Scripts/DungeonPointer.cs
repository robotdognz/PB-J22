using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPointer : MonoBehaviour
{
    float screenDiam = 5f;
    private Vector3 pointingTo;

    private void LateUpdate()
    {
        // TODO: don't hard code this
        pointingTo = new Vector3(DungeonManager.bossPosition.x, DungeonManager.bossPosition.y);

        // move to correct position on edge of screen
        Vector3 diff = pointingTo - Camera.main.transform.position;
        float xDiffAbs = Mathf.Abs(diff.x);
        float yDiffAbs = Mathf.Abs(diff.y);
        float x;
        float y;
        if (xDiffAbs > yDiffAbs)
        {
            x = Mathf.Clamp(pointingTo.x, Camera.main.transform.position.x - screenDiam, Camera.main.transform.position.x + screenDiam);

            float ySlope = diff.y/diff.x;
            float yIntercept = ySlope * screenDiam * Mathf.Sign(diff.x);

            y = Camera.main.transform.position.y + yIntercept;
        }
        else if (xDiffAbs < yDiffAbs)
        {
            y = Mathf.Clamp(pointingTo.y, Camera.main.transform.position.y - screenDiam, Camera.main.transform.position.y + screenDiam);

            float xSlope = diff.x/diff.y;
            float xIntercept = xSlope * screenDiam * Mathf.Sign(diff.y);

            x = Camera.main.transform.position.x + xIntercept;
        }
        else
        {
            x = Mathf.Clamp(pointingTo.x, Camera.main.transform.position.x - screenDiam, Camera.main.transform.position.x + screenDiam);
            y = Mathf.Clamp(pointingTo.y, Camera.main.transform.position.y - screenDiam, Camera.main.transform.position.y + screenDiam);
        }
        Vector2 newPos = new Vector2(x, y);
        transform.position = newPos;

        // rotate to point at location
        Vector3 difference = pointingTo - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
    }

}
