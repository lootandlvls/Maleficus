using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

/// <summary>
/// Spawned when a new device has joined and responsible for transmetting the input controls to the DeviceInputSource
/// </summary>
public class PlayerInputListener : MaleficusMonoBehaviour
{
    public event Action<EControllerID, EInputButton> ButtonPressed;
    public event Action<EControllerID, EInputButton> ButtonReleased;

    public Vector2 MoveAxis { get { return moveAxis; } }
    public Vector2 RotateAxis { get { return rotateAxis; } }
    public string DeviceName { get { return deviceName; } }

    private PlayerInput myPlayerInput;
    private Controls controls = null;
    private string deviceName = "";
    private EControllerID myControllerID;
    private Vector2 moveAxis;
    private Vector2 rotateAxis;

    private InputAction inputAction_Move;
    private InputAction inputAction_Rotate;

    private bool canPerformDirectionalButton = true;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        // Create a control only to get Input Actions IDs 
        controls = new Controls();
        controls.Disable();
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Initialze Player Input and callbacks
        myPlayerInput = GetComponentWithCheck<PlayerInput>();
        if (IS_VALID(myPlayerInput))
        {
            InitializePlayerDeviceName();
            InitializePlayerInputCallbacks();
        }
    }

    protected override void Start()
    {
        base.Start();

        if (IS_VALID(myPlayerInput))
        {
            // Initialize Input Actions references
            inputAction_Move = myPlayerInput.currentActionMap.FindAction(controls.Player.Move.id);
            inputAction_Rotate = myPlayerInput.currentActionMap.FindAction(controls.Player.Rotate.id);

            // Inform Device Input Source that a new Player Input has joined and become a ControllerID
            DeviceInputSource deviceInputSource = InputManager.Instance.GetInputSource<DeviceInputSource>();
            if (IS_VALID(deviceInputSource))
            {
                myControllerID = deviceInputSource.OnNewDeviceJoined(this);
                IS_NOT_NONE(myControllerID);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        // Debug
        LogCanvas(9, "Move : " + MoveAxis + "\nRotate : " + RotateAxis);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateAxisInput();
        UpdateDirectionalInput();
    }

    private void InitializePlayerInputCallbacks()
    {
        myPlayerInput.onActionTriggered += (InputAction.CallbackContext callbackContext) =>
        {
            Guid actionID = callbackContext.action.id;

            // Confirm
            if (actionID == controls.Player.Confirm.id)
            {
                ProcessButtonEvent(callbackContext.phase, EInputButton.CONFIRM);
            }
            // Cancel
            else if (actionID == controls.Player.Cancel.id)
            {
                ProcessButtonEvent(callbackContext.phase, EInputButton.CANCEL);
            }
            // Spell_1
            else if (actionID == controls.Player.Spell_1.id)
            {
                ProcessButtonEvent(callbackContext.phase, EInputButton.CAST_SPELL_1);
            }
            // Spell_2
            else if (actionID == controls.Player.Spell_2.id)
            {
                ProcessButtonEvent(callbackContext.phase, EInputButton.CAST_SPELL_2);
            }
            // Spell_3
            else if (actionID == controls.Player.Spell_3.id)
            {
                ProcessButtonEvent(callbackContext.phase, EInputButton.CAST_SPELL_3);
            }
           


        };

        myPlayerInput.onDeviceLost += (PlayerInput playerInput) =>
        {
            Debug.Log("Device '" + playerInput.devices[0].name + "' lost!");
        };

        myPlayerInput.onDeviceRegained += (PlayerInput playerInput) =>
        {
            Debug.Log("Device '" + playerInput.devices[0].name + "' lost!");

        };
    }

    private void InitializePlayerDeviceName()
    {
        if (myPlayerInput.devices.Count > 0)
        {
            foreach (InputDevice inputDevice in myPlayerInput.devices)
            {
                deviceName += inputDevice.displayName + " ";
            }
        }
        else
        {
            LogConsoleWarning("Player input has 0 device!");
        }
    }

    private void ProcessButtonEvent(InputActionPhase actionPhase, EInputButton inputButton)
    {
        switch (actionPhase)
        {
            case InputActionPhase.Started:
                InvokeEventIfBound(ButtonPressed, myControllerID, inputButton);

                break;

            case InputActionPhase.Performed:
                InvokeEventIfBound(ButtonReleased, myControllerID, inputButton);

                break;
        }
    }

    private void UpdateAxisInput()
    {
        moveAxis    = inputAction_Move.ReadValue<Vector2>();
        rotateAxis  = inputAction_Rotate.ReadValue<Vector2>();
    }

    private void UpdateDirectionalInput()
    {
        if (canPerformDirectionalButton == true)
        {
            // Joystick moved beyond threshold
            if (Vector2.Distance(MoveAxis, Vector2.zero) > 0.5f)
            {
                // Horizontal move
                if (Mathf.Abs(MoveAxis.x) > Mathf.Abs(MoveAxis.y))
                {
                    // Right move
                    if (MoveAxis.x > 0.0f)
                    {
                        InvokeEventIfBound(ButtonPressed, myControllerID, EInputButton.RIGHT);
                    }
                    // Left move
                    else
                    {
                        InvokeEventIfBound(ButtonPressed, myControllerID, EInputButton.LEFT);
                    }
                }
                // Vertical move
                else
                {
                    // Up move
                    if (MoveAxis.y > 0.0f)
                    {
                        InvokeEventIfBound(ButtonPressed, myControllerID, EInputButton.UP);
                    }
                    // Down move
                    else
                    {
                        InvokeEventIfBound(ButtonPressed, myControllerID, EInputButton.DOWN);
                    }
                }
                canPerformDirectionalButton = false;
            }
        }
        else
        {
            // Joystick moved below threshold
            if (Vector2.Distance(MoveAxis, Vector2.zero) < 0.5f)
            {
                canPerformDirectionalButton = true;
            }
        }
    }
}
