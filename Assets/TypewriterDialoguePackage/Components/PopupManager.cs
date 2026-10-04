using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TypewriterDialoguePackage.Components
{
    /// <summary>
    /// Manages popup window operations including showing, hiding, and configuration.
    /// Can be used as a standalone component or integrated with sequence systems.
    /// </summary>
    public class PopupManager : MonoBehaviour
    {
        [Header("Popup Settings")]
        [SerializeField] private GameObject popupWindow;
        [SerializeField] private float defaultAnimationDuration = 0.3f;
        [SerializeField] private bool useUnscaledTime = true;

        [Header("Default Content")]
        [SerializeField] private string defaultTitle = "Popup Title";
        [SerializeField] private string defaultContent = "Popup content goes here.";
        [SerializeField] private bool defaultShowCloseButton = true;
        [SerializeField] private bool defaultShowBackground = true;

        // Runtime state
        private Coroutine currentAnimationCoroutine;
        private Vector2 originalScale;

        public bool IsPopupVisible => popupWindow != null && popupWindow.activeSelf;
        public bool IsAnimating => currentAnimationCoroutine != null;

        /// <summary>
        /// Set the popup window GameObject.
        /// </summary>
        public void SetPopupWindow(GameObject popup)
        {
            popupWindow = popup;
            if (popupWindow != null)
            {
                RectTransform rect = popupWindow.GetComponent<RectTransform>();
                if (rect != null)
                {
                    originalScale = rect.localScale;
                }
            }
        }

        /// <summary>
        /// Show the popup window with custom content and animation.
        /// </summary>
        public void ShowPopup(string title, string content, bool showCloseButton = true, bool showBackground = true, float animationDuration = 0.3f)
        {
            if (popupWindow == null)
            {
                Debug.LogWarning("PopupManager: No popup window assigned!");
                return;
            }

            // Configure popup content
            ConfigurePopup(title, content, showCloseButton, showBackground);

            // Activate popup
            popupWindow.SetActive(true);

            // Play appearance animation
            if (animationDuration > 0f)
            {
                StopCurrentAnimation();
                currentAnimationCoroutine = StartCoroutine(PlayPopupAnimation(animationDuration, true));
            }
        }

        /// <summary>
        /// Show the popup window with default settings.
        /// </summary>
        public void ShowPopup()
        {
            ShowPopup(defaultTitle, defaultContent, defaultShowCloseButton, defaultShowBackground, defaultAnimationDuration);
        }

        /// <summary>
        /// Hide the popup window with optional animation.
        /// </summary>
        public void HidePopup(float animationDuration = 0.3f)
        {
            if (popupWindow == null) return;

            if (animationDuration > 0f)
            {
                StopCurrentAnimation();
                currentAnimationCoroutine = StartCoroutine(PlayPopupAnimation(animationDuration, false));
            }
            else
            {
                popupWindow.SetActive(false);
            }
        }

        /// <summary>
        /// Hide the popup window immediately without animation.
        /// </summary>
        public void HidePopupImmediate()
        {
            if (popupWindow == null) return;

            StopCurrentAnimation();
            popupWindow.SetActive(false);

            // Reset scale
            RectTransform rect = popupWindow.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.localScale = originalScale;
            }
        }

        /// <summary>
        /// Toggle popup visibility.
        /// </summary>
        public void TogglePopup()
        {
            if (IsPopupVisible)
            {
                HidePopup();
            }
            else
            {
                ShowPopup();
            }
        }

        /// <summary>
        /// Update popup content while it's visible.
        /// </summary>
        public void UpdateContent(string title, string content)
        {
            if (popupWindow == null || !IsPopupVisible) return;

            ConfigurePopup(title, content, defaultShowCloseButton, defaultShowBackground);
        }

        /// <summary>
        /// Configure the popup window with the specified settings.
        /// </summary>
        private void ConfigurePopup(string title, string content, bool showCloseButton, bool showBackground)
        {
            if (popupWindow == null) return;

            // Find and configure title text
            TextMeshProUGUI titleText = popupWindow.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = title;
            }

            // Find and configure content text
            TextMeshProUGUI contentText = popupWindow.transform.Find("ContentText")?.GetComponent<TextMeshProUGUI>();
            if (contentText != null)
            {
                contentText.text = content;
            }

            // Find and configure background
            Transform background = popupWindow.transform.Find("Background");
            if (background != null)
            {
                background.gameObject.SetActive(showBackground);
            }

            // Find and configure close button
            Button closeButton = popupWindow.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.gameObject.SetActive(showCloseButton);
                if (showCloseButton)
                {
                    // Remove existing listeners
                    closeButton.onClick.RemoveAllListeners();
                    // Add hide functionality
                    closeButton.onClick.AddListener(() => HidePopup(defaultAnimationDuration));
                }
            }
        }

        /// <summary>
        /// Play popup appearance/disappearance animation.
        /// </summary>
        private IEnumerator PlayPopupAnimation(float duration, bool show)
        {
            if (popupWindow == null) yield break;

            RectTransform rect = popupWindow.GetComponent<RectTransform>();
            if (rect == null) yield break;

            Vector2 startScale = show ? Vector2.zero : originalScale;
            Vector2 endScale = show ? originalScale : Vector2.zero;

            float elapsed = 0f;
            rect.localScale = startScale;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                // Use smooth step for more natural animation
                float smoothT = t * t * (3f - 2f * t);
                rect.localScale = Vector2.Lerp(startScale, endScale, smoothT);

                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            rect.localScale = endScale;

            // Hide popup after animation completes if we were hiding
            if (!show)
            {
                popupWindow.SetActive(false);
                rect.localScale = originalScale; // Reset for next show
            }

            currentAnimationCoroutine = null;
        }

        /// <summary>
        /// Stop the current animation.
        /// </summary>
        private void StopCurrentAnimation()
        {
            if (currentAnimationCoroutine != null)
            {
                StopCoroutine(currentAnimationCoroutine);
                currentAnimationCoroutine = null;
            }
        }

        private void Awake()
        {
            if (popupWindow != null)
            {
                RectTransform rect = popupWindow.GetComponent<RectTransform>();
                if (rect != null)
                {
                    originalScale = rect.localScale;
                }
                // Start with popup hidden
                popupWindow.SetActive(false);
            }
        }

        private void OnDisable()
        {
            StopCurrentAnimation();
        }
    }
}