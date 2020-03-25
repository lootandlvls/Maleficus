using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

//[RequireComponent(typeof(Button))]
public class BNJMOButton : BNJMOBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action<BNJMOButton> ButtonHighlighted;
    public event Action<BNJMOButton> ButtonPressed;
    public event Action<BNJMOButton> ButtonUnhighlighted;
    public event Action<BNJMOButton> ButtonSuccessfullyReleased;

    public BNJMOButton LeftButton { get { return leftButton; } set { leftButton = value; } }
    public BNJMOButton RightButton { get { return rightButton; } set { rightButton = value; } }
    public BNJMOButton UpperButton { get { return upperButton; } set { upperButton = value; } }
    public BNJMOButton BottomButton { get { return buttomButton; } set { buttomButton = value; } }

    [Header("Button")]
    [SerializeField] private string buttonName = "MenuButton";
    [SerializeField] private bool writeUppercase = false;
    [SerializeField] private bool overrideGameOjbectName = true;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightedColor = Color.white;
    [SerializeField] private Color pressedColor = Color.white;
    
    [Header("Navigation")]
    [SerializeField] private BNJMOButton leftButton;
    [SerializeField] private BNJMOButton buttomButton;
    [SerializeField] private BNJMOButton rightButton;
    [SerializeField] private BNJMOButton upperButton;

    //private Button myButton;
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

        // Get Image
        myButtonImage = GetComponentWithCheck<Image>();

        //myButton = GetComponent<Button>();
        //if (IS_NOT_NULL(myButton))
        //{
        //    // Bind clicked event
        //    GetComponent<Button>().onClick.AddListener(Release);
            
        //    // Get image
        //    IS_NOT_NULL(myButtonImage);
        //}

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
        //myButtonImage.color = myButton.colors.highlightedColor;
        myButtonImage.color = highlightedColor;

        InvokeEventIfBound(ButtonHighlighted, this);
    }

    public void Release()
    {
        //myButtonImage.color = myButton.colors.highlightedColor;
        myButtonImage.color = highlightedColor;

        InvokeEventIfBound(ButtonSuccessfullyReleased, this);
    }

    public void Press()
    {
        //myButtonImage.color = myButton.colors.selectedColor;
        myButtonImage.color = pressedColor;

        InvokeEventIfBound(ButtonPressed, this);
    }

    public void Unhighlighted()
    {
        //myButtonImage.color = myButton.colors.normalColor;
        myButtonImage.color = normalColor;

        InvokeEventIfBound(ButtonUnhighlighted, this);
    }

    public BNJMOButton GetNextButton(EButtonDirection buttonDirection)
    {
        BNJMOButton buttonToReturn = null;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        Press();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }
}
