using UnityEditor;
using UnityEngine;

namespace SharedUtilities.Editor
{
    public static class AttributeDrawerUtils
    {
        private static string GetPath(SerializedProperty property) => 
            $"{property.serializedObject.targetObject.GetType().Name}.{property.propertyPath}";
        
        public static void LogAttributeDrawerError<TAttribute>(string message, SerializedProperty property) =>
            Debug.LogError($"[{typeof(TAttribute).Name}] {message} ({GetPath(property)})");
    }
}