using TFF.Core.Variables.Optional;
using UnityEditor;
using UnityEngine;

namespace TFF.Core.Editor.PropertyDrawers.Variables.Optional
{
    [CustomPropertyDrawer(typeof(SOptional<>))]
    public class SOptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            SerializedProperty enabledProperty = property.FindPropertyRelative("enabled");

            EditorGUI.BeginProperty(position, label, property);
                // Adds value part of the property field
                position.width -= 24;
                EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
                    EditorGUI.PropertyField(position, valueProperty, label, true);
                EditorGUI.EndDisabledGroup();

                // Adds check box
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                position.x += position.width + 24;
                position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
                position.x -= position.width;
                EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
                EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}