using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MaleficusButton : MonoBehaviour
{

    [SerializeField] private MaleficusButton leftButton;
    [SerializeField] private MaleficusButton rightButton;
    [SerializeField] private MaleficusButton upperButton;
    [SerializeField] private MaleficusButton buttomButton;

    private Button myButton;
    private AbstractUIAction myMenuAction;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myMenuAction = GetComponent<AbstractUIAction>();
    }

    public void Highlight()
    {
        myButton.Select();
    }

    public void Press()
    {
        myMenuAction.Execute();
    }

    public MaleficusButton GoToNextButton(EButtonDirection buttonDirection)
    {
        MaleficusButton buttonToReturn = null;
        switch(buttonDirection)
        {
            case EButtonDirection.LEFT:
                if (leftButton != null)
                {
                    leftButton.Highlight();
                    buttonToReturn = leftButton;
                }
                break;

            case EButtonDirection.RIGHT:
                if (rightButton != null)
                {
                    rightButton.Highlight();
                    buttonToReturn = rightButton;
                }
                break;

            case EButtonDirection.UP:
                if (upperButton != null)
                {
                    upperButton.Highlight();
                    buttonToReturn = upperButton;
                }
                break;

            case EButtonDirection.DOWN:
                if (buttomButton != null)
                {
                    buttomButton.Highlight();
                    buttonToReturn = buttomButton;
                }
                break;
        }
        return buttonToReturn;
    }
}
