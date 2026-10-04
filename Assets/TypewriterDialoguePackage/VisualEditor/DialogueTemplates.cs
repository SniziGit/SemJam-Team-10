using UnityEngine;

namespace TypewriterDialoguePackage.VisualEditor
{
    /// <summary>
    /// Factory class for creating pre-built dialogue graph templates.
    /// Provides common dialogue patterns that can be customized.
    /// </summary>
    public static class DialogueTemplates
    {
        /// <summary>
        /// Create a simple linear dialogue template.
        /// </summary>
        public static DialogueGraph CreateLinearDialogue()
        {
            DialogueGraph graph = ScriptableObject.CreateInstance<DialogueGraph>();
            graph.name = "Linear Dialogue";

            StartNode startNode = new StartNode();
            startNode.SetPosition(new Vector2(50f, 50f));
            graph.AddNode(startNode);

            TextNode textNode1 = new TextNode();
            textNode1.SetName("Greeting");
            textNode1.dialogueText = "Hello there! Welcome to our world.";
            textNode1.SetPosition(new Vector2(300f, 50f));
            graph.AddNode(textNode1);

            TextNode textNode2 = new TextNode();
            textNode2.SetName("Introduction");
            textNode2.dialogueText = "I am the guide for this journey.";
            textNode2.SetPosition(new Vector2(550f, 50f));
            graph.AddNode(textNode2);

            EndNode endNode = new EndNode();
            endNode.SetPosition(new Vector2(800f, 50f));
            graph.AddNode(endNode);

            // Connect nodes
            startNode.AddConnection(textNode1.NodeGuid);
            textNode1.AddConnection(textNode2.NodeGuid);
            textNode2.AddConnection(endNode.NodeGuid);

            graph.SetStartNode(startNode.NodeGuid);

            return graph;
        }

        /// <summary>
        /// Create a branching dialogue with choices template.
        /// </summary>
        public static DialogueGraph CreateBranchingDialogue()
        {
            DialogueGraph graph = ScriptableObject.CreateInstance<DialogueGraph>();
            graph.name = "Branching Dialogue";

            StartNode startNode = new StartNode();
            startNode.SetPosition(new Vector2(50f, 200f));
            graph.AddNode(startNode);

            TextNode introNode = new TextNode();
            introNode.SetName("Introduction");
            introNode.dialogueText = "You stand at a crossroads. Which path do you choose?";
            introNode.SetPosition(new Vector2(300f, 200f));
            graph.AddNode(introNode);

            ChoiceNode choiceNode = new ChoiceNode();
            choiceNode.SetName("Path Choice");
            choiceNode.choices[0].choiceText = "Take the left path";
            choiceNode.choices[1].choiceText = "Take the right path";
            choiceNode.SetPosition(new Vector2(550f, 200f));
            graph.AddNode(choiceNode);

            TextNode leftPathNode = new TextNode();
            leftPathNode.SetName("Left Path");
            leftPathNode.dialogueText = "You venture into the dark forest...";
            leftPathNode.SetPosition(new Vector2(800f, 100f));
            graph.AddNode(leftPathNode);

            TextNode rightPathNode = new TextNode();
            rightPathNode.SetName("Right Path");
            rightPathNode.dialogueText = "You walk along the sunny meadow...";
            rightPathNode.SetPosition(new Vector2(800f, 300f));
            graph.AddNode(rightPathNode);

            EndNode endNode = new EndNode();
            endNode.SetPosition(new Vector2(1050f, 200f));
            graph.AddNode(endNode);

            // Connect nodes
            startNode.AddConnection(introNode.NodeGuid);
            introNode.AddConnection(choiceNode.NodeGuid);
            choiceNode.AddConnection(leftPathNode.NodeGuid);
            choiceNode.AddConnection(rightPathNode.NodeGuid);
            leftPathNode.AddConnection(endNode.NodeGuid);
            rightPathNode.AddConnection(endNode.NodeGuid);

            graph.SetStartNode(startNode.NodeGuid);

            return graph;
        }

