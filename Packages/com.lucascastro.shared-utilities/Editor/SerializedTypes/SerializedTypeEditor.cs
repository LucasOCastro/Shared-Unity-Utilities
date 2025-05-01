using System;
using System.Linq;
using JetBrains.Annotations;
using SharedUtilities.Editor.Extensions;
using SharedUtilities.Extensions;
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
            var stringProperty = GetUnderlyingProperty(property, fieldInfo.FieldType);
            var element = stringProperty != null ? MakeGUI(stringProperty) : null;
            return element ?? new PropertyField(property);
        }

        [CanBeNull]
        private VisualElement MakeGUI(SerializedProperty stringProperty)
        {
            var attr = (SerializedTypeAttribute)attribute;
            var types = attr.SolveAllowedTypes(fieldInfo);
            if (types != null)
                return MakeTypeDropdown(stringProperty, types);
            
            LogError("No types found for attribute", stringProperty);
            return null;
        }

        private VisualElement MakeTypeDropdown(SerializedProperty stringProperty, Type[] types)
        {
            int currentIndex = types.FindIndex(t => t.AssemblyQualifiedName == stringProperty.stringValue);
            if (!string.IsNullOrEmpty(stringProperty.stringValue) && currentIndex == -1)
            {
                LogError($"Cleared type from {stringProperty.stringValue} as it was not allowed by the attribute.", stringProperty);
                currentIndex = -1;
                stringProperty.stringValue = null;
            }

            var choices = types.Select(t => t.GetDisplayName()).ToList();
            var dropdownField = new DropdownField(choices, currentIndex)
            {
                label = preferredLabel,
                style = { height = EditorGUIUtility.singleLineHeight }
            };
            var clearButton = new Button(() => dropdownField.index = -1) { text = "X" };
            dropdownField.RegisterValueChangedCallback(_ =>
            {
                int newIndex = dropdownField.index;
                string newName = newIndex >= 0 ? types[newIndex].AssemblyQualifiedName : null;
                if (newName != stringProperty.stringValue)
                {
                    stringProperty.stringValue = newName;
                    stringProperty.serializedObject.ApplyModifiedProperties();
                }
                
                // clearButton.style.display = newIndex == -1 ? DisplayStyle.None : DisplayStyle.Flex;
            });

            dropdownField.Add(clearButton);
            return dropdownField;
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
            property.serializedObject.targetObject);
    }
}