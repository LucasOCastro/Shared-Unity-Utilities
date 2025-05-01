using System;
using System.Linq;
using System.Reflection;
using SharedUtilities.Extensions;
using UnityEngine;

namespace SharedUtilities.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializedTypeAttribute : PropertyAttribute
    {
        private const BindingFlags MethodBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public Type BaseType { get; }
        
        public Type[] Types { get; }
        
        public string FilterMethodName { get; }
        
        public SerializedTypeAttribute(Type baseType) => BaseType = baseType;
        public SerializedTypeAttribute(params Type[] types) => Types = types;
        public SerializedTypeAttribute(Type baseType, string filterMethodName) => (BaseType, FilterMethodName) = (baseType, filterMethodName);

        private MethodInfo GetMethod(FieldInfo fieldInfo)
        {
            var declaringType = fieldInfo.DeclaringType;
            if (declaringType == null)
                throw new NullReferenceException();
                    
            var method = declaringType.GetMethod(FilterMethodName, MethodBindingFlags);
            if (method == null)
                throw new MissingMethodException(declaringType.Name, FilterMethodName);
            
            return method;
        }

        public Type[] SolveAllowedTypes(FieldInfo fieldInfo)
        {
            if (Types != null) 
                return Types;

            if (BaseType == null)
                throw new InvalidOperationException($"{nameof(SerializedTypeAttribute)} must have either {nameof(BaseType)} or {nameof(Types)} set.");

            var types = BaseType.GetDerivedTypes();
            if (FilterMethodName == null) 
                return types;

            var method = GetMethod(fieldInfo);
            object[] methodParams = new object[1];
            return types.Where(t =>
            {
                methodParams[0] = t;
                return (bool)method.Invoke(null, methodParams);
            }).ToArray();
        }
    }
}