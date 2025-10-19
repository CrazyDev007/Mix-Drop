using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CrazyDev007.LevelEditor.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
    private VisualElement rootElement;

    // [MenuItem("Window/Level Editor")] - REMOVED
    // public static void ShowWindow()
    // {
    //     LevelEditorWindow window = GetWindow<LevelEditorWindow>("Level Editor");
    //     window.minSize = new Vector2(400, 300);
    // }

    private void OnEnable()
    {
        // DEPRECATED - This window has been removed
        // rootElement = new VisualElement();
        // rootElement.styleSheets.Add(Resources.Load<StyleSheet>("EditorStyles"));
        // var visualTree = Resources.Load<VisualTreeAsset>("LevelEditorWindow");
        // visualTree.CloneTree(rootElement);
        // rootVisualElement.Add(rootElement);
        // InitializeUI();
    }

    private void InitializeUI()
    {
        // DEPRECATED - This window has been removed
    }

    private void OnGUI()
    {
        // DEPRECATED - This window has been removed
    }
    }
}