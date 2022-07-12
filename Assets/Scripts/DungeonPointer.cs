using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPointer : MonoBehaviour
{
    [SerializeField] GameObject pointerObject;
    public float screenDiam = 5f;
    private Vector3 pointingTo;

    public void SetColor(Color color)
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = color;
        }
    }

    public void SetScale(float scale)
    {
        pointerObject.transform.localScale = new Vector2(scale, scale);
    }

    public void SetPointingTo(Vector3 position)
    {
        pointingTo = new Vector3(position.x, position.y);
    }

    private void LateUpdate()
    {
        // move to correct position on edge of screen
        Vector3 diff = pointingTo - Camera.main.transform.position;
        float xDiffAbs = Mathf.Abs(diff.x);
        float yDiffAbs = Mathf.Abs(diff.y);
        float x;
        float y;
        if (xDiffAbs > yDiffAbs)
        {
            x = Mathf.Clamp(pointingTo.x, Camera.main.transform.position.x - screenDiam, Camera.main.transform.position.x + screenDiam);

            float ySlope = diff.y / diff.x;
            float yIntercept = ySlope * screenDiam * Mathf.Sign(diff.x);

            y = Camera.main.transform.position.y + yIntercept;
        }
        else if (xDiffAbs < yDiffAbs)
        {
            y = Mathf.Clamp(pointingTo.y, Camera.main.transform.position.y - screenDiam, Camera.main.transform.position.y + screenDiam);

            float xSlope = diff.x / diff.y;
            float xIntercept = xSlope * screenDiam * Mathf.Sign(diff.y);

            x = Camera.main.transform.position.x + xIntercept;
        }
        else
        {
            x = Mathf.Clamp(pointingTo.x, Camera.main.transform.position.x - screenDiam, Camera.main.transform.position.x + screenDiam);
            y = Mathf.Clamp(pointingTo.y, Camera.main.transform.position.y - screenDiam, Camera.main.transform.position.y + screenDiam);
        }
        Vector2 newPos = new Vector2(x, y);
        pointerObject.transform.position = newPos;

        // rotate to point at location
        Vector3 difference = pointingTo - pointerObject.transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        pointerObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
    }

}
