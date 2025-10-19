using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace CrazyDev007.LevelEditor.Editor
{
    public class TubeColorLevelEditorWindow : EditorWindow
    {
        private List<TubeColorLevel> levels = new List<TubeColorLevel>();
        private TubeColorLevelSerializer serializer = new TubeColorLevelSerializer();
        private string jsonFilePath = "Packages/com.crazydev007.leveleditor/Editor/Resources/color_sort_1000_levels.json";
        
        private int selectedLevelIndex = 0;
        private Vector2 scrollPosition = Vector2.zero;
        private Vector2 tubeScrollPosition = Vector2.zero;
        
        private int newTubeColorValue = 0;
        private int removeIndex = 0;

        [MenuItem("Tools/Tube Color Level Editor/Open Editor")]
        public static void ShowWindow()
        {
            TubeColorLevelEditorWindow window = GetWindow<TubeColorLevelEditorWindow>("Tube Color Editor");
            window.minSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Tube Color Level Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // File operations
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Levels", GUILayout.Width(120)))
            {
                LoadLevels();
            }
            if (GUILayout.Button("Save Levels", GUILayout.Width(120)))
            {
                SaveLevels();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("File Path:", jsonFilePath);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Browse...", GUILayout.Width(100)))
            {
                BrowseJsonFile();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();

            if (levels.Count == 0)
            {
                EditorGUILayout.HelpBox("No levels loaded. Click 'Load Levels' to get started.", MessageType.Info);
                return;
            }

            // Level selection
            EditorGUILayout.LabelField($"Total Levels: {levels.Count}", EditorStyles.boldLabel);
            selectedLevelIndex = EditorGUILayout.IntSlider("Select Level", selectedLevelIndex, 0, levels.Count - 1);
            
            EditorGUILayout.Space();
            
            if (selectedLevelIndex >= 0 && selectedLevelIndex < levels.Count)
            {
                EditLevel(levels[selectedLevelIndex]);
            }
        }

        private void EditLevel(TubeColorLevel level)
        {
            EditorGUILayout.LabelField($"Level {level.level}", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            // Level properties
            EditorGUILayout.LabelField("Level Properties", EditorStyles.boldLabel);
            level.emptyTubes = EditorGUILayout.IntField("Empty Tubes", level.emptyTubes);
            level.maxMoves = EditorGUILayout.IntField("Max Moves", level.maxMoves);
            level.timeLimit = EditorGUILayout.IntField("Time Limit (seconds)", level.timeLimit);
            level.availableSwaps = EditorGUILayout.IntField("Available Swaps", level.availableSwaps);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tubes Editor", EditorStyles.boldLabel);

            tubeScrollPosition = EditorGUILayout.BeginScrollView(tubeScrollPosition, GUILayout.Height(300));

            for (int i = 0; i < level.tubes.Count; i++)
            {
                EditTube(level, i);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            // Add new tube
            if (GUILayout.Button("Add New Tube", GUILayout.Height(30)))
            {
                level.tubes.Add(new TubeData());
                level.emptyTubes++;
            }

            // Remove tube
            if (level.tubes.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Remove Tube Index:", GUILayout.Width(120));
                removeIndex = EditorGUILayout.IntField(removeIndex, GUILayout.Width(50));
                if (GUILayout.Button("Remove", GUILayout.Width(80)) && removeIndex >= 0 && removeIndex < level.tubes.Count)
                {
                    level.tubes.RemoveAt(removeIndex);
                    level.emptyTubes = Mathf.Max(0, level.emptyTubes - 1);
                    removeIndex = 0; // Reset after removal
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Locked Tubes", EditorStyles.boldLabel);
            EditLockedTubes(level);
        }

        private void EditTube(TubeColorLevel level, int tubeIndex)
        {
            TubeData tube = level.tubes[tubeIndex];
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Tube {tubeIndex + 1} ({tube.values.Count}/4)", EditorStyles.boldLabel);

            // Display and edit colors
            for (int i = 0; i < 4; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"Slot {i + 1}:", GUILayout.Width(60));
                
                if (i < tube.values.Count)
                {
                    tube.values[i] = EditorGUILayout.IntField(tube.values[i], GUILayout.Width(60));
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        tube.values.RemoveAt(i);
                    }
                }
                else
                {
                    GUILayout.Label("(Empty)", GUILayout.Width(60));
                }
                EditorGUILayout.EndHorizontal();
            }

            // Add color button
            if (tube.values.Count < 4)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Add Color:", GUILayout.Width(80));
                newTubeColorValue = EditorGUILayout.IntField(newTubeColorValue, GUILayout.Width(60));
                if (GUILayout.Button("Add", GUILayout.Width(80)))
                {
                    tube.AddColor(newTubeColorValue);
                }
                EditorGUILayout.EndHorizontal();
            }

            // Clear tube button
            if (tube.values.Count > 0)
            {
                if (GUILayout.Button("Clear Tube", GUILayout.Height(25)))
                {
                    tube.ClearTube();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void EditLockedTubes(TubeColorLevel level)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Lock Tube Index:", GUILayout.Width(120));
            int lockIndex = EditorGUILayout.IntField(0, GUILayout.Width(50));
            if (GUILayout.Button("Lock", GUILayout.Width(80)))
            {
                if (lockIndex >= 0 && lockIndex < level.tubes.Count)
                {
                    level.LockTube(lockIndex);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (level.lockedTubes.Count > 0)
            {
                EditorGUILayout.LabelField($"Locked Tubes: {string.Join(", ", level.lockedTubes)}", EditorStyles.miniLabel);
                if (GUILayout.Button("Clear All Locked Tubes"))
                {
                    level.lockedTubes.Clear();
                }
            }
        }

        private void LoadLevels()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            levels = serializer.LoadLevels(fullPath);
            Debug.Log($"Loaded {levels.Count} levels");
        }

        private void SaveLevels()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            serializer.SaveLevels(fullPath, levels);
            Debug.Log("Levels saved successfully!");
        }

        private void BrowseJsonFile()
        {
            string selectedPath = EditorUtility.OpenFilePanel("Select JSON Level File", Application.dataPath, "json");
            
            if (!string.IsNullOrEmpty(selectedPath))
            {
                // Convert absolute path to relative path from project root
                string projectRoot = Path.Combine(Application.dataPath, "..");
                if (selectedPath.StartsWith(projectRoot))
                {
                    jsonFilePath = selectedPath.Substring(projectRoot.Length + 1).Replace("\\", "/");
                }
                else
                {
                    jsonFilePath = selectedPath;
                }
                
                Debug.Log($"Selected JSON file: {jsonFilePath}");
            }
        }
    }
}
