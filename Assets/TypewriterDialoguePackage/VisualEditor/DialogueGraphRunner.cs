using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using TypewriterDialoguePackage.Components;
using TypewriterDialoguePackage.Utility;

namespace TypewriterDialoguePackage.VisualEditor
{
    /// <summary>
    /// Runtime component that executes a dialogue graph.
    /// Handles node execution, state management, and UI integration.
    /// </summary>
    public class DialogueGraphRunner : MonoBehaviour
    {
        [Serializable]
        public class VariableOverride
        {
            public string variableName;
            public bool overrideGraphDefault;
            public string stringValue;
            public int integerValue;
            public float floatValue;
            public bool booleanValue;

            public object GetValue(SetVariableNode.VariableType variableType)
            {
                switch (variableType)
                {
                    case SetVariableNode.VariableType.Integer:
                        return integerValue;
                    case SetVariableNode.VariableType.Float:
                        return floatValue;
                    case SetVariableNode.VariableType.Boolean:
                        return booleanValue;
                    default:
                        return stringValue;
                }
            }
        }

        [Serializable]
        public class FunctionBinding
        {
            public string functionName;
            public UnityEvent function = new UnityEvent();
        }

        [Serializable]
        public class ButtonBinding
        {
            public string inputName;
            public Button button;
        }

        [Header("Dialogue Graph")]
        [SerializeField] private DialogueGraph dialogueGraph;
        [SerializeField] private bool playOnStart = true;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Component References")]
        [SerializeField] private TextEffectManager textEffectManager;
        [SerializeField] private PopupManager popupManager;

        [Header("Graph Bindings")]
        [SerializeField] private List<VariableOverride> variableOverrides = new List<VariableOverride>();
        [SerializeField] private List<FunctionBinding> functionBindings = new List<FunctionBinding>();
        [SerializeField] private List<ButtonBinding> buttonBindings = new List<ButtonBinding>();

        // Runtime state
        private DialogueContext context;
        private DialogueNode currentNode;
        private bool isRunning = false;
        private bool isWaitingForInput = false;

        public bool IsRunning => isRunning;
        public bool IsWaitingForInput => isWaitingForInput;

        /// <summary>
        /// Event called when dialogue starts.
        /// </summary>
        public System.Action OnDialogueStart;

        /// <summary>
        /// Event called when dialogue ends.
        /// </summary>
        public System.Action OnDialogueEnd;

        /// <summary>
        /// Event called when a text node is executed.
        /// </summary>
        public System.Action<string, string> OnTextDisplay; // speaker, text

        /// <summary>
        /// Event called when choices are presented.
        /// </summary>
        public System.Action<List<string>> OnChoicesPresented;

        private void OnValidate()
        {
            SynchronizeGraphBindings();
        }

        public bool SynchronizeGraphBindings()
        {
            if (dialogueGraph == null)
            {
                return false;
            }

            if (variableOverrides == null) variableOverrides = new List<VariableOverride>();
            if (functionBindings == null) functionBindings = new List<FunctionBinding>();
            if (buttonBindings == null) buttonBindings = new List<ButtonBinding>();

            List<string> variableNames = new List<string>();
            foreach (DialogueVariableDefinition variable in dialogueGraph.Variables)
            {
                if (variable != null)
                {
                    variableNames.Add(variable.variableName);
                }
            }

            bool changed = SynchronizeBindings(
                variableOverrides,
                variableNames,
                binding => binding.variableName,
                variableName => new VariableOverride { variableName = variableName });
            changed |= SynchronizeBindings(
                functionBindings,
                dialogueGraph.FunctionNames,
                binding => binding.functionName,
                functionName => new FunctionBinding { functionName = functionName });
            changed |= SynchronizeBindings(
                buttonBindings,
                dialogueGraph.ButtonInputNames,
                binding => binding.inputName,
                inputName => new ButtonBinding { inputName = inputName });

            foreach (FunctionBinding binding in functionBindings)
            {
                if (binding.function == null)
                {
                    binding.function = new UnityEvent();
                    changed = true;
                }
            }

            return changed;
        }

        private static bool SynchronizeBindings<T>(List<T> bindings, IEnumerable<string> declaredNames, Func<T, string> getName, Func<string, T> createBinding)
            where T : class
        {
            HashSet<string> validNames = new HashSet<string>();
            foreach (string name in declaredNames)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    validNames.Add(name);
                }
            }

            bool changed = false;
            HashSet<string> retainedNames = new HashSet<string>();
            for (int i = bindings.Count - 1; i >= 0; i--)
            {
                string name = bindings[i] == null ? null : getName(bindings[i]);
                if (string.IsNullOrWhiteSpace(name) || !validNames.Contains(name) || !retainedNames.Add(name))
                {
                    bindings.RemoveAt(i);
                    changed = true;
                }
            }

