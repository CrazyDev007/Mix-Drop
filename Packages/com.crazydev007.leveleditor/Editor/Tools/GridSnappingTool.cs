using UnityEditor;
using UnityEngine;

namespace CrazyDev007.LevelEditor.Editor
{
    public class GridSnappingTool
    {
    private float gridSize = 1.0f;

    public void SetGridSize(float size)
    {
        gridSize = size;
    }

    public Vector3 SnapToGrid(Vector3 position)
    {
        position.x = Mathf.Round(position.x / gridSize) * gridSize;
        position.y = Mathf.Round(position.y / gridSize) * gridSize;
        position.z = Mathf.Round(position.z / gridSize) * gridSize;
        return position;
    }
    }
}