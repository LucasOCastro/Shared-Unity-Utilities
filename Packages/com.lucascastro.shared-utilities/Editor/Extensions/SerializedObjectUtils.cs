using System.Linq;
using SharedUtilities.Extensions;
using UnityEditor;

namespace SharedUtilities.Editor.Extensions
{
    public static class SerializedObjectUtils
    {
        public static SerializedProperty FindBackingFieldProperty(this SerializedObject obj, string propertyPath)
        {
            string propertyName = propertyPath.Split('.').Last();
            string backingFieldName = FieldUtils.GetBackingFieldName(propertyName);
            return obj.FindProperty(backingFieldName);
        }
    }
}