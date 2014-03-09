using UnityEngine;

namespace KRES.Extensions
{
    public static class PartExtensions
    {
        #region Animations
        public static void InitiateAnimation(this Part part, string animationName)
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

        public static void PlayAnimation(this Part part, string animationName, float animationSpeed)
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

        public static void PlayAnimation(this Part part, string animationName, float animationSpeed, float animationTime)
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

        public static void PlayAnimation(this Part part, string animationName, float animationSpeed, WrapMode mode)
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

        public static void PlayAnimation(this Part part, string animationName, float animationSpeed, float animationTime, WrapMode mode)
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

        public static bool CheckAnimationPlaying(this Part part, string animationName)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName)) { return anim.isPlaying; }
            return false;
        }

        public static float GetAnimationTime(this Part part, string animationName)
        {
            foreach (Animation anim in part.FindModelAnimators(animationName)) { return anim.animation[animationName].normalizedTime; }
            return 0;
        }
        #endregion
    }
}
