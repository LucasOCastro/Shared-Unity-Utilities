using UnityEditor;

namespace SharedUtilities.Extensions
{
    public static class SerializedObjectUtils
    {
        public static SerializedProperty FindBackingFieldProperty(this SerializedObject obj, string propertyName)
        {
            string backingFieldName = FieldUtils.GetBackingFieldName(propertyName);
            return obj.FindProperty(backingFieldName);
        }
    }
}