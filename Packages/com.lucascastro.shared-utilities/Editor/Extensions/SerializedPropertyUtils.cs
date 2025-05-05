using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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
        
        public static IEnumerable<VisualElement> GetFieldSubElements(this SerializedProperty property, bool bind = false)
        {
            object obj = property.managedReferenceValue;
            if (obj is Object o)
            {
                yield return new InspectorElement(o);
                yield break;
            }
            
            var prop = property.Copy();
            while (prop.NextVisible(true) && prop.depth > property.depth)
            {
                var field = new PropertyField(prop.Copy());
                if (bind)
                    field.BindProperty(prop);
                yield return field;
            }
        }
    }
}