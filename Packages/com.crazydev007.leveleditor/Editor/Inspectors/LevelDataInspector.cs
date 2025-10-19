using UnityEditor;
using UnityEngine;
using CrazyDev007.LevelEditor;

namespace CrazyDev007.LevelEditor.Editor
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataInspector : UnityEditor.Editor
    {
    public override void OnInspectorGUI()
    {
        LevelData levelData = (LevelData)target;

        // Draw default inspector
        DrawDefaultInspector();

        // Custom GUI elements for LevelData
        EditorGUILayout.LabelField("Level Properties", EditorStyles.boldLabel);
        levelData.levelName = EditorGUILayout.TextField("Level Name", levelData.levelName);
        levelData.difficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty", levelData.difficulty);

        // Add more properties as needed
        if (GUILayout.Button("Save Level Data"))
        {
            // Implement save functionality
        }

        // Mark the object as dirty to save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(levelData);
        }
    }
    }
}