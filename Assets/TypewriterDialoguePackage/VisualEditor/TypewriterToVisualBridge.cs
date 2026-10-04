using UnityEngine;
using System.Collections.Generic;
using TypewriterDialoguePackage.Components;

namespace TypewriterDialoguePackage.VisualEditor
{
    /// <summary>
    /// Simplified bridge component for dialogue system integration.
    /// Provides a unified interface for dialogue execution and optional component management.
    /// </summary>
    public class TypewriterToVisualBridge : MonoBehaviour
    {
        [Header("Dialogue References")]
        [SerializeField] private DialogueGraph dialogueGraph;
        [SerializeField] private DialogueGraphRunner graphRunner;

        [Header("Optional Enhanced Components")]
        [SerializeField] private TextEffectManager textEffectManager;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private UIAnimationManager uiAnimationManager;

        /// <summary>
        /// Start dialogue using the visual editor system.
        /// </summary>
        public void StartDialogue()
        {
            if (graphRunner != null && dialogueGraph != null)
            {
                graphRunner.SetDialogueGraph(dialogueGraph);
                graphRunner.StartDialogue();
            }
            else
            {
                Debug.LogError("Dialogue references not set!");
            }
        }

        /// <summary>
        /// Stop the current dialogue.
        /// </summary>
        public void StopDialogue()
        {
            if (graphRunner != null)
            {
                graphRunner.StopDialogue();
            }
        }

        /// <summary>
        /// Get a variable value from the dialogue system.
        /// </summary>
        public T GetVariable<T>(string key, T defaultValue = default(T))
        {
            if (graphRunner != null)
            {
                return graphRunner.GetVariable<T>(key, defaultValue);
            }
            return defaultValue;
        }

        /// <summary>
        /// Set a variable value in the dialogue system.
        /// </summary>
        public void SetVariable(string key, object value)
        {
            if (graphRunner != null)
            {
                graphRunner.SetVariable(key, value);
            }
        }

        /// <summary>
        /// Check if dialogue is currently running.
        /// </summary>
        public bool IsDialogueRunning()
        {
            return graphRunner != null && graphRunner.IsRunning;
        }

        /// <summary>
        /// Skip the current dialogue step.
        /// </summary>
        public void SkipCurrentStep()
        {
            if (graphRunner != null)
            {
                graphRunner.SkipCurrentStep();
            }
        }

        /// <summary>
        /// Apply a text effect to the current dialogue text.
        /// </summary>
        public void ApplyTextEffect(string effectType, params object[] parameters)
        {
            if (textEffectManager != null && graphRunner != null)
            {
                // Get the current text component from the runner
                // This would require additional access to internal runner state
                Debug.Log("Text effect application through bridge");
            }
        }

        /// <summary>
        /// Show a popup window.
        /// </summary>
        public void ShowPopup(string title, string content, bool showCloseButton = true, bool showBackground = true)
        {
            if (popupManager != null)
            {
                popupManager.ShowPopup(title, content, showCloseButton, showBackground);
            }
        }

        /// <summary>
        /// Hide the current popup window.
        /// </summary>
        public void HidePopup()
        {
            if (popupManager != null)
            {
                popupManager.HidePopup();
            }
        }

        private void Start()
        {
            // Auto-assign references if not set
            if (graphRunner == null)
            {
                graphRunner = GetComponent<DialogueGraphRunner>();
            }

            if (textEffectManager == null)
            {
                textEffectManager = GetComponent<TextEffectManager>();
            }

            if (popupManager == null)
            {
                popupManager = GetComponent<PopupManager>();
            }

            if (uiAnimationManager == null)
            {
                uiAnimationManager = GetComponent<UIAnimationManager>();
            }
        }
    }
}