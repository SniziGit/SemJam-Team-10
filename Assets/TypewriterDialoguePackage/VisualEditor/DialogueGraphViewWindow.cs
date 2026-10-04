#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TypewriterDialoguePackage.VisualEditor
{
    /// <summary>
    /// Main window for the dialogue graph editor using Unity's GraphView.
    /// </summary>
    public class DialogueGraphViewWindow : EditorWindow
    {
        private DialogueGraphView graphView;
        private DialogueGraph currentGraph;
        public Toolbar toolbar;

        [MenuItem("Window/Typewriter Dialogue/Visual Editor")]
        public static void ShowWindow()
        {
            DialogueGraphViewWindow window = GetWindow<DialogueGraphViewWindow>("Dialogue Graph Editor");
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            ConstructToolbar();
        }

        private void ConstructGraphView()
        {
            graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph"
            };

            graphView.style.flexGrow = 1;
            rootVisualElement.Add(graphView);
        }

        private void ConstructToolbar()
        {
            toolbar = new Toolbar();

            // Graph selection label
            var graphLabel = new Label("Graph:");
            toolbar.Add(graphLabel);

            // Graph object field
            var graphField = new ObjectField
            {
                objectType = typeof(DialogueGraph),
                value = currentGraph,
                allowSceneObjects = false
            };
            graphField.RegisterValueChangedCallback(evt =>
            {
                currentGraph = evt.newValue as DialogueGraph;
                if (currentGraph != null)
                {
                    graphView.LoadGraph(currentGraph);
                }
            });
            toolbar.Add(graphField);

            var spacer1 = new VisualElement();
            spacer1.style.flexGrow = 1;
            toolbar.Add(spacer1);

            // Node creation buttons
            toolbar.Add(new Button(() => graphView.AddNode(new TextNode())) { text = "+ Text" });
            toolbar.Add(new Button(() => graphView.AddNode(new ChoiceNode())) { text = "+ Choice" });
            toolbar.Add(new Button(() => graphView.AddNode(new ConditionNode())) { text = "+ Condition" });
            toolbar.Add(new Button(() => graphView.AddNode(new SetVariableNode())) { text = "+ Set Var" });
            toolbar.Add(new Button(() => graphView.AddNode(new WaitNode())) { text = "+ Wait" });
            toolbar.Add(new Button(() => graphView.AddNode(new EventNode())) { text = "+ Event" });
            toolbar.Add(new Button(() => graphView.AddNode(new StartNode())) { text = "+ Start" });
            toolbar.Add(new Button(() => graphView.AddNode(new EndNode())) { text = "+ End" });
            toolbar.Add(new Button(() => graphView.AddStickyNote(new Vector2(300, 300))) { text = "+ Note" });

            var spacer2 = new VisualElement();
            spacer2.style.flexGrow = 1;
            toolbar.Add(spacer2);

            // Save button
            var saveButton = new Button(() =>
            {
                if (currentGraph != null)
                {
                    graphView.SaveGraph(currentGraph);
                    EditorUtility.SetDirty(currentGraph);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Graph saved!");
                }
            })
            {
                text = "Save Graph"
            };
            toolbar.Add(saveButton);

            var spacer3 = new VisualElement();
            spacer3.style.flexGrow = 1;
            toolbar.Add(spacer3);

            // Clear button
            var clearButton = new Button(() =>
            {
                graphView.ClearGraph();
                currentGraph = null;
            })
            {
                text = "Clear"
            };
            toolbar.Add(clearButton);

            rootVisualElement.Add(toolbar);
        }

        private void OnDisable()
        {
            if (graphView != null)
            {
                rootVisualElement.Remove(graphView);
            }
        }

        public void SetCurrentGraph(DialogueGraph graph)
        {
            currentGraph = graph;
            if (graphView != null)
            {
                graphView.LoadGraph(graph);
            }
        }
    }

    /// <summary>
    /// The main GraphView for the dialogue system.
    /// </summary>
    public class DialogueGraphView : GraphView
    {
        private DialogueGraphViewWindow window;
        private Dictionary<string, DialogueNodeView> nodeViews = new Dictionary<string, DialogueNodeView>();

        public DialogueGraphView(DialogueGraphViewWindow window)
        {
            this.window = window;
            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Add manipulators
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            // Grid background
            var grid = new GridBackground();
            Insert(0, grid);
            grid.style.flexGrow = 1;

            // Load styles
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/TypewriterDialoguePackage/Resources/DialogueGraphStyles.uss"
            );
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            // Node creation
            nodeCreationRequest = context =>
            {
                // Show a simple menu for node creation
                var menu = new GenericMenu();
                Vector2 creationPosition = ScreenToGraphPosition(context.screenMousePosition);
                menu.AddItem(new GUIContent("Text Node"), false, () => AddNodeAtPosition(new TextNode(), creationPosition));
                menu.AddItem(new GUIContent("Choice Node"), false, () => AddNodeAtPosition(new ChoiceNode(), creationPosition));
                menu.AddItem(new GUIContent("Condition Node"), false, () => AddNodeAtPosition(new ConditionNode(), creationPosition));
                menu.AddItem(new GUIContent("Set Variable Node"), false, () => AddNodeAtPosition(new SetVariableNode(), creationPosition));
                menu.AddItem(new GUIContent("Wait Node"), false, () => AddNodeAtPosition(new WaitNode(), creationPosition));
                menu.AddItem(new GUIContent("Event Node"), false, () => AddNodeAtPosition(new EventNode(), creationPosition));
                menu.AddItem(new GUIContent("Start Node"), false, () => AddNodeAtPosition(new StartNode(), creationPosition));
                menu.AddItem(new GUIContent("End Node"), false, () => AddNodeAtPosition(new EndNode(), creationPosition));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Sticky Note"), false, () => AddStickyNote(creationPosition));
                menu.ShowAsContext();
            };
        }

        public void LoadGraph(DialogueGraph graph)
        {
            ClearGraph();

            if (graph == null || graph.Nodes == null) return;

            foreach (DialogueNode node in graph.Nodes)
            {
                CreateNodeView(node);
            }

            // Load connections
            foreach (DialogueNode node in graph.Nodes)
            {
                DialogueNodeView fromNodeView = GetNodeView(node.NodeGuid);
                if (fromNodeView == null) continue;

                // Handle ChoiceNode connections (each choice has its own connection)
                if (node is ChoiceNode choiceNode)
                {
                    for (int i = 0; i < choiceNode.choices.Count; i++)
                    {
                        string connectionGuid = choiceNode.choices[i].connectedNodeGuid;
                        if (!string.IsNullOrEmpty(connectionGuid))
                        {
                            DialogueNodeView toNodeView = GetNodeView(connectionGuid);
                            if (toNodeView != null && i < fromNodeView.choicePorts.Count)
                            {
                                Port fromPort = fromNodeView.choicePorts[i];
                                Port toPort = toNodeView.inputPort;

                                if (fromPort != null && toPort != null)
                                {
                                    AddElement(fromPort.ConnectTo(toPort));
                                }
                            }
                        }
                    }
                }
                // Handle regular node connections
                else if (node.Connections != null)
                {
                    foreach (string connectionGuid in node.Connections)
                    {
                        DialogueNodeView toNodeView = GetNodeView(connectionGuid);
                        if (toNodeView != null)
                        {
                            Port fromPort = fromNodeView.outputPort;
                            Port toPort = toNodeView.inputPort;

                            if (fromPort != null && toPort != null)
                            {
                                AddElement(fromPort.ConnectTo(toPort));
                            }
                        }
                    }
                }
            }
        }

        public void SaveGraph(DialogueGraph graph)
        {
            if (graph == null) return;

            List<DialogueNodeView> visibleNodeViews = nodes.OfType<DialogueNodeView>().ToList();
            nodeViews = visibleNodeViews.ToDictionary(nodeView => nodeView.Node.NodeGuid);
            graph.Nodes.RemoveAll(node => !nodeViews.ContainsKey(node.NodeGuid));

            // Update nodes instead of clearing
            foreach (DialogueNodeView nodeView in visibleNodeViews)
            {
                DialogueNode node = nodeView.Node;

                if (graph.GetNodeByGuid(node.NodeGuid) == null)
                {
                    graph.AddNode(node);
                }

                node.SetPosition(nodeView.GetPosition().position);

                if (node is ChoiceNode choiceNode)
                {
                    for (int i = 0; i < choiceNode.choices.Count; i++)
                    {
                        choiceNode.choices[i].connectedNodeGuid = string.Empty;
                        if (i < nodeView.choicePorts.Count)
                        {
                            Port choicePort = nodeView.choicePorts[i];
                            foreach (Edge edge in choicePort.connections)
                            {
                                if (edge.input.node is DialogueNodeView connectedNodeView)
                                {
                                    choiceNode.choices[i].connectedNodeGuid = connectedNodeView.Node.NodeGuid;
                                }
                            }
                        }
                    }
                }
                else if (nodeView.outputPort != null)
                {
                    node.ClearConnections();
                    foreach (Edge edge in nodeView.outputPort.connections)
                    {
                        if (edge.input.node is DialogueNodeView connectedNodeView)
                        {
                            node.AddConnection(connectedNodeView.Node.NodeGuid);
                        }
                    }
                }
            }

            // Set start node
            DialogueNodeView startNodeView = nodeViews.Values.FirstOrDefault(nv => nv.Node is StartNode);
            graph.SetStartNode(startNodeView != null ? startNodeView.Node.NodeGuid : string.Empty);
            graph.SynchronizeRuntimeDeclarations();

            // Persist changes
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            foreach (Port port in ports)
            {
                if (startPort == port) continue;
                if (startPort.node == port.node) continue;
                if (startPort.direction == port.direction) continue;

                compatiblePorts.Add(port);
            }

            return compatiblePorts;
        }

        public void ClearGraph()
        {
            foreach (var kvp in nodeViews)
            {
                RemoveElement(kvp.Value);
            }
            nodeViews.Clear();
        }

        public void AddNode(TextNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        public void AddNode(ChoiceNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        public void AddNode(ConditionNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        public void AddNode(SetVariableNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        public void AddNode(WaitNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        public void AddNode(EventNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        public void AddNode(StartNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        public void AddNode(EndNode node)
        {
            AddNodeAtPosition(node, GetGraphViewCenter());
        }

        private void AddNodeAtPosition(DialogueNode node, Vector2 position)
        {
            node.SetPosition(position - new Vector2(100f, 75f));
            CreateNodeView(node);
        }

        private Vector2 GetGraphViewCenter()
        {
            Vector2 viewportCenter = new Vector2(layout.width * 0.5f, layout.height * 0.5f);
            Vector2 pan = viewTransform.position;
            Vector2 scale = viewTransform.scale;
            return new Vector2(
                (viewportCenter.x - pan.x) / scale.x,
                (viewportCenter.y - pan.y) / scale.y
            );
        }

        private Vector2 ScreenToGraphPosition(Vector2 screenPosition)
        {
            if (panel == null)
            {
                return GetGraphViewCenter();
            }

            Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(panel, screenPosition);
            Vector2 viewPosition = panelPosition - worldBound.position;
            Vector2 pan = viewTransform.position;
            Vector2 scale = viewTransform.scale;
            return new Vector2(
                (viewPosition.x - pan.x) / scale.x,
                (viewPosition.y - pan.y) / scale.y
            );
        }

        public void AddStickyNote(Vector2 position)
        {
            var stickyNote = new StickyNote
            {
                title = "Note",
                contents = "Add your notes here..."
            };
            stickyNote.SetPosition(new Rect(position, new Vector2(200, 100)));
            AddElement(stickyNote);
        }

        private DialogueNodeView CreateNodeView(DialogueNode node)
        {
            DialogueNodeView nodeView = new DialogueNodeView(node);

            // If node has no position, set a default one
            if (node.NodePosition == Vector2.zero)
            {
                node.SetPosition(new Vector2(
                    100 + (nodeViews.Count * 50) % 500,
                    100 + (nodeViews.Count / 10) * 200
                ));
            }

            nodeView.SetPosition(new Rect(node.NodePosition, new Vector2(200, 150)));
            AddElement(nodeView);
            nodeViews[node.NodeGuid] = nodeView;
            return nodeView;
        }

        private DialogueNodeView GetNodeView(string nodeGuid)
        {
            return nodeViews.ContainsKey(nodeGuid) ? nodeViews[nodeGuid] : null;
        }
    }

    /// <summary>
    /// Node view for displaying dialogue nodes in the graph.
    /// </summary>
    public class DialogueNodeView : Node
    {
        public DialogueNode Node { get; private set; }
        public Port inputPort { get; private set; }
        public Port outputPort { get; private set; }
        public List<Port> choicePorts = new List<Port>();

        public DialogueNodeView(DialogueNode node)
        {
            Node = node;
            title = node.NodeName;
            titleContainer.style.backgroundColor = new Color(node.GetNodeColor().r, node.GetNodeColor().g, node.GetNodeColor().b, 0.8f);

            // Create ports
            CreatePorts();

            // Create content
            CreateContent();

            // Refresh
            RefreshExpandedState();
            RefreshPorts();
        }

        private void CreatePorts()
        {
            // Input port (left side)
            if (!(Node is StartNode))
            {
                inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
                inputPort.portName = "Input";
                inputContainer.Add(inputPort);
            }

            // Output port (right side) - only for non-Choice nodes
            if (!(Node is EndNode) && !(Node is ChoiceNode))
            {
                outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                outputPort.portName = "Output";
                outputContainer.Add(outputPort);
            }

            // Choice nodes have multiple output ports (one per choice)
            if (Node is ChoiceNode choiceNode)
            {
                for (int i = 0; i < choiceNode.choices.Count; i++)
                {
                    var choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                    choicePort.portName = $"Choice {i + 1}";
                    outputContainer.Add(choicePort);
                    choicePorts.Add(choicePort);
                }
            }
        }

        private void CreateContent()
        {
            switch (Node)
            {
                case TextNode textNode:
                    CreateTextNodeContent(textNode);
                    break;
                case ChoiceNode choiceNode:
                    CreateChoiceNodeContent(choiceNode);
                    break;
                case ConditionNode conditionNode:
                    CreateConditionNodeContent(conditionNode);
                    break;
                case SetVariableNode setVarNode:
                    CreateSetVariableNodeContent(setVarNode);
                    break;
                case WaitNode waitNode:
                    CreateWaitNodeContent(waitNode);
                    break;
                case EventNode eventNode:
                    CreateEventNodeContent(eventNode);
                    break;
                case EndNode endNode:
                    CreateEndNodeContent(endNode);
                    break;
                default:
                    CreateDefaultContent();
                    break;
            }
        }

        private void CreateTextNodeContent(TextNode node)
        {
            var textField = new TextField("Dialogue Text")
            {
                value = node.dialogueText,
                multiline = true
            };
            textField.RegisterValueChangedCallback(evt =>
            {
                node.dialogueText = evt.newValue;
            });
            extensionContainer.Add(textField);

            var speakerField = new TextField("Speaker Name")
            {
                value = node.speakerName
            };
            speakerField.RegisterValueChangedCallback(evt =>
            {
                node.speakerName = evt.newValue;
            });
            extensionContainer.Add(speakerField);

            var speedField = new FloatField("Typing Speed")
            {
                value = node.typingSpeed
            };
            speedField.RegisterValueChangedCallback(evt =>
            {
                node.typingSpeed = evt.newValue;
            });
            extensionContainer.Add(speedField);

            var clickToProceedToggle = new Toggle("Click to Proceed")
            {
                value = node.clickToProceed
            };
            clickToProceedToggle.RegisterValueChangedCallback(evt =>
            {
                node.clickToProceed = evt.newValue;
                Refresh();
            });
            extensionContainer.Add(clickToProceedToggle);

            if (!node.clickToProceed)
            {
                var delayField = new FloatField("Auto Advance Delay (seconds)")
                {
                    value = node.autoAdvanceDelay
                };
                delayField.RegisterValueChangedCallback(evt =>
                {
                    node.autoAdvanceDelay = Mathf.Max(0f, evt.newValue);
                });
                extensionContainer.Add(delayField);
            }

            var effectToggle = new Toggle("Enable Typewriter Effect")
            {
                value = node.enableTypewriterEffect
            };
            effectToggle.RegisterValueChangedCallback(evt =>
            {
                node.enableTypewriterEffect = evt.newValue;
            });
            extensionContainer.Add(effectToggle);
        }

        private void CreateChoiceNodeContent(ChoiceNode node)
        {
            var choicesContainer = new VisualElement();
            choicesContainer.style.flexDirection = FlexDirection.Column;

            for (int i = 0; i < node.choices.Count; i++)
            {
                var choiceIndex = i;
                var choice = node.choices[i];

                var choiceContainer = new VisualElement();
                choiceContainer.style.flexDirection = FlexDirection.Column;
                choiceContainer.style.marginBottom = 5;

                var choiceLabel = new Label($"Choice {i + 1} (Port: Choice {i + 1})");
                choiceLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                choiceContainer.Add(choiceLabel);

                var choiceTextField = new TextField("Text")
                {
                    value = choice.choiceText
                };
                choiceTextField.RegisterValueChangedCallback(evt =>
                {
                    node.choices[choiceIndex].choiceText = evt.newValue;
                });
                choiceContainer.Add(choiceTextField);

                var conditionToggle = new Toggle("Require Condition")
                {
                    value = choice.requireCondition
                };
                conditionToggle.RegisterValueChangedCallback(evt =>
                {
                    node.choices[choiceIndex].requireCondition = evt.newValue;
                    Refresh();
                });
                choiceContainer.Add(conditionToggle);

                if (choice.requireCondition)
                {
                    var condVarField = new TextField("Condition Variable")
                    {
                        value = choice.conditionVariable
                    };
                    condVarField.RegisterValueChangedCallback(evt =>
                    {
                        node.choices[choiceIndex].conditionVariable = evt.newValue;
                    });
                    choiceContainer.Add(condVarField);

                    var condValueField = new TextField("Condition Value")
                    {
                        value = choice.conditionValue
                    };
                    condValueField.RegisterValueChangedCallback(evt =>
                    {
                        node.choices[choiceIndex].conditionValue = evt.newValue;
                    });
                    choiceContainer.Add(condValueField);
                }

                choicesContainer.Add(choiceContainer);
            }

            extensionContainer.Add(choicesContainer);

            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;

            var addButton = new Button(() =>
            {
                node.choices.Add(new ChoiceNode.ChoiceOption { choiceText = "New Choice" });
                RefreshPorts();
                Refresh();
            })
            {
                text = "Add Choice"
            };
            buttonContainer.Add(addButton);

            if (node.choices.Count > 0)
            {
                var removeButton = new Button(() =>
                {
                    if (node.choices.Count > 0)
                    {
                        node.choices.RemoveAt(node.choices.Count - 1);
                        RefreshPorts();
                        Refresh();
                    }
                })
                {
                    text = "Remove Last"
                };
                buttonContainer.Add(removeButton);
            }

            extensionContainer.Add(buttonContainer);
        }

        private void CreateConditionNodeContent(ConditionNode node)
        {
            var varField = new TextField("Variable Name")
            {
                value = node.variableName
            };
            varField.RegisterValueChangedCallback(evt =>
            {
                node.variableName = evt.newValue;
            });
            extensionContainer.Add(varField);

            var conditionEnum = new EnumField("Condition", node.condition);
            conditionEnum.RegisterValueChangedCallback(evt =>
            {
                node.condition = (ConditionNode.ConditionType)evt.newValue;
            });
            extensionContainer.Add(conditionEnum);

            var compareField = new TextField("Compare Value")
            {
                value = node.compareValue
            };
            compareField.RegisterValueChangedCallback(evt =>
            {
                node.compareValue = evt.newValue;
            });
            extensionContainer.Add(compareField);
        }

        private void CreateSetVariableNodeContent(SetVariableNode node)
        {
            var varField = new TextField("Variable Name")
            {
                value = node.variableName
            };
            varField.RegisterValueChangedCallback(evt =>
            {
                node.variableName = evt.newValue;
            });
            extensionContainer.Add(varField);

            var typeEnum = new EnumField("Variable Type", node.variableType);
            typeEnum.RegisterValueChangedCallback(evt =>
            {
                node.variableType = (SetVariableNode.VariableType)evt.newValue;
                Refresh();
            });
            extensionContainer.Add(typeEnum);

            switch (node.variableType)
            {
                case SetVariableNode.VariableType.String:
                    var stringField = new TextField("Value")
                    {
                        value = node.stringValue
                    };
                    stringField.RegisterValueChangedCallback(evt =>
                    {
                        node.stringValue = evt.newValue;
                    });
                    extensionContainer.Add(stringField);
                    break;

                case SetVariableNode.VariableType.Integer:
                    var intField = new IntegerField("Value")
                    {
                        value = node.intValue
                    };
                    intField.RegisterValueChangedCallback(evt =>
                    {
                        node.intValue = evt.newValue;
                    });
                    extensionContainer.Add(intField);
                    break;

                case SetVariableNode.VariableType.Float:
                    var floatField = new FloatField("Value")
                    {
                        value = node.floatValue
                    };
                    floatField.RegisterValueChangedCallback(evt =>
                    {
                        node.floatValue = evt.newValue;
                    });
                    extensionContainer.Add(floatField);
                    break;

                case SetVariableNode.VariableType.Boolean:
                    var boolField = new Toggle("Value")
                    {
                        value = node.boolValue
                    };
                    boolField.RegisterValueChangedCallback(evt =>
                    {
                        node.boolValue = evt.newValue;
                    });
                    extensionContainer.Add(boolField);
                    break;
            }
        }

        private void CreateWaitNodeContent(WaitNode node)
        {
            var waitTypeEnum = new EnumField("Wait Type", node.waitType);
            waitTypeEnum.RegisterValueChangedCallback(evt =>
            {
                node.waitType = (WaitNode.WaitType)evt.newValue;
                Refresh();
            });
            extensionContainer.Add(waitTypeEnum);

            if (node.waitType == WaitNode.WaitType.Time)
            {
                var durationField = new FloatField("Duration (seconds)")
                {
                    value = node.waitDuration
                };
                durationField.RegisterValueChangedCallback(evt =>
                {
                    node.waitDuration = evt.newValue;
                });
                extensionContainer.Add(durationField);
            }
            else if (node.waitType == WaitNode.WaitType.Button)
            {
                var inputField = new TextField("Button Input Name")
                {
                    value = node.buttonInputName
                };
                inputField.RegisterValueChangedCallback(evt =>
                {
                    node.buttonInputName = evt.newValue;
                });
                extensionContainer.Add(inputField);
            }
        }

        private void CreateEventNodeContent(EventNode node)
        {
            var eventField = new TextField("Function Name")
            {
                value = node.eventName
            };
            eventField.RegisterValueChangedCallback(evt =>
            {
                node.eventName = evt.newValue;
            });
            extensionContainer.Add(eventField);

            var label = new Label("Declare this function on the graph and assign its UnityEvent on DialogueGraphRunner.");
            label.style.fontSize = 10;
            label.style.unityFontStyleAndWeight = FontStyle.Italic;
            extensionContainer.Add(label);
        }

        private void CreateEndNodeContent(EndNode node)
        {
            var reasonField = new TextField("End Reason")
            {
                value = node.endReason
            };
            reasonField.RegisterValueChangedCallback(evt =>
            {
                node.endReason = evt.newValue;
            });
            extensionContainer.Add(reasonField);
        }

        private void CreateDefaultContent()
        {
            var label = new Label("No content");
            extensionContainer.Add(label);
        }

        private void Refresh()
        {
            extensionContainer.Clear();
            CreateContent();
        }

        private new void RefreshPorts()
        {
            // Clear existing choice ports
            foreach (var port in choicePorts)
            {
                outputContainer.Remove(port);
            }
            choicePorts.Clear();

            // Recreate ports based on current node state
            if (Node is ChoiceNode choiceNode)
            {
                for (int i = 0; i < choiceNode.choices.Count; i++)
                {
                    var choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                    choicePort.portName = $"Choice {i + 1}";
                    outputContainer.Add(choicePort);
                    choicePorts.Add(choicePort);
                }
            }

            RefreshExpandedState();
            base.RefreshPorts();
        }
    }

    [CustomEditor(typeof(DialogueGraphRunner))]
    public class DialogueGraphRunnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DialogueGraphRunner runner = (DialogueGraphRunner)target;
            if (runner.SynchronizeGraphBindings())
            {
                EditorUtility.SetDirty(runner);
            }

            DrawDefaultInspector();
        }
    }
}
#endif