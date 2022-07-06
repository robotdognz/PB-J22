using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static Room PreviousRoom;
    public static PlayerMovement Instance;

    public Rigidbody2D body;
    Vector2 movement;

    [SerializeField] float moveSpeed = 5;
    public float MoveSpeed { get { return moveSpeed; } }

    public bool isDisabled { get; private set; }
    SpriteRenderer sRenderer;

    /// <summary>
    /// 0 - Up
    /// 1 - Left
    /// 2 - Down
    /// 3 - Right
    /// </summary>
    private int Direction = 0;

    [SerializeField] private float WalkAnimationLength = 0.75f;
    [Space]
    [SerializeField] private Sprite[] WalkUp;
    [SerializeField] private Sprite[] WalkLeft;
    [SerializeField] private Sprite[] WalkDown;
    [SerializeField] private Sprite[] WalkRight;

    private bool MidAnimation = false;

    private void SetCharacterSprite(int FrameIndex)
    {
        Sprite[] Sprites = WalkUp;

        switch (Direction)
        {
            case 0: // Up
                Sprites = WalkUp;
                break;
            case 1: // Left
                Sprites = WalkLeft;
                break;
            case 2: // Down
                Sprites = WalkDown;
                break;
            case 3: // Right
                Sprites = WalkRight;
                break;
        }
        
        sRenderer.sprite = Sprites[FrameIndex];
    }

    private IEnumerator WalkAnimation()
    {
        MidAnimation = true;

        SetCharacterSprite(0);
        yield return new WaitForSeconds(WalkAnimationLength / 4);
        SetCharacterSprite(1);
        yield return new WaitForSeconds(WalkAnimationLength / 4);
        SetCharacterSprite(2);
        yield return new WaitForSeconds(WalkAnimationLength / 4);
        SetCharacterSprite(3);
        yield return new WaitForSeconds(WalkAnimationLength / 4);

        MidAnimation = false;
    }

    void Awake()
    {
        Instance = this;
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

        if (movement.x > 0)
        {
            Direction = 3;
        }
        if (movement.x < 0)
        {
            Direction = 1;
        }

        if (movement.y > 0)
        {
            Direction = 0;
        }
        if (movement.y < 0)
        {
            Direction = 2;
        }

        Debug.Log(Direction);

        if (movement.magnitude != 0)
        {
            if (!MidAnimation)
            {
                StartCoroutine(WalkAnimation());
            }
        }
        else
        {
            StopCoroutine(WalkAnimation());
            SetCharacterSprite(0); // Index 1 should be the "idle" sprite
        }
    }

    void FixedUpdate()
    {
        if (isDisabled)
        {
            return;
        }

        // do movement
        body.MovePosition(body.position + movement * moveSpeed * Time.fixedDeltaTime);
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
