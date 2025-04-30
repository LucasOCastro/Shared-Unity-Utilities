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
            var attr = (SerializedTypeAttribute)attribute;
            var stringProperty = GetUnderlyingProperty(property, fieldInfo.FieldType);
            var element = stringProperty != null ? MakeGUI(stringProperty, attr) : null;
            return element ?? new PropertyField(property);
        }

        [CanBeNull]
        private static VisualElement MakeGUI(SerializedProperty stringProperty, SerializedTypeAttribute attribute)
        {
            var types = attribute.Types ?? attribute.BaseType?.GetDerivedTypes();
            if (types != null)
                return MakeTypeDropdown(stringProperty, types);
            
            LogError("No types found for attribute", stringProperty);
            return null;
        }

        private static VisualElement MakeTypeDropdown(SerializedProperty stringProperty, Type[] types)
        {
            int currentIndex = types.FindIndex(t => t.AssemblyQualifiedName == stringProperty.stringValue);
            if (!string.IsNullOrEmpty(stringProperty.stringValue) && currentIndex == -1)
            {
                LogError($"Cleared type from {stringProperty.stringValue} as it was not allowed by the attribute.", stringProperty);
                currentIndex = -1;
                stringProperty.stringValue = null;
            }
            
            DropdownField dropdownField = new();
            dropdownField.choices.AddRange(types.Select(t => t.GetDisplayName()));
            dropdownField.index = currentIndex;
            dropdownField.RegisterCallback<ChangeEvent<int>>(i =>
            {
                if (i.newValue != i.previousValue)
                    stringProperty.stringValue = types[i.newValue].AssemblyQualifiedName;
            });
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