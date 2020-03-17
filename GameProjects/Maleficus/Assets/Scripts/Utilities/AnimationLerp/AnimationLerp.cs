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
        [SerializeField] private string animationName = "AnimLerp_X";
        public float PlayTime = 3.0f;
        public bool PlayInReverse = false;
        public A StartValue;
        public A EndValue;
        public bool IsLoop;
        public AnimationCurve Curve     { get { return curve; } }
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
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

            LogCanvas(69, AnimationName + " - Is Running : " + IsRunning + "\n"
                + "CurrentPercentage : " + CurrentPercentage + "\n"
                + "CurrentAlpha : " + CurrentAlpha);
        }

        public void StartAnimation(ref AnimationLerpWrapper<A> animationLerpWrapper)
        {
            valueWrapper = animationLerpWrapper;

            StartAnimation();
        }

        public void StartAnimation()
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

        public void StopAnimation()
        {
            InvokeEventIfBound(AnimationStopped, this);
            StopCoroutine(CurrentAnimationEnumerator);
        }

        protected IEnumerator CurrentAnimationCoroutine(bool isFirstRun)
        {
            if (isFirstRun == true)
            {
                InvokeEventIfBound(AnimationStarted, this);
            }

            CurrentPercentage = 0.0f;
            CurrentValue = StartValue;
            float startTime = Time.time;
            IsRunning = true;

            while (CurrentPercentage < 1.0f)
            {
                CurrentPercentage = (Time.time - startTime) / PlayTime;

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
                LogConsole(AnimationName + " progressed : " + value.ToString());
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
