using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using UnityEngine.UI;

namespace TypewriterDialoguePackage.VisualEditor
{
    /// <summary>
    /// Node that displays text to the player.
    /// </summary>
    [System.Serializable]
    public class TextNode : DialogueNode
    {
        [TextArea(3, 10)]
        public string dialogueText = "Enter dialogue text here...";
        public string speakerName = "";
        public float typingSpeed = 60f;
        [FormerlySerializedAs("waitForCompletion")]
        public bool clickToProceed = true;
        [Min(0f)]
        public float autoAdvanceDelay = 1.5f;
        public bool enableTypewriterEffect = true;

        public TextNode() : base()
        {
            Name = "Text";
        }

        public override string GetNodeType()
        {
            return "Text";
        }

        public override Color GetNodeColor()
        {
            return new Color(0.4f, 0.6f, 1f); // Light blue
        }

        public override void Execute(DialogueContext context)
        {
            context.SetTemporaryData("dialogueText", dialogueText);
            context.SetTemporaryData("speakerName", speakerName);
            context.SetTemporaryData("typingSpeed", typingSpeed);
            context.SetTemporaryData("clickToProceed", clickToProceed);
            context.SetTemporaryData("enableTypewriterEffect", enableTypewriterEffect);
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            return new List<string>(ConnectedNodeGuids);
        }
    }

    /// <summary>
    /// Node that presents multiple choices to the player.
    /// </summary>
    [System.Serializable]
    public class ChoiceNode : DialogueNode
    {
        [System.Serializable]
        public class ChoiceOption
        {
            public string choiceText = "Choice text";
            public string conditionVariable = "";
            public string conditionValue = "";
            public bool requireCondition = false;
            public string connectedNodeGuid = ""; // Each choice has its own connection
        }

        public List<ChoiceOption> choices = new List<ChoiceOption>
        {

        };

        public ChoiceNode() : base()
        {
            Name = "Choice";
        }

        public override string GetNodeType()
        {
            return "Choice";
        }

        public override Color GetNodeColor()
        {
            return new Color(0.4f, 1f, 0.6f); // Light green
        }

        public override void Execute(DialogueContext context)
        {
            context.SetTemporaryData("choices", choices);
            context.SetWaitingForInput(true);
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            // Return all possible connections - the actual choice is handled externally
            List<string> nextNodes = new List<string>();
            foreach (var choice in choices)
            {
                if (!string.IsNullOrEmpty(choice.connectedNodeGuid))
                {
                    nextNodes.Add(choice.connectedNodeGuid);
                }
            }
            return nextNodes;
        }

        /// <summary>
        /// Get the valid choices based on current conditions.
        /// </summary>
        public List<ChoiceOption> GetValidChoices(DialogueContext context)
        {
            List<ChoiceOption> validChoices = new List<ChoiceOption>();

            for (int i = 0; i < choices.Count; i++)
            {
                ChoiceOption choice = choices[i];

                if (!choice.requireCondition)
                {
                    validChoices.Add(choice);
                }
                else if (context.HasVariable(choice.conditionVariable))
                {
                    string currentValue = context.GetVariable<string>(choice.conditionVariable);
                    if (currentValue == choice.conditionValue)
                    {
                        validChoices.Add(choice);
                    }
                }
            }

            return validChoices;
        }

        /// <summary>
        /// Get the node GUID for a specific choice index.
        /// </summary>
        public string GetNodeForChoice(int choiceIndex)
        {
            if (choiceIndex >= 0 && choiceIndex < choices.Count)
            {
                return choices[choiceIndex].connectedNodeGuid;
            }
            return null;
        }

        /// <summary>
        /// Set the node GUID for a specific choice index.
        /// </summary>
        public void SetNodeForChoice(int choiceIndex, string nodeGuid)
        {
            if (choiceIndex >= 0 && choiceIndex < choices.Count)
            {
                choices[choiceIndex].connectedNodeGuid = nodeGuid;
            }
        }
    }

    /// <summary>
    /// Node that conditionally branches based on variable values.
    /// </summary>
    [System.Serializable]
    public class ConditionNode : DialogueNode
    {
        public enum ConditionType
        {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterOrEqual,
            LessOrEqual,
            Contains,
            HasVariable
        }

        public string variableName = "";
        public ConditionType condition = ConditionType.Equals;
        public string compareValue = "";
        public string trueNodeGuid = "";
        public string falseNodeGuid = "";

        public ConditionNode() : base()
        {
            Name = "Condition";
        }

        public override string GetNodeType()
        {
            return "Condition";
        }

        public override Color GetNodeColor()
        {
            return new Color(1f, 0.8f, 0.4f); // Orange
        }

        public override void Execute(DialogueContext context)
        {
            // Condition logic is handled in GetNextNodes
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            List<string> nextNodes = new List<string>();

            bool conditionResult = EvaluateCondition(context);

            if (conditionResult && !string.IsNullOrEmpty(trueNodeGuid))
            {
                nextNodes.Add(trueNodeGuid);
            }
            else if (!conditionResult && !string.IsNullOrEmpty(falseNodeGuid))
            {
                nextNodes.Add(falseNodeGuid);
            }

            return nextNodes;
        }

