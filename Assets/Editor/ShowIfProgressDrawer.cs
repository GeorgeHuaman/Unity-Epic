using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfProgressAttribute))]
public class ShowIfProgressDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeProp = property.serializedObject.FindProperty(property.propertyPath.Replace("progressSteps", "type"));
        if (typeProp != null && typeProp.enumValueIndex == (int)Task.TaskType.Progress)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeProp = property.serializedObject.FindProperty(property.propertyPath.Replace("progressSteps", "type"));
        if (typeProp != null && typeProp.enumValueIndex == (int)Task.TaskType.Progress)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        return 0;
    }
}