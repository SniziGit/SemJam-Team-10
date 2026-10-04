using System.Collections;
using TMPro;
using UnityEngine;

namespace TypewriterDialoguePackage.Components
{
    /// <summary>
    /// Manages various visual text effects including shake, wave, gradient, shadow, fade, scale, and color effects.
    /// Can be used as a standalone component or integrated with sequence systems.
    /// </summary>
    public class TextEffectManager : MonoBehaviour
    {
        [Header("Effect Settings")]
        [SerializeField] private bool useUnscaledTime = true;
        [SerializeField] private TextMeshProUGUI targetText;

        // Runtime state
        private Coroutine currentEffectCoroutine;
        private Vector3 originalPosition;
        private Vector3 originalScale;
        private Color originalColor;

        public bool IsEffectRunning => currentEffectCoroutine != null;

        /// <summary>
        /// Set the target text component for effects.
        /// </summary>
        public void SetTargetText(TextMeshProUGUI text)
        {
            targetText = text;
            if (targetText != null)
            {
                originalPosition = targetText.transform.localPosition;
                originalScale = targetText.transform.localScale;
                originalColor = targetText.color;
            }
        }

        /// <summary>
        /// Apply a shake effect to the target text.
        /// </summary>
        public void ApplyShakeEffect(float intensity, float speed, float duration, bool loop = false)
        {
            StopCurrentEffect();
            currentEffectCoroutine = StartCoroutine(ShakeEffect(intensity, speed, duration, loop));
        }

        /// <summary>
        /// Apply a wave effect to the target text.
        /// </summary>
        public void ApplyWaveEffect(float amplitude, float frequency, float duration, bool loop = false)
        {
            StopCurrentEffect();
            currentEffectCoroutine = StartCoroutine(WaveEffect(amplitude, frequency, duration, loop));
        }

        /// <summary>
        /// Apply a gradient effect to the target text.
        /// </summary>
        public void ApplyGradientEffect(Gradient gradient, float duration, bool loop = false)
        {
            StopCurrentEffect();
            currentEffectCoroutine = StartCoroutine(GradientEffect(gradient, duration, loop));
        }

        /// <summary>
        /// Apply a shadow effect to the target text.
        /// </summary>
        public void ApplyShadowEffect(Color shadowColor, Vector2 offset, float blur)
        {
            if (targetText == null) return;

            // TextMeshPro has built-in shadow support
            targetText.fontMaterial.EnableKeyword("UNDERLAY_ON");
            targetText.fontMaterial.SetFloat("_UnderlayOffsetX", offset.x);
            targetText.fontMaterial.SetFloat("_UnderlayOffsetY", offset.y);
            targetText.fontMaterial.SetColor("_UnderlayColor", shadowColor);
            targetText.fontMaterial.SetFloat("_UnderlaySoftness", blur);
        }

        /// <summary>
        /// Apply a fade effect to the target text.
        /// </summary>
        public void ApplyFadeEffect(float startAlpha, float endAlpha, float duration, bool loop = false)
        {
            StopCurrentEffect();
            currentEffectCoroutine = StartCoroutine(FadeEffect(startAlpha, endAlpha, duration, loop));
        }

        /// <summary>
        /// Apply a scale effect to the target text.
        /// </summary>
        public void ApplyScaleEffect(float startScale, float endScale, float duration, bool loop = false)
        {
            StopCurrentEffect();
            currentEffectCoroutine = StartCoroutine(ScaleEffect(startScale, endScale, duration, loop));
        }

        /// <summary>
        /// Apply a color effect to the target text.
        /// </summary>
        public void ApplyColorEffect(Color startColor, Color endColor, float duration, bool loop = false)
        {
            StopCurrentEffect();
            currentEffectCoroutine = StartCoroutine(ColorEffect(startColor, endColor, duration, loop));
        }

