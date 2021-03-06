using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private FootstepsSystem Footsteps;
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
    private bool dropping = false;

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
        Footsteps.PlayFootstepSounds();
        yield return new WaitForSeconds(WalkAnimationLength / 4);
        SetCharacterSprite(2);
        yield return new WaitForSeconds(WalkAnimationLength / 4);
        SetCharacterSprite(3);
        Footsteps.PlayFootstepSounds();
        yield return new WaitForSeconds(WalkAnimationLength / 4);

        MidAnimation = false;
    }

    private IEnumerator DropCandy()
    {
        {
            dropping = true;
            float rand = Random.Range(0.05f, 0.3f);
            yield return new WaitForSeconds(rand);
            DungeonManager.DropWrappers(transform.position);
            dropping = false;
        }
    }

    void Awake()
    {
        GetComponent<Alchemy.Stats.ActorStats>().CurrentLevel = SettingsSingleton.instance.playerLevel;

        if (SettingsSingleton.instance.WasPlayerKilled)
            Alchemy.Inventory.Inventory.Items.Clear();

        Instance = this;
        body = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
        Footsteps = GetComponent<FootstepsSystem>();
    }

    void Update()
    {
        // do input
        movement.x = InputManager.GetAxis("Horizontal");
        movement.y = InputManager.GetAxis("Vertical");

        if (movement.magnitude != 0 && !isDisabled && !PauseMenu.MenuOpen)
        {
            Footsteps.Mute = false;

            if (!MidAnimation)
            {
                StartCoroutine(WalkAnimation());
            }

            if (!dropping)
            {
                StartCoroutine(DropCandy());
            }
        }
        else
        {
            StopCoroutine(WalkAnimation());
            StopCoroutine(DropCandy());
            SetCharacterSprite(1); // Index 1 should be the "idle" sprite
            Footsteps.Mute = true;
        }

        if (isDisabled || PauseMenu.MenuOpen)
        {
            return;
        }

        if (movement.x > 0)
        {
            Direction = 3;
        }
        if (movement.x < 0)
        {
            Direction = 1;
        }

        if ((movement.y > 0 && movement.x == 0) || (movement.y > (movement.x < 0 ? -movement.x : movement.x)))
        {
            Direction = 0;
        }
        if ((movement.y < 0 && movement.x == 0) || (movement.y < (movement.x > 0 ? -movement.x : movement.x)))
        {
            Direction = 2;
        }

        Debug.Log(Direction);
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
