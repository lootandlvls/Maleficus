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

        Transform animatedTransform;

        public override void StartAnimation()
        {
            if (animatedTransform == null)
            {
                animatedTransform = transform;
            }

            base.StartAnimation();
        }

        public void StartAnimation(Transform animatedTransform)
        {
            if (IS_NOT_NULL(animatedTransform))
            {
                this.animatedTransform = animatedTransform;

                StartAnimation();
            }
        }

        public void StartAnimation(Transform animatedTransform, Transform startValue, Transform endValue, float playTime = 0.0f, bool isLoop = false, bool playInReverse = false)
        {
            if (IS_NOT_NULL(animatedTransform))
            {
                this.animatedTransform = animatedTransform;

                StartAnimation(startValue, endValue, playTime, isLoop, playInReverse);
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

            animatedTransform.position = position;
            animatedTransform.rotation = rotation;
            animatedTransform.localScale = scale;

            return animatedTransform;
        }

        protected override void On_AnimationEnded(AnimationLerp<Transform> animationLerp)
        {
            base.On_AnimationEnded(animationLerp);

            animatedTransform = null;
        }
    }
}