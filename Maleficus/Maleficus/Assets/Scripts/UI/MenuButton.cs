using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MenuButton : MonoBehaviour
{

    [SerializeField] private MenuButton leftButton;
    [SerializeField] private MenuButton rightButton;
    [SerializeField] private MenuButton upperButton;
    [SerializeField] private MenuButton buttomButton;

    private Button myButton;
    private AbstractMenuCommand myMenuCommand;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myMenuCommand = GetComponent<AbstractMenuCommand>();
    }

    public void Highlight()
    {
        myButton.Select();
    }

    public void Press()
    {
        myMenuCommand.Execute();
    }

    public MenuButton GoToNextButton(ButtonDirection buttonDirection)
    {
        MenuButton buttonToReturn = null;
        switch(buttonDirection)
        {
            case ButtonDirection.LEFT:
                if (leftButton != null)
                {
                    leftButton.Highlight();
                    buttonToReturn = leftButton;
                }
                break;

            case ButtonDirection.RIGHT:
                if (rightButton != null)
                {
                    rightButton.Highlight();
                    buttonToReturn = rightButton;
                }
                break;

            case ButtonDirection.UP:
                if (upperButton != null)
                {
                    upperButton.Highlight();
                    buttonToReturn = upperButton;
                }
                break;

            case ButtonDirection.DOWN:
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