        /// <summary>
        /// Stop the currently running effect and reset to original state.
        /// </summary>
        public void StopCurrentEffect()
        {
            if (currentEffectCoroutine != null)
            {
                StopCoroutine(currentEffectCoroutine);
                currentEffectCoroutine = null;
            }

            ResetToOriginalState();
        }

        /// <summary>
        /// Reset the target text to its original state.
        /// </summary>
        public void ResetToOriginalState()
        {
            if (targetText == null) return;

            targetText.transform.localPosition = originalPosition;
            targetText.transform.localScale = originalScale;
            targetText.color = originalColor;
        }

        /// <summary>
        /// Shake effect coroutine implementation.
        /// </summary>
        private IEnumerator ShakeEffect(float intensity, float speed, float duration, bool loop)
        {
            if (targetText == null) yield break;

            float elapsed = 0f;

            do
            {
                while (elapsed < duration)
                {
                    float xOffset = Random.Range(-intensity, intensity);
                    float yOffset = Random.Range(-intensity, intensity);
                    targetText.transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);

                    elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return new WaitForSeconds(1f / speed);
                }

                targetText.transform.localPosition = originalPosition;
                elapsed = 0f;

            } while (loop);
        }

        /// <summary>
        /// Wave effect coroutine implementation.
        /// </summary>
        private IEnumerator WaveEffect(float amplitude, float frequency, float duration, bool loop)
        {
            if (targetText == null) yield break;

            float elapsed = 0f;

            do
            {
                while (elapsed < duration)
                {
                    float yOffset = Mathf.Sin(elapsed * frequency) * amplitude;
                    targetText.transform.localPosition = originalPosition + new Vector3(0, yOffset, 0);

                    elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }

                targetText.transform.localPosition = originalPosition;
                elapsed = 0f;

            } while (loop);
        }

        /// <summary>
        /// Gradient effect coroutine implementation.
        /// </summary>
        private IEnumerator GradientEffect(Gradient gradient, float duration, bool loop)
        {
            if (targetText == null) yield break;

            float elapsed = 0f;

            do
            {
                while (elapsed < duration)
                {
                    Color targetColor = gradient.Evaluate(elapsed / duration);
                    targetText.color = targetColor;

                    elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }

                elapsed = 0f;

            } while (loop);
        }

        /// <summary>
        /// Fade effect coroutine implementation.
        /// </summary>
        private IEnumerator FadeEffect(float startAlpha, float endAlpha, float duration, bool loop)
        {
            if (targetText == null) yield break;

            float elapsed = 0f;

            do
            {
                while (elapsed < duration)
                {
                    float t = elapsed / duration;
                    float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
                    targetText.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);

                    elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }

                elapsed = 0f;

            } while (loop);
        }

        /// <summary>
        /// Scale effect coroutine implementation.
        /// </summary>
        private IEnumerator ScaleEffect(float startScale, float endScale, float duration, bool loop)
        {
            if (targetText == null) yield break;

            float elapsed = 0f;

            do
            {
                while (elapsed < duration)
                {
                    float t = elapsed / duration;
                    float currentScale = Mathf.Lerp(startScale, endScale, t);
                    targetText.transform.localScale = originalScale * currentScale;

                    elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }

                elapsed = 0f;

            } while (loop);
        }

        /// <summary>
        /// Color effect coroutine implementation.
        /// </summary>
        private IEnumerator ColorEffect(Color startColor, Color endColor, float duration, bool loop)
        {
            if (targetText == null) yield break;

            float elapsed = 0f;

            do
            {
                while (elapsed < duration)
                {
                    float t = elapsed / duration;
                    targetText.color = Color.Lerp(startColor, endColor, t);

                    elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }

                elapsed = 0f;

            } while (loop);
        }

        private void Awake()
        {
            if (targetText != null)
            {
                originalPosition = targetText.transform.localPosition;
                originalScale = targetText.transform.localScale;
                originalColor = targetText.color;
            }
        }

        private void OnDisable()
        {
            StopCurrentEffect();
        }
    }
}