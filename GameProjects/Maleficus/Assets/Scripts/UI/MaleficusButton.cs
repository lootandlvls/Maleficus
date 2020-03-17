using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class MaleficusButton : BNJMOBehaviour
{
    public event Action<MaleficusButton> ButtonHighlighted;
    public event Action<MaleficusButton> ButtonPressed;
    public event Action<MaleficusButton> ButtonUnpressed;
    public event Action<MaleficusButton> ButtonSuccessfullyReleased;

    public MaleficusButton LeftButton { get { return leftButton; } set { leftButton = value; } }
    public MaleficusButton RightButton { get { return rightButton; } set { rightButton = value; } }
    public MaleficusButton UpperButton { get { return upperButton; } set { upperButton = value; } }
    public MaleficusButton BottomButton { get { return buttomButton; } set { buttomButton = value; } }

    [SerializeField] private MaleficusButton leftButton;
    [SerializeField] private MaleficusButton rightButton;
    [SerializeField] private MaleficusButton upperButton;
    [SerializeField] private MaleficusButton buttomButton;

    private Button myButton;
    private Image myButtonImage;
    private AbstractUIAction[] myUIActions;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Get Unity's Button
        myButton = GetComponent<Button>();
        if (IS_NOT_NULL(myButton))
        {
            // Bind clicked event
            GetComponent<Button>().onClick.AddListener(Release);
            
            // Get image
            myButtonImage = myButton.image;
            IS_NOT_NULL(myButtonImage);
        }

        // Get attached UIActions
        myUIActions = GetComponents<AbstractUIAction>();
    }

    public void Highlight()
    {
        myButtonImage.color = myButton.colors.highlightedColor;

        InvokeEventIfBound(ButtonHighlighted, this);
    }

    public void Release()
    {
        myButtonImage.color = myButton.colors.highlightedColor;

        InvokeEventIfBound(ButtonSuccessfullyReleased, this);
    }

    public void Press()
    {
        myButtonImage.color = myButton.colors.selectedColor;

        InvokeEventIfBound(ButtonPressed, this);
    }

    public void Unpress()
    {
        myButtonImage.color = myButton.colors.normalColor;

        InvokeEventIfBound(ButtonUnpressed, this);

    }

    public MaleficusButton GetNextButton(EButtonDirection buttonDirection)
    {
        MaleficusButton buttonToReturn = null;
        switch (buttonDirection)
        {
            case EButtonDirection.LEFT:
                buttonToReturn = leftButton;
                break;

            case EButtonDirection.RIGHT:
                buttonToReturn = rightButton;
                break;

            case EButtonDirection.UP:
                buttonToReturn = upperButton;
                break;

            case EButtonDirection.DOWN:
                buttonToReturn = buttomButton;
                break;
        }
        return buttonToReturn;
    }

    public void UnPopulateNavigationButtons()
    {
        LogConsole("Unpopulating buttons");
        LeftButton = null;
        RightButton = null;
        UpperButton = null;
        BottomButton = null;
    }
}
