using UnityEditor;
using UnityEngine;

namespace CrazyDev007.LevelEditor.Editor
{
    [CustomPropertyDrawer(typeof(TubeColorLevel))]
    public class TubeColorLevelDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, label) * 10;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty levelProp = property.FindPropertyRelative("level");
            SerializedProperty tubesProp = property.FindPropertyRelative("tubes");
            SerializedProperty emptyTubesProp = property.FindPropertyRelative("emptyTubes");
            SerializedProperty maxMovesProp = property.FindPropertyRelative("maxMoves");

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, levelProp);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            EditorGUI.PropertyField(position, tubesProp, true);
            position.y += EditorGUI.GetPropertyHeight(tubesProp) + 2;
            
            EditorGUI.PropertyField(position, emptyTubesProp);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            EditorGUI.PropertyField(position, maxMovesProp);
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }
    }
}