            foreach (string name in validNames)
            {
                if (!retainedNames.Contains(name))
                {
                    bindings.Add(createBinding(name));
                    changed = true;
                }
            }

            return changed;
        }

        private void Start()
        {
            if (playOnStart && dialogueGraph != null)
            {
                StartDialogue();
            }
        }

        /// <summary>
        /// Start executing the dialogue graph.
        /// </summary>
        public void StartDialogue()
        {
            if (dialogueGraph == null)
            {
                Debug.LogError("No dialogue graph assigned!");
                return;
            }

            if (isRunning)
            {
                Debug.LogWarning("Dialogue is already running!");
                return;
            }

            context = new DialogueContext();
            InitializeGraphVariables();
            isRunning = true;
            isWaitingForInput = false;

            DialogueNode startNode = dialogueGraph.GetStartNode();
            if (startNode == null)
            {
                Debug.LogError("No start node found in dialogue graph!");
                EndDialogue();
                return;
            }

            OnDialogueStart?.Invoke();
            ExecuteNode(startNode);
        }

        /// <summary>
        /// Stop the dialogue execution.
        /// </summary>
        public void StopDialogue()
        {
            isRunning = false;
            isWaitingForInput = false;
            currentNode = null;

            if (textEffectManager != null)
            {
                textEffectManager.StopCurrentEffect();
            }

            // Clear UI
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }
            if (speakerNameText != null)
            {
                speakerNameText.text = "";
            }
            if (choicePanel != null)
            {
                choicePanel.SetActive(false);
            }

            OnDialogueEnd?.Invoke();
        }

        /// <summary>
        /// Set the dialogue graph to run.
        /// </summary>
        public void SetDialogueGraph(DialogueGraph graph)
        {
            dialogueGraph = graph;
            SynchronizeGraphBindings();
        }

        private void InitializeGraphVariables()
        {
            SynchronizeGraphBindings();

            foreach (DialogueVariableDefinition definition in dialogueGraph.Variables)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.variableName))
                {
                    continue;
                }

                VariableOverride variableOverride = variableOverrides.Find(binding => binding.variableName == definition.variableName);
                object value = definition.GetDefaultValue();
                if (variableOverride != null && variableOverride.overrideGraphDefault)
                {
                    value = variableOverride.GetValue(definition.variableType);
                }

                context.SetVariable(definition.variableName, value);
            }
        }

        /// <summary>
        /// Execute a dialogue node.
        /// </summary>
        private void ExecuteNode(DialogueNode node)
        {
            if (node == null)
            {
                EndDialogue();
                return;
            }

            currentNode = node;
            context.SetCurrentNode(node.NodeGuid);

            // Execute the node's logic
            node.Execute(context);

            // Handle node type-specific logic
            HandleNodeExecution(node);
        }

        /// <summary>
        /// Handle node type-specific execution logic.
        /// </summary>
        private void HandleNodeExecution(DialogueNode node)
        {
            switch (node)
            {
                case TextNode textNode:
                    HandleTextNode(textNode);
                    break;

                case ChoiceNode choiceNode:
                    HandleChoiceNode(choiceNode);
                    break;

                case ConditionNode conditionNode:
                    HandleConditionNode(conditionNode);
                    break;

                case SetVariableNode setVarNode:
                    HandleSetVariableNode(setVarNode);
                    break;

                case WaitNode waitNode:
                    HandleWaitNode(waitNode);
                    break;

                case EventNode eventNode:
                    HandleEventNode(eventNode);
                    break;

                case EndNode endNode:
                    HandleEndNode(endNode);
                    break;

                default:
                    // Default behavior: move to next node
                    MoveToNextNode();
                    break;
            }
        }

        /// <summary>
        /// Handle text node execution.
        /// </summary>
        private void HandleTextNode(TextNode node)
        {
            string text = context.GetTemporaryData<string>("dialogueText");
            string speaker = context.GetTemporaryData<string>("speakerName");
            bool enableTypewriter = context.GetTemporaryData<bool>("enableTypewriterEffect");
            bool clickToProceed = context.GetTemporaryData<bool>("clickToProceed");

            // Update UI
            if (speakerNameText != null)
            {
                speakerNameText.text = speaker;
            }

            if (dialogueText != null)
            {
                if (enableTypewriter)
                {
                    // Use TypewriterEffect component
                    TypewriterEffect typewriter = dialogueText.GetComponent<TypewriterEffect>();
                    if (typewriter == null)
                    {
                        typewriter = dialogueText.gameObject.AddComponent<TypewriterEffect>();
                    }

                    typewriter.charactersPerSecond = node.typingSpeed;
                    typewriter.SetText(text);
                    StartCoroutine(WaitForTypewriterThenProceed(typewriter, clickToProceed, node.autoAdvanceDelay));
                }
                else
                {
                    // Show text immediately
                    dialogueText.text = text;
                    OnTextDisplay?.Invoke(speaker, text);

                    if (clickToProceed)
                    {
                        StartCoroutine(WaitForInputThenProceed());
                    }
                    else
                    {
                        StartCoroutine(WaitForTimeThenProceed(Mathf.Max(0f, node.autoAdvanceDelay)));
                    }
                }
            }
            else
            {
                MoveToNextNode();
            }
        }

        /// <summary>
        /// Handle choice node execution.
        /// </summary>
        private void HandleChoiceNode(ChoiceNode node)
        {
            List<ChoiceNode.ChoiceOption> choices = node.GetValidChoices(context);
            if (choices.Count == 0)
            {
                Debug.LogWarning($"Choice node '{node.NodeName}' has no valid options. Add choices or check their conditions.", this);
                EndDialogue();
                return;
            }

            List<string> choiceTexts = new List<string>();

            foreach (var choice in choices)
            {
                choiceTexts.Add(choice.choiceText);
            }

            OnChoicesPresented?.Invoke(choiceTexts);

            // Display choices in UI
            if (choicePanel != null && choiceButtonPrefab != null)
            {
                DisplayChoicesUI(node, choices);
            }
            else
            {
                List<string> missingReferences = new List<string>();
                if (choicePanel == null) missingReferences.Add("Choice Panel");
                if (choiceButtonPrefab == null) missingReferences.Add("Choice Button Prefab");
                Debug.LogWarning($"Choice UI is missing: {string.Join(", ", missingReferences)}. Assign these fields on DialogueGraphRunner.", this);
                // Default to first choice
                SelectChoice(node, 0);
            }
        }

        /// <summary>
        /// Display choices in the UI.
        /// </summary>
        private void DisplayChoicesUI(ChoiceNode node, List<ChoiceNode.ChoiceOption> choices)
        {
            // Clear existing buttons
            foreach (Transform child in choicePanel.transform)
            {
                Destroy(child.gameObject);
            }

            choicePanel.SetActive(true);

            for (int i = 0; i < choices.Count; i++)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
                Button button = buttonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = choices[i].choiceText;
                }

                if (button != null)
                {
                    int choiceIndex = i;
                    button.onClick.AddListener(() => SelectChoice(node, choiceIndex));
                }
            }

            isWaitingForInput = true;
        }

        /// <summary>
        /// Handle player choice selection.
        /// </summary>
        public void SelectChoice(ChoiceNode node, int choiceIndex)
        {
            if (choicePanel != null)
            {
                choicePanel.SetActive(false);
            }

            isWaitingForInput = false;

            string nextNodeGuid = node.GetNodeForChoice(choiceIndex);
            if (!string.IsNullOrEmpty(nextNodeGuid))
            {
                DialogueNode nextNode = dialogueGraph.GetNodeByGuid(nextNodeGuid);
                ExecuteNode(nextNode);
            }
            else
            {
                EndDialogue();
            }
        }

        /// <summary>
        /// Handle condition node execution.
        /// </summary>
        private void HandleConditionNode(ConditionNode node)
        {
            List<string> nextNodes = node.GetNextNodes(context);
            if (nextNodes.Count > 0)
            {
                DialogueNode nextNode = dialogueGraph.GetNodeByGuid(nextNodes[0]);
                ExecuteNode(nextNode);
            }
            else
            {
                EndDialogue();
            }
        }

        /// <summary>
        /// Handle set variable node execution.
        /// </summary>
        private void HandleSetVariableNode(SetVariableNode node)
        {
            MoveToNextNode();
        }

        /// <summary>
        /// Handle wait node execution.
        /// </summary>
        private void HandleWaitNode(WaitNode node)
        {
            WaitNode.WaitType waitType = context.GetTemporaryData<WaitNode.WaitType>("waitType");

            switch (waitType)
            {
                case WaitNode.WaitType.Time:
                    float duration = context.GetTemporaryData<float>("waitDuration");
                    StartCoroutine(WaitForTimeThenProceed(duration));
                    break;

                case WaitNode.WaitType.Input:
                    isWaitingForInput = true;
                    StartCoroutine(WaitForInputThenProceed());
                    break;

                case WaitNode.WaitType.Button:
                    string inputName = context.GetTemporaryData<string>("buttonInputName");
                    ButtonBinding buttonBinding = buttonBindings.Find(binding => binding.inputName == inputName);
                    if (buttonBinding != null && buttonBinding.button != null)
                    {
                        StartCoroutine(WaitForButtonThenProceed(buttonBinding.button));
                    }
                    else
                    {
                        Debug.LogWarning($"Wait node '{node.NodeName}' has no Button assigned for graph input '{inputName}'.", this);
                        EndDialogue();
                    }
                    break;
            }
        }

        /// <summary>
        /// Handle event node execution.
        /// </summary>
        private void HandleEventNode(EventNode node)
        {
            FunctionBinding binding = functionBindings.Find(item => item.functionName == node.eventName);
            if (binding == null || binding.function == null)
            {
                Debug.LogWarning($"Event node '{node.NodeName}' references an unbound graph function '{node.eventName}'.", this);
            }
            else
            {
                binding.function.Invoke();
            }

            MoveToNextNode();
        }

        /// <summary>
        /// Handle end node execution.
        /// </summary>
        private void HandleEndNode(EndNode node)
        {
            string endReason = context.GetTemporaryData<string>("endReason");
            Debug.Log($"Dialogue ended: {endReason}");
            EndDialogue();
        }

        /// <summary>
        /// Move to the next node in the sequence.
        /// </summary>
        private void MoveToNextNode()
        {
            if (currentNode == null)
            {
                EndDialogue();
                return;
            }

            List<string> nextNodeGuids = currentNode.GetNextNodes(context);
            if (nextNodeGuids.Count > 0)
            {
                DialogueNode nextNode = dialogueGraph.GetNodeByGuid(nextNodeGuids[0]);
                ExecuteNode(nextNode);
            }
            else
            {
                EndDialogue();
            }
        }

        /// <summary>
        /// Wait for typewriter effect to complete.
        /// </summary>
        private IEnumerator WaitForTypewriterThenProceed(TypewriterEffect typewriter, bool clickToProceed, float autoAdvanceDelay)
        {
            while (typewriter.IsTyping)
            {
                yield return null;
            }

            OnTextDisplay?.Invoke(context.GetTemporaryData<string>("speakerName"), dialogueText.text);

            if (clickToProceed)
            {
                isWaitingForInput = true;
                while (isWaitingForInput)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        isWaitingForInput = false;
                    }
                    yield return null;
                }
            }
            else if (autoAdvanceDelay > 0f)
            {
                yield return new WaitForSeconds(autoAdvanceDelay);
            }

            MoveToNextNode();
        }

        /// <summary>
        /// Wait for time then proceed.
        /// </summary>
        private IEnumerator WaitForTimeThenProceed(float duration)
        {
            yield return new WaitForSeconds(duration);
            MoveToNextNode();
        }

        /// <summary>
        /// Wait for input then proceed.
        /// </summary>
        private IEnumerator WaitForInputThenProceed()
        {
            isWaitingForInput = true;
            while (isWaitingForInput)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isWaitingForInput = false;
                }
                yield return null;
            }
            MoveToNextNode();
        }

        /// <summary>
        /// Wait for button click then proceed.
        /// </summary>
        private IEnumerator WaitForButtonThenProceed(Button button)
        {
            isWaitingForInput = true;
            UnityAction onButtonClicked = () => isWaitingForInput = false;
            button.onClick.AddListener(onButtonClicked);

            while (isWaitingForInput && isRunning)
            {
                yield return null;
            }

            button.onClick.RemoveListener(onButtonClicked);
            if (isRunning)
            {
                MoveToNextNode();
            }
        }

        /// <summary>
        /// End the dialogue.
        /// </summary>
        private void EndDialogue()
        {
            isRunning = false;
            isWaitingForInput = false;
            currentNode = null;
            OnDialogueEnd?.Invoke();
        }

        /// <summary>
        /// Skip current dialogue step.
        /// </summary>
        public void SkipCurrentStep()
        {
            if (isWaitingForInput)
            {
                isWaitingForInput = false;
            }
        }

        /// <summary>
        /// Get a variable value from the dialogue context.
        /// </summary>
        public T GetVariable<T>(string key, T defaultValue = default(T))
        {
            if (context != null)
            {
                return context.GetVariable<T>(key, defaultValue);
            }
            return defaultValue;
        }

        /// <summary>
        /// Set a variable value in the dialogue context.
        /// </summary>
        public void SetVariable(string key, object value)
        {
            if (context != null)
            {
                context.SetVariable(key, value);
            }
        }
    }
}