        private bool EvaluateCondition(DialogueContext context)
        {
            if (!context.HasVariable(variableName))
            {
                return condition == ConditionType.HasVariable ? false : false;
            }

            object variableValue = context.GetVariable<object>(variableName);
            string stringValue = variableValue?.ToString() ?? "";

            switch (condition)
            {
                case ConditionType.Equals:
                    return stringValue == compareValue;

                case ConditionType.NotEquals:
                    return stringValue != compareValue;

                case ConditionType.GreaterThan:
                    if (float.TryParse(stringValue, out float floatVal) && float.TryParse(compareValue, out float compareFloat))
                    {
                        return floatVal > compareFloat;
                    }
                    return false;

                case ConditionType.LessThan:
                    if (float.TryParse(stringValue, out float floatVal2) && float.TryParse(compareValue, out float compareFloat2))
                    {
                        return floatVal2 < compareFloat2;
                    }
                    return false;

                case ConditionType.GreaterOrEqual:
                    if (float.TryParse(stringValue, out float floatVal3) && float.TryParse(compareValue, out float compareFloat3))
                    {
                        return floatVal3 >= compareFloat3;
                    }
                    return false;

                case ConditionType.LessOrEqual:
                    if (float.TryParse(stringValue, out float floatVal4) && float.TryParse(compareValue, out float compareFloat4))
                    {
                        return floatVal4 <= compareFloat4;
                    }
                    return false;

                case ConditionType.Contains:
                    return stringValue.Contains(compareValue);

                case ConditionType.HasVariable:
                    return true;

                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Node that sets a variable value.
    /// </summary>
    [System.Serializable]
    public class SetVariableNode : DialogueNode
    {
        public enum VariableType
        {
            String,
            Integer,
            Float,
            Boolean
        }

        public string variableName = "";
        public VariableType variableType = VariableType.String;
        public string stringValue = "";
        public int intValue = 0;
        public float floatValue = 0f;
        public bool boolValue = false;

        public SetVariableNode() : base()
        {
            Name = "Set Variable";
        }

        public override string GetNodeType()
        {
            return "Set Variable";
        }

        public override Color GetNodeColor()
        {
            return new Color(1f, 0.4f, 0.4f); // Red
        }

        public override void Execute(DialogueContext context)
        {
            object value = GetTypedValue();
            context.SetVariable(variableName, value);
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            return new List<string>(ConnectedNodeGuids);
        }

        private object GetTypedValue()
        {
            switch (variableType)
            {
                case VariableType.String:
                    return stringValue;
                case VariableType.Integer:
                    return intValue;
                case VariableType.Float:
                    return floatValue;
                case VariableType.Boolean:
                    return boolValue;
                default:
                    return stringValue;
            }
        }
    }

    /// <summary>
    /// Node that waits for a specified time or user input.
    /// </summary>
    [System.Serializable]
    public class WaitNode : DialogueNode
    {
        public enum WaitType
        {
            Time,
            Input,
            Button
        }

        public WaitType waitType = WaitType.Time;
        public float waitDuration = 1f;
        public string buttonInputName = "";

        public WaitNode() : base()
        {
            Name = "Wait";
        }

        public override string GetNodeType()
        {
            return "Wait";
        }

        public override Color GetNodeColor()
        {
            return new Color(0.8f, 0.4f, 1f); // Purple
        }

        public override void Execute(DialogueContext context)
        {
            context.SetTemporaryData("waitType", waitType);
            context.SetTemporaryData("waitDuration", waitDuration);
            context.SetTemporaryData("buttonInputName", buttonInputName);

            if (waitType == WaitType.Input)
            {
                context.SetWaitingForInput(true);
            }
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            return new List<string>(ConnectedNodeGuids);
        }
    }

    /// <summary>
    /// Node that triggers a custom event or UnityEvent.
    /// </summary>
    [System.Serializable]
    public class EventNode : DialogueNode
    {
        public string eventName = "";
        public UnityEngine.Events.UnityEvent customEvent;

        public EventNode() : base()
        {
            Name = "Event";
        }

        public override string GetNodeType()
        {
            return "Event";
        }

        public override Color GetNodeColor()
        {
            return new Color(1f, 1f, 0.4f); // Yellow
        }

        public override void Execute(DialogueContext context)
        {
            context.SetTemporaryData("eventName", eventName);
            context.SetTemporaryData("customEvent", customEvent);

            if (customEvent != null)
            {
                customEvent.Invoke();
            }
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            return new List<string>(ConnectedNodeGuids);
        }
    }

    /// <summary>
    /// Starting node for a dialogue graph.
    /// </summary>
    [System.Serializable]
    public class StartNode : DialogueNode
    {
        public StartNode() : base()
        {
            Name = "Start";
        }

        public override string GetNodeType()
        {
            return "Start";
        }

        public override Color GetNodeColor()
        {
            return new Color(0.4f, 1f, 0.4f); // Green
        }

        public override void Execute(DialogueContext context)
        {
            // Start node doesn't execute anything special
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            return new List<string>(ConnectedNodeGuids);
        }
    }

    /// <summary>
    /// Ending node for a dialogue graph.
    /// </summary>
    [System.Serializable]
    public class EndNode : DialogueNode
    {
        public string endReason = "Dialogue Complete";

        public EndNode() : base()
        {
            Name = "End";
        }

        public override string GetNodeType()
        {
            return "End";
        }

        public override Color GetNodeColor()
        {
            return new Color(1f, 0.4f, 0.4f); // Red
        }

        public override void Execute(DialogueContext context)
        {
            context.SetTemporaryData("endReason", endReason);
        }

        public override List<string> GetNextNodes(DialogueContext context)
        {
            return new List<string>(); // End node has no connections
        }
    }
}