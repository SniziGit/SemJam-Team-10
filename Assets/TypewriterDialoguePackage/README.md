# TypewriterDialoguePackage User Guide

## Overview
The TypewriterDialoguePackage is a visual node-based dialogue system for Unity. It provides an intuitive drag-and-drop interface for creating complex dialogue systems with branching, conditional logic, and rich text effects.

## 🚀 Quick Start

### 1. Open the Visual Editor
Navigate to `Window/Typewriter Dialogue/Visual Editor` in Unity's menu bar.

### 2. Create a Dialogue Graph
Right-click in your Project window → `Create` → `Typewriter Dialogue` → `Dialogue Graph`
Or use a pre-built template for quick starts.

### 3. Design Your Dialogue
- **Load Graph**: Select your Dialogue Graph asset in the toolbar
- **Add Nodes**: Click the node type buttons in the toolbar (+ Text, + Choice, etc.)
- **Connect Nodes**: Drag from the output port (right) to the input port (left) of another node
- **Edit Nodes**: Click on a node to select it, then edit its properties directly in the node
- **Move Nodes**: Drag nodes to reorganize your graph
- **Zoom/Pan**: Use mouse wheel to zoom, middle-click to pan
- **Save Graph**: Click "Save Graph" in the toolbar

### 4. Set Up Runtime Execution in the Scene
1. Add a `DialogueGraphRunner` component to a GameObject in your scene.
2. Assign the saved graph asset to **Dialogue Graph** in the Inspector.
3. Assign the dialogue and speaker `TextMeshProUGUI` objects to **Dialogue Text** and **Speaker Name Text**.
4. For choice nodes, assign a **Choice Panel** and a **Choice Button Prefab** containing a `Button` and a child `TextMeshProUGUI`.
5. Leave **Play On Start** enabled, or add `DialogueGraphRunner.StartDialogue` to a UI Button's **On Click** event.

`TextEffectManager` and `PopupManager` are optional. The sample scene at `Assets/TypewriterDialoguePackage/TypewriterDialohguePackage.unity` already has a graph runner and UI references assigned.

## 🎨 How to Use the Visual Editor

### **Opening the Editor**
Navigate to `Window/Typewriter Dialogue/Visual Editor` in Unity's menu bar.

### **Creating a Dialogue Graph**
1. Right-click in your Project window → `Create` → `Typewriter Dialogue` → `Dialogue Graph`
2. Select the graph in the Visual Editor window's toolbar (top)
3. Start adding nodes to create your dialogue

### **Adding Nodes**
**Method 1: Toolbar Buttons**
- Click the node type buttons in the toolbar to add nodes:
  - **+ Text**: Add a dialogue text node
  - **+ Choice**: Add a choice node for player decisions
  - **+ Condition**: Add a conditional branching node
  - **+ Set Var**: Add a variable-setting node
  - **+ Wait**: Add a timing/input wait node
  - **+ Event**: Add a custom event node
  - **+ Start**: Add a start node (entry point)
  - **+ End**: Add an end node (exit point)
  - **+ Note**: Add a sticky note for annotations

**Method 2: Right-Click Context Menu**
- Right-click anywhere in the graph area
- Select the node type from the context menu
- Can also add **Sticky Notes** for annotations

### **Selecting and Moving Nodes**
- **Left-click** on a node to select it
- **Left-click and drag** to move nodes around the graph
- **Click empty space** to deselect

### **Connecting Nodes**
1. **Drag from output port** (right side of a node) to **input port** (left side of another node)
2. The connection will be drawn as a bezier curve
3. Nodes can have multiple outgoing connections (for branching paths)
4. **Choice Nodes**: Each choice has its own output port (labeled "Choice 1", "Choice 2", etc.) - connect each to different nodes for branching dialogue
5. Right-click on a connection and select "Delete" to remove it

### **Editing Node Properties**
- **Click on a node** to select it
- **Edit properties directly in the node** (text fields, dropdowns, toggles, etc.)
- **Text Node**: Dialogue text, speaker name, typing speed, wait settings, typewriter effect toggle
- **Choice Node**: Add/remove choices, set choice text, configure conditions for each choice. Each choice has its own output port (Choice 1, Choice 2, etc.) that can be connected to different dialogue paths
- **Condition Node**: Variable name, condition type (Equals, GreaterThan, Contains, etc.), compare value
- **Set Variable Node**: Variable name, variable type (String, Integer, Float, Boolean), type-specific value field
- **Wait Node**: Wait type (Time, Input, Button), duration (for Time type)
- **Event Node**: Function name declared on the graph and bound to a UnityEvent on DialogueGraphRunner
- **End Node**: End reason message
- **Sticky Notes**: Add annotations and comments to your graph (click the note to edit title and content). Note: Sticky notes are editor-only visual elements and are not saved to the DialogueGraph asset.
- Changes are saved when you click "Save Graph"

### **Deleting Nodes**
- Select a node → Press **Delete** key
- Or right-click on a node → Delete

### **Zooming and Panning**
- **Mouse wheel**: Zoom in/out
- **Middle-click + drag**: Pan the view
- **Right-click + drag**: Pan the view (alternative)

