using System;
using System.Collections.Generic;
using UnityEngine;

namespace TypewriterDialoguePackage.VisualEditor
{
    /// <summary>
    /// Base class for all dialogue nodes in the visual editor.
    /// Provides common functionality for node identification, connections, and execution.
    /// </summary>
    [Serializable]
    public abstract class DialogueNode
    {
        [SerializeField] private string nodeGuid;
        [SerializeField] private string nodeName;
        [SerializeField] private Vector2 nodePosition;
        [SerializeField] private List<string> connectedNodeGuids;

        public string NodeGuid => nodeGuid;
        public string NodeName => nodeName;
        public Vector2 NodePosition => nodePosition;
        public List<string> ConnectedNodeGuids => connectedNodeGuids;
        public List<string> Connections => connectedNodeGuids;

        public DialogueNode()
        {
            nodeGuid = Guid.NewGuid().ToString();
            nodeName = "New Node";
            nodePosition = Vector2.zero;
            connectedNodeGuids = new List<string>();
        }

        /// <summary>
        /// Set the position of this node in the visual editor.
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            nodePosition = position;
        }

        /// <summary>
        /// Set the name of this node.
        /// </summary>
        public void SetName(string name)
        {
            nodeName = name;
        }

        /// <summary>
        /// Get the name of this node (protected setter for derived classes).
        /// </summary>
        protected string Name { get => nodeName; set => nodeName = value; }

        /// <summary>
        /// Add a connection to another node.
        /// </summary>
        public void AddConnection(string nodeGuid)
        {
            if (!connectedNodeGuids.Contains(nodeGuid))
            {
                connectedNodeGuids.Add(nodeGuid);
            }
        }

        /// <summary>
        /// Remove a connection to another node.
        /// </summary>
        public void RemoveConnection(string nodeGuid)
        {
            connectedNodeGuids.Remove(nodeGuid);
        }

        /// <summary>
        /// Clear all connections from this node.
        /// </summary>
        public void ClearConnections()
        {
            connectedNodeGuids.Clear();
        }

        /// <summary>
        /// Get the type of this node for display purposes.
        /// </summary>
        public abstract string GetNodeType();

        /// <summary>
        /// Get the color for this node type in the visual editor.
        /// </summary>
        public abstract Color GetNodeColor();

        /// <summary>
        /// Execute this node's logic during dialogue runtime.
        /// </summary>
        public abstract void Execute(DialogueContext context);

        /// <summary>
        /// Get the next node(s) to execute based on this node's logic.
        /// </summary>
        public abstract List<string> GetNextNodes(DialogueContext context);
    }

    /// <summary>
    /// Context object passed between nodes during dialogue execution.
    /// Contains state, variables, and runtime information.
    /// </summary>
    public class DialogueContext
    {
        private Dictionary<string, object> variables;
        private Dictionary<string, object> temporaryData;
        private string currentNodeGuid;
        private bool isWaitingForInput;

        public Dictionary<string, object> Variables => variables;
        public Dictionary<string, object> TemporaryData => temporaryData;
        public string CurrentNodeGuid => currentNodeGuid;
        public bool IsWaitingForInput => isWaitingForInput;

        public DialogueContext()
        {
            variables = new Dictionary<string, object>();
            temporaryData = new Dictionary<string, object>();
            currentNodeGuid = string.Empty;
            isWaitingForInput = false;
        }

        /// <summary>
        /// Set a variable value.
        /// </summary>
        public void SetVariable(string key, object value)
        {
            if (variables.ContainsKey(key))
            {
                variables[key] = value;
            }
            else
            {
                variables.Add(key, value);
            }
        }

        /// <summary>
        /// Get a variable value.
        /// </summary>
        public T GetVariable<T>(string key, T defaultValue = default(T))
        {
            if (variables.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Check if a variable exists.
        /// </summary>
        public bool HasVariable(string key)
        {
            return variables.ContainsKey(key);
        }

        /// <summary>
        /// Set temporary data for the current node execution.
        /// </summary>
        public void SetTemporaryData(string key, object value)
        {
            if (temporaryData.ContainsKey(key))
            {
                temporaryData[key] = value;
            }
            else
            {
                temporaryData.Add(key, value);
            }
        }

        /// <summary>
        /// Get temporary data.
        /// </summary>
        public T GetTemporaryData<T>(string key, T defaultValue = default(T))
        {
            if (temporaryData.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Clear all temporary data.
        /// </summary>
        public void ClearTemporaryData()
        {
            temporaryData.Clear();
        }

        /// <summary>
        /// Set the current node being executed.
        /// </summary>
        public void SetCurrentNode(string nodeGuid)
        {
            currentNodeGuid = nodeGuid;
        }

        /// <summary>
        /// Set whether the dialogue is waiting for user input.
        /// </summary>
        public void SetWaitingForInput(bool waiting)
        {
            isWaitingForInput = waiting;
        }

        /// <summary>
        /// Create a copy of this context (for branching dialogue).
        /// </summary>
        public DialogueContext Clone()
        {
            DialogueContext clone = new DialogueContext();
            foreach (var kvp in variables)
            {
                clone.variables[kvp.Key] = kvp.Value;
            }
            clone.currentNodeGuid = currentNodeGuid;
            clone.isWaitingForInput = isWaitingForInput;
            return clone;
        }
    }
}