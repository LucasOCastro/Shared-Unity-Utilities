using System;
using System.Linq;
using JetBrains.Annotations;
using SharedUtilities.Editor.VisualElements;
using SharedUtilities.Extensions;
using SharedUtilities.Serialization;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SharedUtilities.Editor.Serialization
{
    [CustomPropertyDrawer(typeof(TypePickerAttribute))]
    public class TypePickerAttributeDrawer : PropertyDrawer
    {
        [CanBeNull]
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var types = GetAllowedTypes();
            if (PropertyHasValidType(property, types)) 
                return new ObjectPropertyFieldWithTypePicker(property, types);
            
            LogError($"Type {property.managedReferenceValue.GetType().Name} is not allowed.", property);
            property.managedReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
            return null;

        }

        private static bool PropertyHasValidType(SerializedProperty property, Type[] types)
        {
            if (property.managedReferenceValue == null)
                return true;
            
            int existingIndex = types.IndexOf(property.managedReferenceValue.GetType());
            return existingIndex >= 0;
        }
        
        private Type[] GetAllowedTypes()
        {
            var attr = (TypePickerAttribute)attribute;
            return (attr.AllowedTypes ?? fieldInfo.FieldType.GetDerivedTypes())
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