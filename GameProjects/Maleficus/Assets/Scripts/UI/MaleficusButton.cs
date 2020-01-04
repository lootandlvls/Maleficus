using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class MaleficusButton : MonoBehaviour
{

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
}
