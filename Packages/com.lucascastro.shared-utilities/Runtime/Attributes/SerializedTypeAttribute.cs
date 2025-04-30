using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SharedUtilities.Extensions;
using UnityEngine;

namespace SharedUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializedTypeAttribute : PropertyAttribute
    {
        public Type BaseType { get; }
        
        public Type[] Types { get; }
        
        public string FilterMethodName { get; }
        
        public SerializedTypeAttribute(Type baseType) => BaseType = baseType;
        public SerializedTypeAttribute(params Type[] types) => Types = types;
        public SerializedTypeAttribute(Type baseType, string filterMethodName) => (BaseType, FilterMethodName) = (baseType, filterMethodName);

        [CanBeNull]
        public Type[] SolveAllowedTypes(FieldInfo fieldInfo)
        {
            if (Types != null) 
                return Types;

            if (BaseType == null)
                throw new InvalidOperationException($"{nameof(SerializedTypeAttribute)} must have either {nameof(BaseType)} or {nameof(Types)} set.");

            var types = BaseType.GetDerivedTypes();
            if (FilterMethodName == null) 
                return types;
            
            var method = fieldInfo.DeclaringType?.GetMethod(FilterMethodName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                Debug.LogError($"Could not find static method {FilterMethodName} on {fieldInfo.DeclaringType?.Name}");
                return null;
            }
            
            object[] methodParams = new object[1];
            return types.Where(t =>
            {
                methodParams[0] = t;
                return (bool)method.Invoke(null, methodParams);
            }).ToArray();
        }
    }
}