### **Saving Your Work**
- Click **"Save Graph"** in the toolbar to save changes to the Dialogue Graph asset
- Changes are saved to the ScriptableObject asset

## Graph Runtime Declarations

Runtime declaration names are collected from the graph nodes when the graph loads/validates and when you click **Save Graph**:
- Variables are collected from Set Variable, Condition, and conditional Choice nodes. Their type and default value remain editable in the DialogueGraph asset Inspector.
- Function names are collected from Event nodes.
- Button input names are collected from Button Wait nodes. For example, a Wait node named `NextButton` automatically adds one `NextButton` binding, even if several Wait nodes use it.

After assigning the graph to a DialogueGraphRunner, its **Graph Bindings** section synchronizes to those unique names:
- Enable **Override Graph Default** to use the runner's typed variable value instead of the graph default.
- Assign a UnityEvent to each function binding.
- Assign a Unity UI Button to each button input binding.

Repeated names share one runner binding. Condition/Set Variable node variable names, Event node Function Names, and Wait node Button Input Names are the source of these declarations. Button waits proceed when the assigned Button is clicked.

## 🎨 Node Types

### **Text Node (Blue)**
Displays dialogue text with typewriter effect.
- **dialogueText**: The text to display
- **speakerName**: Optional speaker name
- **typingSpeed**: Characters per second
- **Click to Proceed**: Wait for a click after the text finishes displaying; turn off to advance automatically after the configurable **Auto Advance Delay** (default 1.5 seconds)
- **enableTypewriterEffect**: Use typewriter animation

### **Choice Node (Green)**
Presents multiple choices to the player.
- **choices**: List of choice options
- Each choice can have:
  - **choiceText**: Display text
  - **requireCondition**: Enable conditional availability
  - **conditionVariable**: Variable to check
  - **conditionValue**: Required value

### **Condition Node (Orange)**
Branches dialogue based on variable values.
- **variableName**: Variable to check
- **condition**: Comparison type (Equals, GreaterThan, Contains, etc.)
- **compareValue**: Value to compare against
- **trueNodeGuid**: Node to execute if condition is true
- **falseNodeGuid**: Node to execute if condition is false

### **Set Variable Node (Red)**
Sets variable values during dialogue.
- **variableName**: Variable name
- **variableType**: String, Integer, Float, or Boolean
- **value**: The value to set

### **Wait Node (Purple)**
Controls timing and input.
- **waitType**: Time, Input, or Button
- **waitDuration**: Wait time (for Time type)
- **buttonInputName**: Graph-declared button input to wait for (for Button type)

### **Event Node (Yellow)**
Invokes a graph-declared function binding.
- **eventName**: Function name to invoke on DialogueGraphRunner

### **Start Node (Green)**
The entry point for your dialogue.
- Set this as the first node in your graph

### **End Node (Red)**
The exit point for your dialogue.
- Ends the dialogue when reached

## 📊 Templates

### **Quick Templates**
Right-click in Project window → `Create` → `Typewriter Dialogue` → Template Name

Available templates:
- **Linear Dialogue**: Simple conversation from start to end
- **Branching Dialogue**: Multiple choice paths
- **Conditional Dialogue**: Branching based on variables
- **Shop Dialogue**: Shopping interface
- **Quest Dialogue**: Quest assignment and tracking
- **Tutorial Dialogue**: Step-by-step tutorial

## 🔧 Runtime Setup

### **Basic Setup**
Configure `DialogueGraphRunner` and its UI references in the Inspector; those fields are serialized and are not public script fields. To start dialogue from code, keep a reference to the component and call `StartDialogue()`. To assign a graph at runtime, call `SetDialogueGraph(graph)` before starting it.

### **UI Setup**
You need these UI elements in your scene:
- **TextMeshProUGUI**: For displaying dialogue text
- **Choice Panel**: GameObject to hold choice buttons
- **Choice Button Prefab**: Button prefab for choices

### **Event Handling**
```csharp
dialogueRunner.OnDialogueStart += () => {
    Debug.Log("Dialogue started!");
};

dialogueRunner.OnDialogueEnd += () => {
    Debug.Log("Dialogue ended!");
};

dialogueRunner.OnTextDisplay += (speaker, text) => {
    Debug.Log($"{speaker}: {text}");
};

dialogueRunner.OnChoicesPresented += (choices) => {
    Debug.Log($"Choices: {string.Join(", ", choices)}");
};
```

## 🎯 Common Use Cases

### **Simple Linear Dialogue**
1. Create DialogueGraph asset
2. Add Start → Text → End nodes
3. Connect them in sequence
4. Configure Text node with your dialogue
5. Set up DialogueGraphRunner

### **Player Choices**
1. Create DialogueGraph asset
2. Add Start → Text → Choice → Text → End nodes
3. Configure Choice node with multiple options
4. Connect each choice to different dialogue paths
5. Set up choice panel and button prefab

### **Conditional Dialogue**
1. Create DialogueGraph asset
2. Add Start → Set Variable → Condition → Text nodes
3. Configure Set Variable node to set a variable
4. Configure Condition node to check the variable
5. Connect true/false branches to different outcomes

