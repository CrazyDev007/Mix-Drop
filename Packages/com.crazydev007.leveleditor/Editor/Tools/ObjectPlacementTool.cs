using UnityEditor;
using UnityEngine;

namespace CrazyDev007.LevelEditor.Editor
{
    public class ObjectPlacementTool : EditorWindow
    {
    private GameObject objectToPlace;
    private bool isPlacing = false;

    // [MenuItem("Tools/Level Editor/Object Placement Tool")] - REMOVED
    // public static void ShowWindow()
    // {
    //     GetWindow<ObjectPlacementTool>("Object Placement Tool");
    // }

    private void OnGUI()
    {
        GUILayout.Label("Object Placement Tool", EditorStyles.boldLabel);
        objectToPlace = (GameObject)EditorGUILayout.ObjectField("Object to Place", objectToPlace, typeof(GameObject), true);

        if (GUILayout.Button("Start Placing"))
        {
            isPlacing = true;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        if (GUILayout.Button("Stop Placing"))
        {
            isPlacing = false;
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacing || objectToPlace == null) return;

        Event e = Event.current;

        if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Handles.DrawWireDisc(hitPoint, Vector3.up, 0.5f);
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    PlaceObject(hitPoint);
                    e.Use();
                }
            }
        }
    }

    private void PlaceObject(Vector3 position)
    {
        GameObject.Instantiate(objectToPlace, position, Quaternion.identity);
    }
    }
}