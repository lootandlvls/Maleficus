using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

[RequireComponent(typeof (AnimationLerpFloat))]
[RequireComponent(typeof (MaleficusButton))]
public class ButtonAnimation : BNJMOBehaviour
{
    private AnimationLerpFloat animationLerpFloat;
    private MaleficusButton maleficusButton;

    private Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();

        originalScale = transform.localScale;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        maleficusButton = GetComponent<MaleficusButton>();
        maleficusButton.ButtonHighlighted += On_MaleficusButton_ButtonHighlighted;
        maleficusButton.ButtonUnhighlighted += On_MaleficusButton_ButtonUnhighlighted;

        animationLerpFloat = GetComponent<AnimationLerpFloat>();
        animationLerpFloat.AnimationProgressed += On_AnimationLerpFloat_AnimationProgressed;
        animationLerpFloat.AnimationName = "AnimLerp_" + gameObject.name;
    }


  
    private void On_MaleficusButton_ButtonHighlighted(MaleficusButton maleficusButton)
    {
        animationLerpFloat.PlayInReverse = false;
        animationLerpFloat.StartAnimation();
    }

    private void On_MaleficusButton_ButtonUnhighlighted(MaleficusButton maleficusButton)
    {
        animationLerpFloat.PlayInReverse = true;
        animationLerpFloat.StartAnimation();
    }

    private void On_AnimationLerpFloat_AnimationProgressed(AnimationLerp<float> animationLerp, float value)
    {
        transform.localScale = originalScale * value;
    }

}
