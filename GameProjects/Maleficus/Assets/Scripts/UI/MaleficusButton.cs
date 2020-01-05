using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class MaleficusButton : MonoBehaviour
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
    private AbstractUIAction[] myUIActions;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myUIActions = GetComponents<AbstractUIAction>();
    }

    public void Highlight()
    {
        myButton.Select();

        UIManager.Instance.OnButtonHighlighted(this);

        foreach (AbstractUIAction abstractUIAction in myUIActions)
        {
            abstractUIAction.OnHighlighted();
        }
    }

    public void Press()
    {
        foreach (AbstractUIAction abstractUIAction in myUIActions)
        {
            abstractUIAction.Execute();
        }
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
        LeftButton = null;
        RightButton = null;
        UpperButton = null;
        BottomButton = null;
    }
}
