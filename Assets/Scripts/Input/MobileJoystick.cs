using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string JoystickH = "Horizontal";
    public string JoystickV = "Vertical";
    [Space]
    public float JoystickMaxDistance = 100;
    [Space]
    [SerializeField] private Vector2 PlayerInput;
    private bool IsTouching = false;
    private Image ThumbTracker;

    private void Awake()
    {
        ThumbTracker = transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        ThumbTracker.enabled = false;

        if (IsTouching)
        {
            ThumbTracker.rectTransform.position = Input.mousePosition;
            ThumbTracker.rectTransform.localPosition = Vector3.ClampMagnitude(ThumbTracker.rectTransform.localPosition, JoystickMaxDistance);

            PlayerInput = Vector2.ClampMagnitude(new Vector2(ThumbTracker.rectTransform.localPosition.x / JoystickMaxDistance, ThumbTracker.rectTransform.localPosition.y / JoystickMaxDistance), 1);
        }
        else
        {
            ThumbTracker.rectTransform.localPosition = Vector3.zero;
            PlayerInput = Vector2.zero;
        }

        InputManager.SetAxis(JoystickH, PlayerInput.x);
        InputManager.SetAxis(JoystickV, PlayerInput.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsTouching = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouching = false;
    }
}
