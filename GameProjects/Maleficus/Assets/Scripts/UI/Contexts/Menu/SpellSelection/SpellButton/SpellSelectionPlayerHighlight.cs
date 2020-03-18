using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpellSelectionPlayerHighlight : BNJMOBehaviour
{
    public EPlayerID PlayerID   { get { return playerID; } }
    public bool IsHighlighted   { get; private set; } = false;

    [SerializeField] private EPlayerID playerID = EPlayerID.NONE;

    private RawImage myImage;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        myImage = GetComponentWithCheck<RawImage>();
    }

    public void ShowHighlight()
    {
        if (IS_NOT_NULL(myImage))
        {
            myImage.enabled = true;
            IsHighlighted = true;
        }
    }

    public void HideHighlight()
    {
        if (IS_NOT_NULL(myImage))
        {
            myImage.enabled = false;
            IsHighlighted = false;
        }
    }
}
