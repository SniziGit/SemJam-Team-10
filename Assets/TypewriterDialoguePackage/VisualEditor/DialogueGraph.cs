using System;
using System.Collections.Generic;
using UnityEngine;

namespace TypewriterDialoguePackage.VisualEditor
{
    [Serializable]
    public class DialogueVariableDefinition
    {
        public string variableName = "";
        public SetVariableNode.VariableType variableType = SetVariableNode.VariableType.String;
        public string stringDefaultValue = "";
        public int integerDefaultValue;
        public float floatDefaultValue;
        public bool booleanDefaultValue;

        public object GetDefaultValue()
        {
            switch (variableType)
            {
                case SetVariableNode.VariableType.Integer:
                    return integerDefaultValue;
                case SetVariableNode.VariableType.Float:
                    return floatDefaultValue;
                case SetVariableNode.VariableType.Boolean:
                    return booleanDefaultValue;
                default:
                    return stringDefaultValue;
            }
        }
    }

    /// <summary>
    /// Container for a complete dialogue graph with all nodes and connections.
    /// This ScriptableObject can be saved as an asset and referenced by dialogue systems.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDialogueGraph", menuName = "Typewriter Dialogue/Dialogue Graph")]
    public class DialogueGraph : ScriptableObject
    {
        [SerializeReference] private List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] private string startNodeGuid;
        [Header("Runtime Declarations")]
        [SerializeField] private List<DialogueVariableDefinition> variables = new List<DialogueVariableDefinition>();
        [SerializeField] private List<string> functionNames = new List<string>();
        [SerializeField] private List<string> buttonInputNames = new List<string>();

        public List<DialogueNode> Nodes => nodes;
        public string StartNodeGuid => startNodeGuid;
        public List<DialogueVariableDefinition> Variables => variables;
        public List<string> FunctionNames => functionNames;
        public List<string> ButtonInputNames => buttonInputNames;

        public void SynchronizeRuntimeDeclarations()
        {
            if (nodes == null) nodes = new List<DialogueNode>();
            if (variables == null) variables = new List<DialogueVariableDefinition>();

            List<string> variableNames = new List<string>();
            List<string> discoveredFunctionNames = new List<string>();
            List<string> discoveredButtonNames = new List<string>();
            Dictionary<string, SetVariableNode.VariableType> variableTypes = new Dictionary<string, SetVariableNode.VariableType>();

            foreach (DialogueNode node in nodes)
            {
                if (node == null) continue;

                if (node is SetVariableNode setVariableNode)
                {
                    AddUniqueName(variableNames, setVariableNode.variableName);
                    if (!string.IsNullOrWhiteSpace(setVariableNode.variableName) && !variableTypes.ContainsKey(setVariableNode.variableName))
                    {
                        variableTypes.Add(setVariableNode.variableName, setVariableNode.variableType);
                    }
                }
                else if (node is ConditionNode conditionNode)
                {
                    AddUniqueName(variableNames, conditionNode.variableName);
                }
                else if (node is ChoiceNode choiceNode)
                {
                    if (choiceNode.choices == null) continue;
                    foreach (ChoiceNode.ChoiceOption choice in choiceNode.choices)
                    {
                        if (choice != null && choice.requireCondition)
                        {
                            AddUniqueName(variableNames, choice.conditionVariable);
                        }
                    }
                }

                if (node is EventNode eventNode)
                {
                    AddUniqueName(discoveredFunctionNames, eventNode.eventName);
                }

                if (node is WaitNode waitNode && waitNode.waitType == WaitNode.WaitType.Button)
                {
                    AddUniqueName(discoveredButtonNames, waitNode.buttonInputName);
                }
            }

            List<DialogueVariableDefinition> synchronizedVariables = new List<DialogueVariableDefinition>();
            foreach (string variableName in variableNames)
            {
                DialogueVariableDefinition definition = variables.Find(variable => variable != null && variable.variableName == variableName);
                if (definition == null)
                {
                    definition = new DialogueVariableDefinition { variableName = variableName };
                    if (variableTypes.TryGetValue(variableName, out SetVariableNode.VariableType variableType))
                    {
                        definition.variableType = variableType;
                    }
                }
                synchronizedVariables.Add(definition);
            }

            variables = synchronizedVariables;
            functionNames = discoveredFunctionNames;
            buttonInputNames = discoveredButtonNames;
        }

