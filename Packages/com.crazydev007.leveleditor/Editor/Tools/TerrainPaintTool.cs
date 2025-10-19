using UnityEditor;
using UnityEngine;

namespace CrazyDev007.LevelEditor.Editor
{
    public class TerrainPaintTool : EditorWindow
    {
    private Texture2D paintTexture;
    private float brushSize = 1.0f;
    private float opacity = 1.0f;

    // [MenuItem("Tools/Terrain Paint Tool")] - REMOVED
    // public static void ShowWindow()
    // {
    //     GetWindow<TerrainPaintTool>("Terrain Paint Tool");
    // }

    private void OnGUI()
    {
        GUILayout.Label("Terrain Paint Tool", EditorStyles.boldLabel);
        
        paintTexture = (Texture2D)EditorGUILayout.ObjectField("Paint Texture", paintTexture, typeof(Texture2D), false);
        brushSize = EditorGUILayout.Slider("Brush Size", brushSize, 0.1f, 10.0f);
        opacity = EditorGUILayout.Slider("Opacity", opacity, 0.0f, 1.0f);

        if (GUILayout.Button("Apply Paint"))
        {
            ApplyPaint();
        }
    }

    private void ApplyPaint()
    {
        // Logic to apply the paint texture to the terrain
        // This will involve modifying the terrain's texture and height based on brush size and opacity
    }
    }
}