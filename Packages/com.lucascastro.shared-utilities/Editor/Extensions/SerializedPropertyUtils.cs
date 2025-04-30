using System.Linq;
using SharedUtilities.Extensions;
using UnityEditor;

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
    }
}