        private static void AddUniqueName(List<string> names, string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && !names.Contains(name))
            {
                names.Add(name);
            }
        }

        /// <summary>
        /// Add a node to the graph.
        /// </summary>
        public void AddNode(DialogueNode node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
            }
        }

        /// <summary>
        /// Remove a node from the graph.
        /// </summary>
        public void RemoveNode(DialogueNode node)
        {

            if (nodes.Contains(node))
            {
                // Remove connections to this node from other nodes
                foreach (DialogueNode otherNode in nodes)
                {
                    otherNode.RemoveConnection(node.NodeGuid);
                }
                nodes.Remove(node);
            }
        }

        /// <summary>
        /// Get a node by its GUID.
        /// </summary>
        public DialogueNode GetNodeByGuid(string guid)
        {
            return nodes.Find(n => n.NodeGuid == guid);
        }

        /// <summary>
        /// Set the starting node for this dialogue.
        /// </summary>
        public void SetStartNode(string nodeGuid)
        {
            if (string.IsNullOrEmpty(nodeGuid))
            {
                startNodeGuid = string.Empty;
                return;
            }

            if (GetNodeByGuid(nodeGuid) != null)
            {
                startNodeGuid = nodeGuid;
            }
        }

        /// <summary>
        /// Get the starting node.
        /// </summary>
        public DialogueNode GetStartNode()
        {
            return GetNodeByGuid(startNodeGuid);
        }

        /// <summary>
        /// Clear all nodes from the graph.
        /// </summary>
        public void ClearGraph()
        {
            nodes.Clear();
            startNodeGuid = string.Empty;
        }

        /// <summary>
        /// Validate the graph and return any issues.
        /// </summary>
        public List<string> ValidateGraph()
        {
            List<string> issues = new List<string>();

            // Check if start node is set
            if (string.IsNullOrEmpty(startNodeGuid))
            {
                issues.Add("No start node set");
            }
            else if (GetNodeByGuid(startNodeGuid) == null)
            {
                issues.Add("Start node GUID is invalid");
            }

            // Check for orphaned nodes
            foreach (DialogueNode node in nodes)
            {
                bool hasIncomingConnection = false;
                foreach (DialogueNode otherNode in nodes)
                {
                    if (otherNode.ConnectedNodeGuids.Contains(node.NodeGuid))
                    {
                        hasIncomingConnection = true;
                        break;
                    }
                }

                if (!hasIncomingConnection && node.NodeGuid != startNodeGuid)
                {
                    issues.Add($"Node '{node.NodeName}' has no incoming connections (orphaned)");
                }

                // Check for invalid connections
                foreach (string connectedGuid in node.ConnectedNodeGuids)
                {
                    if (GetNodeByGuid(connectedGuid) == null)
                    {
                        issues.Add($"Node '{node.NodeName}' has invalid connection to {connectedGuid}");
                    }
                }
            }

            return issues;
        }

        /// <summary>
        /// Create a copy of this graph.
        /// </summary>
        public DialogueGraph Clone()
        {
            DialogueGraph clone = CreateInstance<DialogueGraph>();
            clone.name = name + " (Copy)";

            // This is a simplified clone - in a full implementation,
            // you'd need to deep copy each node properly
            foreach (DialogueNode node in nodes)
            {
                // For now, we'll just add references - a proper implementation
                // would create new instances of each node type
                clone.nodes.Add(node);
            }

            clone.startNodeGuid = startNodeGuid;
            return clone;
        }

        private void OnEnable()
        {
            // Ensure the list is initialized
            if (nodes == null)
            {
                nodes = new List<DialogueNode>();
            }
            if (variables == null)
            {
                variables = new List<DialogueVariableDefinition>();
            }
            if (functionNames == null)
            {
                functionNames = new List<string>();
            }
            if (buttonInputNames == null)
            {
                buttonInputNames = new List<string>();
            }
            SynchronizeRuntimeDeclarations();
        }

        private void OnValidate()
        {
            SynchronizeRuntimeDeclarations();
        }
    }
}