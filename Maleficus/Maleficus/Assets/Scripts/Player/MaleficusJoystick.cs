﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaleficusJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    public ETouchJoystickType JoystickType      { get { return joystickType; } }
    public float Horizontal                     { get { return (snapX) ? SnapFloat(input.x, EJoystickAxisRestriction.HORIZONTAL) : input.x; } }
    public float Vertical                       { get { return (snapY) ? SnapFloat(input.y, EJoystickAxisRestriction.VERTICAL) : input.y; } }
    public Vector2 Direction                    { get { return new Vector2(Horizontal, Vertical); } }
    public float HandleRange                    { get { return handleRange; } set { handleRange = Mathf.Abs(value); } }
    public float DeadZone                       { get { return deadZone; } set { deadZone = Mathf.Abs(value); } }
    public EJoystickAxisRestriction AxisOptions { get { return AxisOptions; } set { axisRestriction = value; } }
    public bool SnapX                           { get { return snapX; } set { snapX = value; } }
    public bool SnapY                           { get { return snapY; } set { snapY = value; } }
    public float MoveThreshold                  { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }


    [SerializeField] private ETouchJoystickType joystickType;

    [Header("Joystick Settings")]
    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private ESnappingMode snappingMode = ESnappingMode.Dynamic;
    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private EJoystickAxisRestriction axisRestriction = EJoystickAxisRestriction.BOTH;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;
    

    [Header("Joystick Colors")]
    [SerializeField] private Color idleColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color canTriggerButtonColor;

    private RectTransform background = null;
    private RectTransform handle = null;
    private Vector2 fixedPosition = Vector2.zero;
    private RectTransform baseRect = null;
    private Image handleImage;
    private Canvas canvas;
    private Camera cam;
    private Vector2 input = Vector2.zero;
    private EJoystickState myState;

    private void Awake()
    {
        background = transform.GetChild(0).GetComponent<RectTransform>();
        handle = background.GetChild(0).GetComponent<RectTransform>();
        handleImage = handle.GetComponent<Image>();
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
}

    protected virtual void Start()
    {
        if (canvas == null)
        {
            Debug.LogError("The Joystick is not placed inside a canvas");
        }

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;


        fixedPosition = background.anchoredPosition;
        SetMode(snappingMode);

        if (InputManager.Instance.InputMode != EInputMode.TOUCH)
        {
            gameObject.SetActive(false);
        }

        UpdateState(EJoystickState.IDLE);
    }

    private void Update()
    {
        if ((Vertical != 0.0f) || (Horizontal != 0.0f))
        {
            InputManager.Instance.OnJoystickMoved(new Vector2(Horizontal, Vertical), joystickType);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (snappingMode != ESnappingMode.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        }

        //if (joystickType != ETouchJoystickType.MOVE)              // TODO: Add Joystick pressed for charging
        //{
        //    InputManager.Instance.OnJoystickPressed(joystickType);
        //}

        OnDrag(eventData);

        // Update Joystick appearance 
        if (input.magnitude > MaleficusTypes.SPELL_BUTTON_THRESHOLD)
        {
            UpdateState(EJoystickState.SELECTED_CAN_TRIGGER_BUTTON);
        }
        else
        {
            UpdateState(EJoystickState.SELECTED_CANNOT_TRIGGER_BUTTON);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            cam = canvas.worldCamera;
        }

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;
       
        // Update Joystick appearance 
        if (input.magnitude > MaleficusTypes.SPELL_BUTTON_THRESHOLD)
        {
            UpdateState(EJoystickState.SELECTED_CAN_TRIGGER_BUTTON);
        }
        else
        {
            UpdateState(EJoystickState.SELECTED_CANNOT_TRIGGER_BUTTON);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = fixedPosition;

        if ((joystickType != ETouchJoystickType.MOVE) /*&& (input.magnitude > MaleficusTypes.SPELL_BUTTON_THRESHOLD)*/)
        {
            InputManager.Instance.OnJoystickReleased(joystickType);
        }

        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        UpdateState(EJoystickState.IDLE);
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (snappingMode == ESnappingMode.Dynamic && magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }


        if (magnitude > deadZone)
        {
            if (magnitude > 1)
            {
                input = normalised;
            }
        }
        else
        {
            input = Vector2.zero;
        }
    }

    private void FormatInput()
    {
        if (axisRestriction == EJoystickAxisRestriction.HORIZONTAL)
        {
            input = new Vector2(input.x, 0f);
        }
        else if (axisRestriction == EJoystickAxisRestriction.VERTICAL)
        {
            input = new Vector2(0f, input.y);
        }
    }

    private float SnapFloat(float value, EJoystickAxisRestriction snapAxis)
    {
        if (value == 0)
            return value;

        if (axisRestriction == EJoystickAxisRestriction.BOTH)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == EJoystickAxisRestriction.HORIZONTAL)
            {
                if (angle < 22.5f || angle > 157.5f)
                {
                    return 0;
                }
                else
                {
                    return (value > 0) ? 1 : -1;
                }
            }
            else if (snapAxis == EJoystickAxisRestriction.VERTICAL)
            {
                if (angle > 67.5f && angle < 112.5f)
                {
                    return 0;
                }
                else
                {
                    return (value > 0) ? 1 : -1;
                }
            }
            return value;
        }
        else
        {
            if (value > 0)
            {
                return 1;
            }
            if (value < 0)
            {
                return -1;
            }
        }
        return 0;
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }

    public void SetMode(ESnappingMode joystickType)
    {
        this.snappingMode = joystickType;
        if (joystickType == ESnappingMode.Fixed)
        {
            background.anchoredPosition = fixedPosition;
            background.gameObject.SetActive(true);
        }
    }


    private void UpdateState(EJoystickState newState)
    {
        if (newState == myState) return;
        myState = newState;

        switch (newState)
        {
            case EJoystickState.IDLE:
                handleImage.color = idleColor;
                break;

            case EJoystickState.SELECTED_CANNOT_TRIGGER_BUTTON:
                handleImage.color = selectedColor;
                break;

            case EJoystickState.SELECTED_CAN_TRIGGER_BUTTON:
                if (joystickType != ETouchJoystickType.MOVE)
                {
                    handleImage.color = canTriggerButtonColor;
                }
                break;
        }
    }

    public void ReloadJoystick(float rechargeTime)
    {
        Debug.Log("reload joystick " + gameObject.name + " : " + rechargeTime);
        StopAllCoroutines();

        StartCoroutine(ReloadJoystickCoroutine(rechargeTime));
    }

    private IEnumerator ReloadJoystickCoroutine(float rechargeTime)
    {
        float startTime = Time.time;
        while (Time.time - startTime < rechargeTime)
        {
            handleImage.fillAmount = (Time.time - startTime) / rechargeTime;

            yield return new WaitForEndOfFrame();
        }
    }
}