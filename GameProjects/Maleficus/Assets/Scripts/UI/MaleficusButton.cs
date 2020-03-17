using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class MaleficusButton : BNJMOBehaviour
{
    public event Action<MaleficusButton> ButtonHighlighted;
    public event Action<MaleficusButton> ButtonPressed;
    public event Action<MaleficusButton> ButtonUnhighlighted;
    public event Action<MaleficusButton> ButtonSuccessfullyReleased;

    public MaleficusButton LeftButton { get { return leftButton; } set { leftButton = value; } }
    public MaleficusButton RightButton { get { return rightButton; } set { rightButton = value; } }
    public MaleficusButton UpperButton { get { return upperButton; } set { upperButton = value; } }
    public MaleficusButton BottomButton { get { return buttomButton; } set { buttomButton = value; } }

    [Header("Button")]
    [SerializeField] private string buttonName = "MenuButton";
    [SerializeField] private bool writeUppercase = false;
    [SerializeField] private bool overrideGameOjbectName = true;
    
    [Header("Navigation")]
    [SerializeField] private MaleficusButton leftButton;
    [SerializeField] private MaleficusButton buttomButton;
    [SerializeField] private MaleficusButton rightButton;
    [SerializeField] private MaleficusButton upperButton;

    private Button myButton;
    private Image myButtonImage;
    private Text myText;
    private AbstractUIAction[] myUIActions;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        // Get button text
        myText = GetComponentInChildren<Text>();
        if (myText == null)
        {
            myText = GetComponent<Text>();
        }

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

    protected override void OnValidate()
    {
        base.OnValidate();

        ValidateName();

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

    public void Unhighlighted()
    {
        myButtonImage.color = myButton.colors.normalColor;

        InvokeEventIfBound(ButtonUnhighlighted, this);
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

    private void ValidateName()
    {
        // Update GameObject name
        if (overrideGameOjbectName == true)
        {
            gameObject.name = "B_" + buttonName;
        }

        // Update text
        myText = GetComponentInChildren<Text>();
        if (myText == null)
        {
            myText = GetComponent<Text>();
        }
        if (myText)
        {
            if (writeUppercase == true)
            {
                myText.text = buttonName.ToUpper();
            }
            else
            {
                myText.text = buttonName;
            }
        }
    }
}
