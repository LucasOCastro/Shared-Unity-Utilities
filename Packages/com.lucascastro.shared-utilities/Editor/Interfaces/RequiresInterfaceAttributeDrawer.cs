using SharedUtilities.Interfaces;
using UnityEditor;
using UnityEngine;

namespace SharedUtilities.Editor.Interfaces
{
    [CustomPropertyDrawer(typeof(RequiresInterfaceAttribute))]
    public class RequiresInterfaceAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (RequiresInterfaceAttribute)attribute;
            var interfaceType = attr.InterfaceType;
            var objectType = fieldInfo.FieldType;
            if (interfaceType == null)
            {
                Debug.LogError($"Could not get {nameof(RequiresInterfaceAttribute)} type arguments for {property.name}", property.serializedObject.targetObject);
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            if (!typeof(Object).IsAssignableFrom(objectType))
            {
                Debug.LogError($"{property.name} must be of type {nameof(Object)}", property.serializedObject.targetObject);
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            EditorGUI.BeginProperty(position, label, property);
            
            InterfaceReferenceUtils.DrawProperty(position, property, label, objectType, interfaceType);

            EditorGUI.EndProperty();
        }
    }
}