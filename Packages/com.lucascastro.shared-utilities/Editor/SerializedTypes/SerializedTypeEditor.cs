using System;
using JetBrains.Annotations;
using SharedUtilities.Editor.Extensions;
using SharedUtilities.Editor.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using SerializedTypeAttribute = SharedUtilities.Attributes.SerializedTypeAttribute;

namespace SharedUtilities.Editor.SerializedTypes
{
    [CustomPropertyDrawer(typeof(SerializedTypeAttribute))]
    internal class SerializedTypeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // TODO consider making it a autocomplete
            var element = MakeGUI(property);
            return element ?? new PropertyField(property);
        }

        [CanBeNull]
        private VisualElement MakeGUI(SerializedProperty property)
        {
            var fieldType = fieldInfo.FieldType;
            var stringProperty = GetUnderlyingProperty(property, fieldType);
            if (stringProperty == null)
                return null;
            
            var types = GetAllowedTypes(property);
            if (types == null)
                return null;
            
            string label = preferredLabel ?? stringProperty.displayName;
            var dropdown = new TypeDropdown(types, stringProperty) { label = label };
            if (dropdown.index == -1)
                LogError($"Type is not allowed by attribute: {stringProperty.stringValue}", property);
            return dropdown;
        }

        private Type[] GetAllowedTypes(SerializedProperty property)
        {
            var attr = (SerializedTypeAttribute)attribute;
            try
            {
                return attr.SolveAllowedTypes(fieldInfo);
            }
            catch (Exception e)
            {
                LogError(e.Message, property);
                return null;
            }
        }

        [CanBeNull]
        private static SerializedProperty GetUnderlyingProperty(SerializedProperty property, Type fieldType)
        {
            if (fieldType == typeof(string))
                return property;
            if (fieldType == typeof(SerializedType))
                return property.FindBackingFieldPropertyRelative(nameof(SerializedType.AssemblyQualifiedName));

            LogError($"Expected {nameof(SerializedType)} or string but got {fieldType.Name}", property);
            return null;
        }

        private static void LogError(string message, SerializedProperty property) => Debug.LogError(
            $"[{nameof(SerializedTypeAttribute)}] {message} ({property.serializedObject.targetObject.GetType().Name}.{property.propertyPath})",
            property.serializedObject.targetObject
        );
    }
}