using System.Collections;
using UnityEngine;

namespace TypewriterDialoguePackage.Components
{
    /// <summary>
    /// Types of UI animations supported by the manager.
    /// </summary>
    public enum UIAnimationType
    {
        Scroll,     // Animate RectTransform anchored position
        Transform   // Animate Transform position
    }

    /// <summary>
    /// Manages UI animations including scroll and transform animations.
    /// Can be used as a standalone component or integrated with sequence systems.
    /// </summary>
    public class UIAnimationManager : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private bool useUnscaledTime = true;

        // Runtime state
        private Coroutine currentAnimationCoroutine;

        public bool IsAnimating => currentAnimationCoroutine != null;

        /// <summary>
        /// Animate a RectTransform's anchored position (scroll animation).
        /// </summary>
        public void AnimateScroll(RectTransform target, Vector2 startPos, Vector2 endPos, float duration, AnimationCurve curve = null)
        {
            StopCurrentAnimation();
            currentAnimationCoroutine = StartCoroutine(AnimatePanelScroll(target, startPos, endPos, duration, curve));
        }

        /// <summary>
        /// Animate a Transform's position (transform animation).
        /// </summary>
        public void AnimateTransform(Transform target, Vector3 startPos, Vector3 endPos, float duration, AnimationCurve curve = null)
        {
            StopCurrentAnimation();
            currentAnimationCoroutine = StartCoroutine(AnimateTransformPosition(target, startPos, endPos, duration, curve));
        }

        /// <summary>
        /// Animate a RectTransform's anchored position without waiting for completion.
        /// </summary>
        public void AnimateScrollNonBlocking(RectTransform target, Vector2 startPos, Vector2 endPos, float duration, AnimationCurve curve = null)
        {
            StartCoroutine(AnimatePanelScroll(target, startPos, endPos, duration, curve));
        }

        /// <summary>
        /// Animate a Transform's position without waiting for completion.
        /// </summary>
        public void AnimateTransformNonBlocking(Transform target, Vector3 startPos, Vector3 endPos, float duration, AnimationCurve curve = null)
        {
            StartCoroutine(AnimateTransformPosition(target, startPos, endPos, duration, curve));
        }

        /// <summary>
        /// Stop the current animation.
        /// </summary>
        public void StopCurrentAnimation()
        {
            if (currentAnimationCoroutine != null)
            {
                StopCoroutine(currentAnimationCoroutine);
                currentAnimationCoroutine = null;
            }
        }

        /// <summary>
        /// Stop all animations on this manager.
        /// </summary>
        public void StopAllAnimations()
        {
            StopAllCoroutines();
            currentAnimationCoroutine = null;
        }

        /// <summary>
        /// Animate a RectTransform's anchored position coroutine implementation.
        /// </summary>
        private IEnumerator AnimatePanelScroll(RectTransform panel, Vector2 startPos, Vector2 endPos, float duration, AnimationCurve curve)
        {
            if (panel == null) yield break;

            // Use default ease-in-out curve if none provided
            if (curve == null)
            {
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            }

            float elapsed = 0f;
            panel.anchoredPosition = startPos;

            while (elapsed < duration)
            {
                float t = curve.Evaluate(elapsed / duration);
                panel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            panel.anchoredPosition = endPos;
            currentAnimationCoroutine = null;
        }

        /// <summary>
        /// Animate a Transform's position coroutine implementation.
        /// </summary>
        private IEnumerator AnimateTransformPosition(Transform target, Vector3 startPos, Vector3 endPos, float duration, AnimationCurve curve)
        {
            if (target == null) yield break;

            // Use default ease-in-out curve if none provided
            if (curve == null)
            {
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            }

            float elapsed = 0f;
            target.position = startPos;

            while (elapsed < duration)
            {
                float t = curve.Evaluate(elapsed / duration);
                target.position = Vector3.Lerp(startPos, endPos, t);
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            target.position = endPos;
            currentAnimationCoroutine = null;
        }

        private void OnDisable()
        {
            StopAllAnimations();
        }
    }
}