using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class MaleficusButton : BNJMOBehaviour
{
    public MaleficusButton LeftButton   { get { return leftButton; }    set { leftButton = value; } }
    public MaleficusButton RightButton  { get { return rightButton; }   set { rightButton = value; } }
    public MaleficusButton UpperButton  { get { return upperButton; }   set { upperButton = value; } }
    public MaleficusButton BottomButton { get { return buttomButton; }  set { buttomButton = value; } }

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

        // Get Unity's Button and its belonging image
        myButton = GetComponent<Button>();
        if (IS_NOT_NULL(myButton))
        {
            myButtonImage = myButton.image;
            IS_NOT_NULL(myButtonImage);
        }

        // Get attached UIActions
        myUIActions = GetComponents<AbstractUIAction>();
    }

    public void Highlight()
    {
        myButtonImage.color = myButton.colors.highlightedColor;

        UIManager.Instance.OnButtonHighlighted(this);

        // Inform attached UIActions
        foreach (AbstractUIAction abstractUIAction in myUIActions)
        {
            abstractUIAction.OnHighlighted();
        }
    }

    public void Press()
    {
        myButtonImage.color = myButton.colors.highlightedColor;

        foreach (AbstractUIAction abstractUIAction in myUIActions)
        {
            abstractUIAction.Execute();
        }
    }

    public void Select()
    {
        myButtonImage.color = myButton.colors.selectedColor;
    }

    public void UnSelect()
    {
        myButtonImage.color = myButton.colors.normalColor;
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
