using System;
using UnityEngine;

namespace KRES
{
    public static class Animate
    {
        public static void InitiateAnimation(Part part, string animationName)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName))
            {
                AnimationState state = anim.animation[animationName];
                state.normalizedSpeed = 0;
                state.normalizedTime = 0;
                state.layer = 1;
                state.enabled = false;
            }
        }

        #region PlayAnimation()
        public static void PlayAnimation(Part part, string animationName, float animationSpeed)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName))
            {
                AnimationState state = anim.animation[animationName];
                state.normalizedSpeed = animationSpeed;
                state.normalizedTime = 0;
                state.wrapMode = WrapMode.Clamp;
                state.enabled = true;
                anim.Play(animationName);
            }
        }

        public static void PlayAnimation(Part part, string animationName, float animationSpeed, float animationTime)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName))
            {
                AnimationState state = anim.animation[animationName];
                state.normalizedSpeed = animationSpeed;
                state.normalizedTime = animationTime;
                state.wrapMode = WrapMode.Clamp;
                state.enabled = true;
                anim.Play(animationName);
            }
        }

        public static void PlayAnimation(Part part, string animationName, float animationSpeed, WrapMode mode)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName))
            {
                AnimationState state = anim.animation[animationName];
                state.normalizedSpeed = animationSpeed;
                state.normalizedTime = 0;
                state.wrapMode = mode;
                state.enabled = true;
                anim.Play(animationName);
            }
        }

        public static void PlayAnimation(Part part, string animationName, float animationSpeed, float animationTime, WrapMode mode)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName))
            {
                AnimationState state = anim.animation[animationName];
                state.normalizedSpeed = animationSpeed;
                state.normalizedTime = animationTime;
                state.wrapMode = mode;
                state.enabled = true;
                anim.Play(animationName);
            }
        }
        #endregion

        public static bool CheckAnimationPlaying(Part part, string animationName)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName)) { return anim.isPlaying; }
            return false;
        }

        public static float GetAnimationTime(Part part, string animationName)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName)) { return anim.animation[animationName].normalizedTime; }
            return 0;
        }
    }
}
