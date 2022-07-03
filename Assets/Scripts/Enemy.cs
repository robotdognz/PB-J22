using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy;

public class Enemy : MonoBehaviour
{
    bool isDisabled = false;
    SpriteRenderer sRenderer;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    public void DisableEnemy()
    {
        isDisabled = true;
        sRenderer.enabled = false;
    }

    public void EnableEnemy()
    {
        isDisabled = false;
        sRenderer.enabled = true;
    }
}
