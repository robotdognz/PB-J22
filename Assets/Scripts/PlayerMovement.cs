using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D body;
    Vector2 movement;

    [SerializeField] float moveSpeed = 5;

    // [SerializeField] 


    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // do input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // do movement
        body.MovePosition(body.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Room"))
        {
            Debug.Log("Move camera");
            Vector3 temp = Camera.main.transform.position;
            temp.x = other.transform.position.x;
            temp.y = other.transform.position.y;
            Camera.main.transform.position = temp;
        }
    }
}
