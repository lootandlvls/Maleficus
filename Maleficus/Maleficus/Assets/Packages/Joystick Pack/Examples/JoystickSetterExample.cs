using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickSetterExample : MonoBehaviour
{
    public VariableJoystick variableJoystick;
    public Text valueText;
    public Image background;
    public Sprite[] axisSprites;

    public void ModeChanged(int index)
    {
        switch(index)
        {
            case 0:
                variableJoystick.SetMode(ESnappingMode.Fixed);
                break;
            case 1:
                variableJoystick.SetMode(ESnappingMode.Floating);
                break;
            case 2:
                variableJoystick.SetMode(ESnappingMode.Dynamic);
                break;
            default:
                break;
        }     
    }

    public void AxisChanged(int index)
    {
        switch (index)
        {
            case 0:
                variableJoystick.AxisOptions = EJoystickAxisRestriction.BOTH;
                background.sprite = axisSprites[index];
                break;
            case 1:
                variableJoystick.AxisOptions = EJoystickAxisRestriction.HORIZONTAL;
                background.sprite = axisSprites[index];
                break;
            case 2:
                variableJoystick.AxisOptions = EJoystickAxisRestriction.VERTICAL;
                background.sprite = axisSprites[index];
                break;
            default:
                break;
        }
    }

    public void SnapX(bool value)
    {
        variableJoystick.SnapX = value;
    }

    public void SnapY(bool value)
    {
        variableJoystick.SnapY = value;
    }

    private void Update()
    {
        valueText.text = "Current Value: " + variableJoystick.Direction;
    }
}