        /// <summary>
        /// Create a conditional dialogue template.
        /// </summary>
        public static DialogueGraph CreateConditionalDialogue()
        {
            DialogueGraph graph = ScriptableObject.CreateInstance<DialogueGraph>();
            graph.name = "Conditional Dialogue";

            StartNode startNode = new StartNode();
            startNode.SetPosition(new Vector2(50f, 100f));
            graph.AddNode(startNode);

            TextNode greetingNode = new TextNode();
            greetingNode.SetName("Greeting");
            greetingNode.dialogueText = "Hello! How can I help you today?";
            greetingNode.SetPosition(new Vector2(300f, 100f));
            graph.AddNode(greetingNode);

            SetVariableNode setVarNode = new SetVariableNode();
            setVarNode.SetName("Set Player Level");
            setVarNode.variableName = "playerLevel";
            setVarNode.variableType = SetVariableNode.VariableType.Integer;
            setVarNode.intValue = 5;
            setVarNode.SetPosition(new Vector2(550f, 100f));
            graph.AddNode(setVarNode);

            ConditionNode conditionNode = new ConditionNode();
            conditionNode.SetName("Check Level");
            conditionNode.variableName = "playerLevel";
            conditionNode.condition = ConditionNode.ConditionType.GreaterThan;
            conditionNode.compareValue = "10";
            conditionNode.SetPosition(new Vector2(800f, 100f));
            graph.AddNode(conditionNode);

            TextNode lowLevelNode = new TextNode();
            lowLevelNode.SetName("Low Level Response");
            lowLevelNode.dialogueText = "I see you're new here. Let me help you get started!";
            lowLevelNode.SetPosition(new Vector2(1050f, 50f));
            graph.AddNode(lowLevelNode);

            TextNode highLevelNode = new TextNode();
            highLevelNode.SetName("High Level Response");
            highLevelNode.dialogueText = "Ah, an experienced adventurer! I have special quests for you.";
            highLevelNode.SetPosition(new Vector2(1050f, 150f));
            graph.AddNode(highLevelNode);

            EndNode endNode = new EndNode();
            endNode.SetPosition(new Vector2(1300f, 100f));
            graph.AddNode(endNode);

            // Connect nodes
            startNode.AddConnection(greetingNode.NodeGuid);
            greetingNode.AddConnection(setVarNode.NodeGuid);
            setVarNode.AddConnection(conditionNode.NodeGuid);
            conditionNode.trueNodeGuid = highLevelNode.NodeGuid;
            conditionNode.falseNodeGuid = lowLevelNode.NodeGuid;
            highLevelNode.AddConnection(endNode.NodeGuid);
            lowLevelNode.AddConnection(endNode.NodeGuid);

            graph.SetStartNode(startNode.NodeGuid);

            return graph;
        }

        /// <summary>
        /// Create a shop dialogue template.
        /// </summary>
        public static DialogueGraph CreateShopDialogue()
        {
            DialogueGraph graph = ScriptableObject.CreateInstance<DialogueGraph>();
            graph.name = "Shop Dialogue";

            StartNode startNode = new StartNode();
            startNode.SetPosition(new Vector2(50f, 150f));
            graph.AddNode(startNode);

            TextNode welcomeNode = new TextNode();
            welcomeNode.SetName("Welcome");
            welcomeNode.speakerName = "Shopkeeper";
            welcomeNode.dialogueText = "Welcome to my shop! What would you like to buy?";
            welcomeNode.SetPosition(new Vector2(300f, 150f));
            graph.AddNode(welcomeNode);

            ChoiceNode shopChoiceNode = new ChoiceNode();
            shopChoiceNode.SetName("Shop Choices");
            shopChoiceNode.choices[0].choiceText = "Buy weapons";
            shopChoiceNode.choices[1].choiceText = "Buy armor";
            shopChoiceNode.choices[0].choiceText = "Leave shop";
            shopChoiceNode.SetPosition(new Vector2(550f, 150f));
            graph.AddNode(shopChoiceNode);

            TextNode weaponsNode = new TextNode();
            weaponsNode.SetName("Weapons");
            weaponsNode.speakerName = "Shopkeeper";
            weaponsNode.dialogueText = "Here are our finest weapons!";
            weaponsNode.SetPosition(new Vector2(800f, 50f));
            graph.AddNode(weaponsNode);

            TextNode armorNode = new TextNode();
            armorNode.SetName("Armor");
            armorNode.speakerName = "Shopkeeper";
            armorNode.dialogueText = "Protect yourself with our armor!";
            armorNode.SetPosition(new Vector2(800f, 150f));
            graph.AddNode(armorNode);

            TextNode leaveNode = new TextNode();
            leaveNode.SetName("Goodbye");
            leaveNode.speakerName = "Shopkeeper";
            leaveNode.dialogueText = "Come back anytime!";
            leaveNode.SetPosition(new Vector2(800f, 250f));
            graph.AddNode(leaveNode);

            EndNode endNode = new EndNode();
            endNode.SetPosition(new Vector2(1050f, 150f));
            graph.AddNode(endNode);

            // Connect nodes
            startNode.AddConnection(welcomeNode.NodeGuid);
            welcomeNode.AddConnection(shopChoiceNode.NodeGuid);
            shopChoiceNode.AddConnection(weaponsNode.NodeGuid);
            shopChoiceNode.AddConnection(armorNode.NodeGuid);
            shopChoiceNode.AddConnection(leaveNode.NodeGuid);
            weaponsNode.AddConnection(endNode.NodeGuid);
            armorNode.AddConnection(endNode.NodeGuid);
            leaveNode.AddConnection(endNode.NodeGuid);

            graph.SetStartNode(startNode.NodeGuid);

            return graph;
        }

