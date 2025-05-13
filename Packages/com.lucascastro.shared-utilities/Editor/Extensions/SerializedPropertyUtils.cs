using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace SharedUtilities.Editor.Extensions
{
    public static class SerializedPropertyUtils
    {
        public static SerializedProperty FindBackingFieldPropertyRelative(this SerializedProperty property,
            string propertyPath)

        {
            string propertyName = propertyPath.Split('.').Last();
            string backingFieldName = FieldUtils.GetBackingFieldName(propertyName);
            return property.FindPropertyRelative(backingFieldName);
        }
        
        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/UIElements/Controls/PropertyField.cs
        public static IEnumerable<VisualElement> GetFieldSubElements(this SerializedProperty property, bool bind = false)
        {
            Assert.IsNotNull(property);
            
            var endProperty = property.GetEndProperty();
            property.NextVisible(true); // Expand the first child.
            do
            {
                if (SerializedProperty.EqualContents(property, endProperty))
                    break;

                string propPath = property.propertyPath;
                var field = new PropertyField(property)
                {
                    bindingPath = propPath,
                    name = "unity-property-field-" + propPath
                };

                if (bind)
                    field.Bind(property.serializedObject);

                yield return field;
            }
            while (property.NextVisible(false)); // Never expand children.
        }
    }
}