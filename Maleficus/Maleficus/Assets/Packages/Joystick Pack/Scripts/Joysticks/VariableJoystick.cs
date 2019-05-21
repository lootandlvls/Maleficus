using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VariableJoystick : Joystick
{
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }
    public ETouchJoystickType TouchJoystickType { get { return touchJoystickType; } }

    [SerializeField] private ETouchJoystickType touchJoystickType;
    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private ESnappingMode joystickType = ESnappingMode.Dynamic;

    private Vector2 fixedPosition = Vector2.zero;

    protected override void Start()
    {
        base.Start();
        fixedPosition = background.anchoredPosition;
        SetMode(joystickType);

        if (InputManager.Instance.InputMode != EInputMode.TOUCH)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetMode(ESnappingMode joystickType)
    {
        this.joystickType = joystickType;
        if(joystickType == ESnappingMode.Fixed)
        {
            background.anchoredPosition = fixedPosition;
            background.gameObject.SetActive(true);
        }
    }

    

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(joystickType != ESnappingMode.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        }
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = fixedPosition;

        base.OnPointerUp(eventData);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (joystickType == ESnappingMode.Dynamic && magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }
}

public enum ESnappingMode { Fixed, Floating, Dynamic }