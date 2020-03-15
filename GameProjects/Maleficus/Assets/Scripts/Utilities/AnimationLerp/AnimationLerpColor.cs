using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpColor : AnimationLerp<Color>
    {
        private MeshRenderer animationMeshRenderer;

        public void StartAnimation(MeshRenderer meshRenderer)
        {
            animationMeshRenderer = meshRenderer;

            StartAnimation();
        }

        protected override Color Lerp(Color start, Color end, float alpha)
        {
            return Color.LerpUnclamped(start, end, alpha);
        }

        protected override void On_AnimationProgressed(AnimationLerp<Color> animationLerp, Color value)
        {
            base.On_AnimationProgressed(animationLerp, value);

            if (animationMeshRenderer)
            {
                animationMeshRenderer.material.color = value;
            }
        }

        protected override void On_AnimationEnded(AnimationLerp<Color> animationLerp)
        {
            base.On_AnimationEnded(animationLerp);

            animationMeshRenderer = null;
        }
    }
}