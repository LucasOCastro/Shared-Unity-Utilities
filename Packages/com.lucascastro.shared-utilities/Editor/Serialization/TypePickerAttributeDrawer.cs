using System;
using System.Linq;
using JetBrains.Annotations;
using SharedUtilities.Editor.VisualElements;
using SharedUtilities.Extensions;
using SharedUtilities.Serialization;
using SharedUtilities.Serialization.Attributes;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SharedUtilities.Editor.Serialization
{
    [CustomPropertyDrawer(typeof(TypePickerAttribute))]
    public class TypePickerAttributeDrawer : PropertyDrawer
    {
        private TypePickerAttribute Attribute => (TypePickerAttribute)attribute;
        
        [CanBeNull]
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var types = GetAllowedTypes();
            if (!PropertyHasValidType(property, types))
            {
                property.managedReferenceValue = GetDefaultValue(property, types);
                property.serializedObject.ApplyModifiedProperties();
            }

            return new ObjectPropertyFieldWithTypePicker(property, types)
            {
                TypeDropdown = { ShowClearButton = Attribute.AllowNull }
            };
        }

        private bool PropertyHasValidType(SerializedProperty property, Type[] types)
        {
            if (property.managedReferenceValue == null)
                return Attribute.AllowNull;
            
            int existingIndex = types.IndexOf(property.managedReferenceValue.GetType());
            if (existingIndex >= 0) 
                return true;
            
            LogError($"Type {property.managedReferenceValue.GetType().Name} is not allowed.", property);
            return false;
        }

        private object GetDefaultValue(SerializedProperty property, Type[] types)
        {
            if (Attribute.AllowNull) 
                return null;

            if (types.Length > 0) 
                return Activator.CreateInstance(types[0]);
            
            LogError("Null is not allowed but no valid types were found.", property);
            return null;
        }
        
        
        private Type[] GetAllowedTypes()
        {
            return (Attribute.AllowedTypes ?? fieldInfo.FieldType.GetDerivedTypes())
                .Where(t => !t.IsAbstract && 
                            !t.IsGenericTypeDefinition && 
                            t.HasDefaultConstructor() &&
                            !typeof(Object).IsAssignableFrom(t) &&
                            t.IsSerializable)
                .ToArray();
        }

        private static void LogError(string message, SerializedProperty property) =>
            AttributeDrawerUtils.LogAttributeDrawerError<TypePickerAttribute>(message, property);
    }
}