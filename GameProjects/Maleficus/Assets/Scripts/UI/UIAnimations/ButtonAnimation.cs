using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

[RequireComponent(typeof (AnimationLerpFloat))]
[RequireComponent(typeof (BNJMOButton))]
public class ButtonAnimation : BNJMOBehaviour
{
    private AnimationLerpFloat animationLerpFloat;
    private BNJMOButton maleficusButton;

    private Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();

        originalScale = transform.localScale;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        maleficusButton = GetComponent<BNJMOButton>();
        maleficusButton.ButtonHighlighted += On_MaleficusButton_ButtonHighlighted;
        maleficusButton.ButtonUnhighlighted += On_MaleficusButton_ButtonUnhighlighted;

        animationLerpFloat = GetComponent<AnimationLerpFloat>();
        animationLerpFloat.AnimationProgressed += On_AnimationLerpFloat_AnimationProgressed;
        animationLerpFloat.AnimationName = "AnimLerp_" + gameObject.name;
    }


  
    private void On_MaleficusButton_ButtonHighlighted(BNJMOButton maleficusButton)
    {
        animationLerpFloat.PlayInReverse = false;
        animationLerpFloat.StartAnimation();
    }

    private void On_MaleficusButton_ButtonUnhighlighted(BNJMOButton maleficusButton)
    {
        animationLerpFloat.PlayInReverse = true;
        animationLerpFloat.StartAnimation();
    }

    private void On_AnimationLerpFloat_AnimationProgressed(AnimationLerp<float> animationLerp, float value)
    {
        transform.localScale = originalScale * value;
    }

}
