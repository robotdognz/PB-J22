using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy;

public class Enemy : MonoBehaviour
{
    SpriteRenderer sRenderer;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    public void DisableEnemy()
    {
        sRenderer.enabled = false;
    }

    public void EnableEnemy()
    {
        sRenderer.enabled = true;
    }
}