### **Complex Quest System**
1. Create DialogueGraph asset
2. Use Condition nodes to check quest status
3. Use Set Variable nodes to update quest progress
4. Use Choice nodes for player decisions
5. Use Event nodes to trigger quest rewards

## 🔍 Advanced Features

### **Variable System**
Declare variable names, types, and defaults in the DialogueGraph asset Inspector. DialogueGraphRunner initializes the dialogue context from those defaults; enable a runner override to supply a different starting value.

```csharp
// Set a variable
dialogueRunner.SetVariable("playerLevel", 5);

// Get a variable
int level = dialogueRunner.GetVariable<int>("playerLevel", 1);

// Check a variable
bool hasKey = dialogueRunner.GetVariable<bool>("hasKey", false);
```

### **Enhanced Components (Optional)**
```csharp
// Add enhanced components for additional features
TextEffectManager effectManager = gameObject.AddComponent<TextEffectManager>();
PopupManager popupManager = gameObject.AddComponent<PopupManager>();
UIAnimationManager animManager = gameObject.AddComponent<UIAnimationManager>();

// Use them independently
effectManager.ApplyShakeEffect(myText, 1f, 10f, 2f);
popupManager.ShowPopup("Title", "Content", true, true);
```

### **Rich Text Formatting**
```csharp
using TypewriterDialoguePackage.Utility;

// Color specific words
string coloredText = RichTextHelper.ColorWord("Hello world", "world", Color.red);

// Multiple effects
string formatted = RichTextHelper.FormatWord(
    "Hello world", "world",
    color: Color.red,
    bold: true,
    underline: true
);
```

## 🐛 Debugging

### **Using the Debugger**
1. Open `Window/Typewriter Dialogue/Debugger`
2. Select your DialogueGraphRunner
3. Start dialogue and monitor execution
4. View and modify variables in real-time
5. Check execution log for debugging

### **Common Issues**
- **No dialogue displayed**: Check UI references are assigned
- **Choices not appearing**: Verify choice panel and button prefab are set
- **Dialogue not advancing**: Check node connections are correct
- **Variables not working**: Use debugger to inspect variable values

## 📁 File Structure
```
Assets/TypewriterDialoguePackage/
├── VisualEditor/              # Main system
│   ├── DialogueNode.cs       # Base node system
│   ├── DialogueNodeTypes.cs    # Node implementations
│   ├── DialogueGraph.cs        # Graph container
│   ├── DialogueGraphRunner.cs # Runtime execution
│   ├── DialogueGraphViewWindow.cs # GraphView editor
│   ├── DialogueTemplates.cs    # Pre-built templates
│   ├── TypewriterToVisualBridge.cs # Integration helper
│   └── DialogueDebugger.cs     # Debugging tools
├── Components/                # Optional enhancements
│   ├── TextEffectManager.cs
│   ├── PopupManager.cs
│   └── UIAnimationManager.cs
├── Utility/                   # Utilities
│   ├── TypewriterEffect.cs    # Core typewriter effect
│   ├── RichTextHelper.cs      # Text formatting
│   ├── ConditionalHide.cs
│   └── ConditionalHeader.cs
└── Editor/                    # Editor tools
    ├── ConditionalHidePropertyDrawer.cs
    └── ConditionalHeaderPropertyDrawer.cs
```

## 🎓 Learning Path

### **Beginner**
1. Create a simple linear dialogue
2. Set up basic UI and DialogueGraphRunner
3. Test the dialogue in Play mode

### **Intermediate**
1. Add choices to your dialogue
2. Implement conditional branching
3. Use variables for dialogue state
4. Set up choice UI with proper button prefabs

### **Advanced**
1. Create complex quest systems
2. Use enhanced components for effects
3. Implement custom node types
4. Use debugger for runtime testing
5. Create your own dialogue templates

## 💡 Tips

- **Start Simple**: Begin with linear dialogues before adding complexity
- **Use Templates**: Start from pre-built templates for common patterns
- **Validate Often**: Use the "Validate Graph" button to check for issues
- **Test Incrementally**: Test each dialogue branch as you build it
- **Use Debugger**: Monitor dialogue execution to identify issues
- **Organize Nodes**: Keep related dialogue nodes together for clarity
- **Label Clearly**: Use descriptive node names for easy identification

## 🚀 Next Steps

1. **Create Your First Dialogue**: Use the visual editor to design a simple conversation
2. **Set Up UI**: Create basic UI elements for text display and choices
3. **Test Runtime**: Play your scene and verify dialogue execution
4. **Add Complexity**: Gradually add choices, conditions, and variables
5. **Enhance**: Add optional components for text effects and animations
6. **Debug**: Use the debugger to monitor and troubleshoot your dialogue

## 🤝 Support

For issues or questions:
- Check the debugger for runtime issues
- Validate your graph for structural problems
- Review node connections and settings
- Ensure UI references are properly assigned

The visual editor makes complex dialogue systems intuitive and manageable. Start simple, test often, and gradually add complexity as you become familiar with the system!