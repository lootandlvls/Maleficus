//  
// Copyright (c) BNJMO
// Too lazy to create a license.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BNJMO
{
    public abstract class AnimationLerp<A> : BNJMOBehaviour
    {
        public event Action<AnimationLerp<A>> AnimationStarted;
        public event Action<AnimationLerp<A>, A> AnimationProgressed;
        public event Action<AnimationLerp<A>> AnimationRlooped;
        public event Action<AnimationLerp<A>> AnimationEnded;
        public event Action<AnimationLerp<A>> AnimationStopped;

        public string AnimationName { get { return animationName; } set { animationName = value; } }
        [Header("Animation Lerp")]
        [SerializeField] private string animationName = "AnimLerp_X";
        public float PlayTime = 3.0f;
        public float StartDelay = 0.0f;
        public bool PlayInReverse = false;
        public A StartValue;
        public A EndValue;
        public bool IsLoop;
        public AnimationCurve Curve     { get { return curve; } }
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        [Header("Debug")]
        [SerializeField] bool logAnimationEvents = false;

        public A CurrentValue           { get; private set; }
        public float CurrentPercentage  { get; private set; }
        public float CurrentAlpha       { get; private set; }
        public bool IsRunning           { get; private set; } = false;

        private AnimationLerpWrapper<A> valueWrapper = new AnimationLerpWrapper<A>();

        private IEnumerator CurrentAnimationEnumerator;

        protected override void Awake()
        {
            base.Awake();

            AnimationStarted += On_AnimationStarted;
            AnimationProgressed += On_AnimationProgressed;
            AnimationRlooped += On_AnimationRlooped;
            AnimationEnded += On_AnimationEnded;
            AnimationStopped += On_AnimationStopped;
        }

        protected override void Update()
        {
            base.Update();

            if (logAnimationEvents == true)
            {
                LogCanvas(69, AnimationName + " - Is Running : " + IsRunning + "\n"
                    + "CurrentPercentage : " + CurrentPercentage + "\n"
                    + "CurrentAlpha : " + CurrentAlpha);
            }
        }

        public virtual void StartAnimation(ref AnimationLerpWrapper<A> animationLerpWrapper)
        {
            valueWrapper = animationLerpWrapper;

            StartAnimation();
        }

        public virtual void StartAnimation(A startValue, A endValue, float playTime = 0.0f, bool isLoop = false, bool playInReverse = false, float startDelay = 0.0f)
        {
            StartValue = startValue;
            EndValue = endValue;
            if (playTime > 0.0f)
            {
                PlayTime = playTime;
            }
            if (startDelay > 0.0f)
            {
                StartDelay = startDelay;
            }
            IsLoop = isLoop;
            PlayInReverse = playInReverse;

            StartAnimation();
        }

        public virtual void StartAnimation()
        {
            if (PlayTime > 0.0f)
            {
                StartNewCoroutine(ref CurrentAnimationEnumerator, CurrentAnimationCoroutine(true));
            }
            else
            {
                LogConsoleError("Trying to start an AnimationLerp that has playTime set to 0!");
            }
        }

        public virtual void StopAnimation(bool setEndValue = false)
        {
            InvokeEventIfBound(AnimationStopped, this);
            StopCoroutine(CurrentAnimationEnumerator);

            if (setEndValue == true)
            {
                ProgressLerpAnimation(1.0f);
            }
        }

        protected IEnumerator CurrentAnimationCoroutine(bool isFirstRun)
        {
            if (isFirstRun == true)
            {
                InvokeEventIfBound(AnimationStarted, this);

                if (StartDelay > 0.0f)
                {
                    yield return new WaitForSeconds(StartDelay);
                }
            }

            CurrentPercentage = 0.0f;
            CurrentValue = StartValue;
            float startTime = Time.time;
            IsRunning = true;

            while (CurrentPercentage < 1.0f)
            {
                float percentage = (Time.time - startTime) / PlayTime;
                ProgressLerpAnimation(percentage);

                InvokeEventIfBound(AnimationProgressed, this, CurrentValue);

                yield return new WaitForEndOfFrame();
            }
            CurrentPercentage = 1.0f;



            if (IsLoop == true)
            {
                InvokeEventIfBound(AnimationRlooped, this);
                StartNewCoroutine(ref CurrentAnimationEnumerator, CurrentAnimationCoroutine(true));
            }
            else
            {
                IsRunning = false;
                valueWrapper = new AnimationLerpWrapper<A>();
                InvokeEventIfBound(AnimationEnded, this);
            }
        }

        private void ProgressLerpAnimation(float percentage)
        {
            CurrentPercentage = percentage;

            if (PlayInReverse == false)
            {
                CurrentAlpha = curve.Evaluate(CurrentPercentage);
            }
            else
            {
                CurrentAlpha = curve.Evaluate(1.0f - CurrentPercentage);
            }

            CurrentValue = Lerp(StartValue, EndValue, CurrentAlpha);

            valueWrapper.Value = CurrentValue;
        }

        protected abstract A Lerp(A start, A end, float alpha);


        #region Events Callbacks
        protected virtual void On_AnimationStarted(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " started. Play time : " + PlayTime);
            }
        }

        protected virtual void On_AnimationProgressed(AnimationLerp<A> animationLerp, A value)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " progressed : " + CurrentPercentage);
            }
        }

        private void On_AnimationRlooped(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " relooped");
            }
        }

        protected virtual void On_AnimationEnded(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " ended");
            }
        }

        private void On_AnimationStopped(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " stopped");
            }
        }
        #endregion
    }
}
