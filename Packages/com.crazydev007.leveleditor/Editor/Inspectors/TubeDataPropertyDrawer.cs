using UnityEditor;
using UnityEngine;

namespace CrazyDev007.LevelEditor.Editor
{
    [CustomPropertyDrawer(typeof(TubeData))]
    public class TubeDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, label) * 6;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty valuesProp = property.FindPropertyRelative("values");
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, valuesProp, true);
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }
    }
}
