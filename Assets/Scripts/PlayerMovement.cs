using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    Vector2 movement;

    [SerializeField] float moveSpeed = 5;

    bool isDisabled = false;
    SpriteRenderer sRenderer;


    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDisabled)
        {
            return;
        }

        // do input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (isDisabled)
        {
            return;
        }

        // do movement
        body.MovePosition(body.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (movement.x < 0)
        {
            sRenderer.flipX = true;
        }
        if (movement.x > 0)
        {
            sRenderer.flipX = false;
        }
    }

    public void DisablePlayer()
    {
        isDisabled = true;
        sRenderer.enabled = false;
    }

    public void EnablePlayer()
    {
        isDisabled = false;
        sRenderer.enabled = true;
    }

}