        /// <summary>
        /// Create a quest dialogue template.
        /// </summary>
        public static DialogueGraph CreateQuestDialogue()
        {
            DialogueGraph graph = ScriptableObject.CreateInstance<DialogueGraph>();
            graph.name = "Quest Dialogue";

            StartNode startNode = new StartNode();
            startNode.SetPosition(new Vector2(50f, 150f));
            graph.AddNode(startNode);

            ConditionNode hasQuestNode = new ConditionNode();
            hasQuestNode.SetName("Check Quest Status");
            hasQuestNode.variableName = "hasActiveQuest";
            hasQuestNode.condition = ConditionNode.ConditionType.Equals;
            hasQuestNode.compareValue = "true";
            hasQuestNode.SetPosition(new Vector2(300f, 150f));
            graph.AddNode(hasQuestNode);

            TextNode questOfferNode = new TextNode();
            questOfferNode.SetName("Quest Offer");
            questOfferNode.speakerName = "Quest Giver";
            questOfferNode.dialogueText = "I have a quest for you. Will you accept?";
            questOfferNode.SetPosition(new Vector2(550f, 50f));
            graph.AddNode(questOfferNode);

            ChoiceNode acceptChoiceNode = new ChoiceNode();
            acceptChoiceNode.SetName("Accept Quest");
            acceptChoiceNode.choices[0].choiceText = "Accept quest";
            acceptChoiceNode.choices[1].choiceText = "Decline quest";
            acceptChoiceNode.SetPosition(new Vector2(800f, 50f));
            graph.AddNode(acceptChoiceNode);

            SetVariableNode setQuestNode = new SetVariableNode();
            setQuestNode.SetName("Set Quest Active");
            setQuestNode.variableName = "hasActiveQuest";
            setQuestNode.variableType = SetVariableNode.VariableType.Boolean;
            setQuestNode.boolValue = true;
            setQuestNode.SetPosition(new Vector2(1050f, 50f));
            graph.AddNode(setQuestNode);

            TextNode questProgressNode = new TextNode();
            questProgressNode.SetName("Quest Progress");
            questProgressNode.speakerName = "Quest Giver";
            questProgressNode.dialogueText = "How is the quest going?";
            questProgressNode.SetPosition(new Vector2(550f, 250f));
            graph.AddNode(questProgressNode);

            EndNode endNode = new EndNode();
            endNode.SetPosition(new Vector2(1300f, 150f));
            graph.AddNode(endNode);

            // Connect nodes
            startNode.AddConnection(hasQuestNode.NodeGuid);
            hasQuestNode.falseNodeGuid = questOfferNode.NodeGuid;
            hasQuestNode.trueNodeGuid = questProgressNode.NodeGuid;
            questOfferNode.AddConnection(acceptChoiceNode.NodeGuid);
            acceptChoiceNode.AddConnection(setQuestNode.NodeGuid);
            setQuestNode.AddConnection(endNode.NodeGuid);
            questProgressNode.AddConnection(endNode.NodeGuid);

            graph.SetStartNode(startNode.NodeGuid);

            return graph;
        }

        /// <summary>
        /// Create a tutorial dialogue template.
        /// </summary>
        public static DialogueGraph CreateTutorialDialogue()
        {
            DialogueGraph graph = ScriptableObject.CreateInstance<DialogueGraph>();
            graph.name = "Tutorial Dialogue";

            StartNode startNode = new StartNode();
            startNode.SetPosition(new Vector2(50f, 100f));
            graph.AddNode(startNode);

            TextNode step1Node = new TextNode();
            step1Node.SetName("Step 1");
            step1Node.dialogueText = "Welcome! Press SPACE to move forward.";
            step1Node.typingSpeed = 40f;
            step1Node.SetPosition(new Vector2(300f, 100f));
            graph.AddNode(step1Node);

            WaitNode waitNode = new WaitNode();
            waitNode.SetName("Wait For Input");
            waitNode.waitType = WaitNode.WaitType.Input;
            waitNode.SetPosition(new Vector2(550f, 100f));
            graph.AddNode(waitNode);

            TextNode step2Node = new TextNode();
            step2Node.SetName("Step 2");
            step2Node.dialogueText = "Great! Now press E to interact with objects.";
            step2Node.typingSpeed = 40f;
            step2Node.SetPosition(new Vector2(800f, 100f));
            graph.AddNode(step2Node);

            WaitNode waitNode2 = new WaitNode();
            waitNode2.SetName("Wait For Input 2");
            waitNode2.waitType = WaitNode.WaitType.Input;
            waitNode2.SetPosition(new Vector2(1050f, 100f));
            graph.AddNode(waitNode2);

            TextNode step3Node = new TextNode();
            step3Node.SetName("Step 3");
            step3Node.dialogueText = "Excellent! You're ready to begin your adventure.";
            step3Node.typingSpeed = 40f;
            step3Node.SetPosition(new Vector2(1300f, 100f));
            graph.AddNode(step3Node);

            EndNode endNode = new EndNode();
            endNode.SetPosition(new Vector2(1550f, 100f));
            graph.AddNode(endNode);

            // Connect nodes
            startNode.AddConnection(step1Node.NodeGuid);
            step1Node.AddConnection(waitNode.NodeGuid);
            waitNode.AddConnection(step2Node.NodeGuid);
            step2Node.AddConnection(waitNode2.NodeGuid);
            waitNode2.AddConnection(step3Node.NodeGuid);
            step3Node.AddConnection(endNode.NodeGuid);

            graph.SetStartNode(startNode.NodeGuid);

            return graph;
        }
    }
}