#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TypewriterDialoguePackage.VisualEditor
{
    /// <summary>
    /// Runtime debugging tool for dialogue systems.
    /// Provides inspector interface for monitoring and controlling dialogue execution.
    /// </summary>
    public class DialogueDebugger : EditorWindow
    {
        private DialogueGraphRunner targetRunner;
        private Vector2 scrollPosition;
        private bool showVariables = true;
        private bool showExecutionLog = true;

        private List<string> executionLog = new List<string>();
        private int maxLogEntries = 50;

        [MenuItem("Window/Typewriter Dialogue/Debugger")]
        public static void ShowWindow()
        {
            GetWindow<DialogueDebugger>("Dialogue Debugger");
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawRunnerSelection();
            DrawStatus();
            DrawControls();
            DrawVariables();
            DrawExecutionLog();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label("Dialogue Debugger", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear Log", EditorStyles.toolbarButton))
            {
                executionLog.Clear();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawRunnerSelection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Target Runner", EditorStyles.boldLabel);
            targetRunner = (DialogueGraphRunner)EditorGUILayout.ObjectField(
                targetRunner, typeof(DialogueGraphRunner), true);

            EditorGUILayout.EndVertical();
        }

        private void DrawStatus()
        {
            if (targetRunner == null) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);

            bool isRunning = targetRunner.IsRunning;
            bool isWaiting = targetRunner.IsWaitingForInput;

            EditorGUILayout.LabelField($"Running: {(isRunning ? "Yes" : "No")}");
            EditorGUILayout.LabelField($"Waiting for Input: {(isWaiting ? "Yes" : "No")}");

            if (isRunning)
            {
                // Get current node info if available
                EditorGUILayout.LabelField("Current Dialogue", EditorStyles.boldLabel);
                // This would require additional access to internal state
                EditorGUILayout.LabelField("Node execution in progress...");
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawControls()
        {
            if (targetRunner == null) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Start Dialogue"))
            {
                targetRunner.StartDialogue();
                AddLogEntry("Dialogue started");
            }

            if (GUILayout.Button("Stop Dialogue"))
            {
                targetRunner.StopDialogue();
                AddLogEntry("Dialogue stopped");
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Skip Current Step"))
            {
                targetRunner.SkipCurrentStep();
                AddLogEntry("Skipped current step");
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawVariables()
        {
            if (targetRunner == null) return;

            showVariables = EditorGUILayout.Foldout(showVariables, "Variables");

            if (showVariables)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Test variable access
                EditorGUILayout.LabelField("Test Variable Access", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                string varName = EditorGUILayout.TextField("Variable Name:");
                if (GUILayout.Button("Get", GUILayout.Width(50)))
                {
                    string value = targetRunner.GetVariable<string>(varName, "Not found");
                    AddLogEntry($"Variable '{varName}': {value}");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                string setValue = EditorGUILayout.TextField("Set Value:");
                if (GUILayout.Button("Set", GUILayout.Width(50)))
                {
                    targetRunner.SetVariable(varName, setValue);
                    AddLogEntry($"Set variable '{varName}' to '{setValue}'");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawExecutionLog()
        {
            showExecutionLog = EditorGUILayout.Foldout(showExecutionLog, "Execution Log");

            if (showExecutionLog)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

                foreach (string logEntry in executionLog)
                {
                    EditorGUILayout.LabelField(logEntry, EditorStyles.wordWrappedLabel);
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.EndVertical();
            }
        }

        private void AddLogEntry(string message)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            executionLog.Add($"[{timestamp}] {message}");

            // Limit log size
            if (executionLog.Count > maxLogEntries)
            {
                executionLog.RemoveAt(0);
            }

            // Auto-scroll to bottom
            scrollPosition = new Vector2(0, float.MaxValue);
        }

        private void Update()
        {
            // Auto-refresh if target runner is running
            if (targetRunner != null && targetRunner.IsRunning)
            {
                Repaint();
            }
        }
    }
}
#endif