using UnityEditor;
using UnityEngine;
using CrazyDev007.LevelEditor;

namespace CrazyDev007.LevelEditor.Editor
{
    [CustomEditor(typeof(PlaceableObject))]
    public class ObjectPlacementInspector : UnityEditor.Editor
    {
    public override void OnInspectorGUI()
    {
        PlaceableObject placeableObject = (PlaceableObject)target;

        // Draw default inspector
        DrawDefaultInspector();

        // Custom inspector fields for object placement settings
        EditorGUILayout.LabelField("Object Placement Settings", EditorStyles.boldLabel);
        
        placeableObject.snapToGrid = EditorGUILayout.Toggle("Snap to Grid", placeableObject.snapToGrid);
        placeableObject.gridSize = EditorGUILayout.FloatField("Grid Size", placeableObject.gridSize);

        // Apply changes to the serialized object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(placeableObject);
        }
    }
    }
}