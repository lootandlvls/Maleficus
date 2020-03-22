using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpTransform : AnimationLerp<Transform>
    {
        [Header("Transform")]
        public bool LerpPosition = true;
        public bool LerpRotation = true;
        public bool LerpScale = true;

        public Transform AnimatedTransform;

        public override void StartAnimation()
        {
            if (AnimatedTransform == null)
            {
                AnimatedTransform = transform;
            }

            base.StartAnimation();
        }

        public void StartAnimation(Transform animatedTransform)
        {
            if (IS_NOT_NULL(animatedTransform))
            {
                AnimatedTransform = animatedTransform;

                StartAnimation();
            }
        }

        protected override Transform Lerp(Transform start, Transform end, float alpha)
        {
            Vector3 position = start.position;
            if (LerpPosition == true)
            {
                position = Vector3.LerpUnclamped(start.position, end.position, alpha);
            }

            Quaternion rotation = start.rotation;
            if (LerpRotation == true)
            {
                rotation = Quaternion.LerpUnclamped(start.rotation, end.rotation, alpha);
            }

            Vector3 scale = start.localScale;
            if (LerpScale == true)
            {
                scale = Vector3.LerpUnclamped(start.localScale, end.localScale, alpha);
            }

            AnimatedTransform.position = position;
            AnimatedTransform.rotation = rotation;
            AnimatedTransform.localScale = scale;

            return AnimatedTransform;
        }

        protected override void On_AnimationEnded(AnimationLerp<Transform> animationLerp)
        {
            base.On_AnimationEnded(animationLerp);

            AnimatedTransform = null;
        }
    }
}