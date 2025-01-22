using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedUtilities.Extensions
{
    // Some from https://github.com/adammyhre/Unity-Utils - thanks Git-Amend!!
    public static class TypeUtils
    {
        private static readonly Dictionary<Type, string> _typeDisplayNames = new()
        {
            { typeof(int), "int" },
            { typeof(float), "float" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(string), "string" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(uint), "uint" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(char), "char" },
            { typeof(object), "object" }
        };

        private static readonly Type[] _valueTupleTypes =
        {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
        };
        
        public static IEnumerable<Type> DirectlyImplementedInterfaces(this Type type) =>
            type.GetInterfaces().Where(i => type.GetInterfaceMap(i).TargetType == type);
        
        public static bool DirectlyImplementsInterface(this Type type, Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"{interfaceType} is not an interface", nameof(interfaceType));
            
            if (type == interfaceType)
                return true;

            return type.DirectlyImplementedInterfaces().Contains(interfaceType);
        }
        
        /// <summary>
        /// Determines if the type is a generic type of the given non-generic type.
        /// </summary>
        /// <param name="genericType">The Type to be used</param>
        /// <param name="nonGenericType">The non-generic type to test against.</param>
        /// <returns>If the type is a generic type of the non-generic type.</returns>
        public static bool IsGenericTypeOf(this Type genericType, Type nonGenericType)
        {
            return genericType.IsGenericType && genericType.GetGenericTypeDefinition() == nonGenericType;
        }

        /// <summary>
        /// Determines if the type is a derived type of the given base type.
        /// </summary>
        /// <param name="type">this type</param>
        /// <param name="baseType">The base type to test against.</param>
        /// <returns>If the type is a derived type of the base type.</returns>
        public static bool IsDerivedTypeOf(this Type type, Type baseType) => baseType.IsAssignableFrom(type);
        
        /// <summary>
        /// Gets a formatted display name for a given type.
        /// </summary>
        /// <param name="type">The type to generate a display name for.</param>
        /// <param name="includeNamespace">If the namespace should be included when generating the typename.</param>
        /// <returns>The generated display name.</returns>
        public static string GetDisplayName(this Type type, bool includeNamespace = false) {
            if (type.IsGenericParameter)
                return type.Name;

            if (type.IsArray) 
            {
                int rank = type.GetArrayRank();
                string innerTypeName = GetDisplayName(type.GetElementType(), includeNamespace);
                return $"{innerTypeName}[{new string(',', rank - 1)}]";
            }
            
            if (_typeDisplayNames.TryGetValue(type, out string baseName1)) 
            {
                if (!type.IsGenericType || type.IsConstructedGenericType)
                    return baseName1;
                
                Type[] genericArgs = type.GetGenericArguments();
                return $"{baseName1}<{new string(',', genericArgs.Length - 1)}>";
            }

            if (type.IsGenericTypeOf(typeof(Nullable<>))) 
            {
                var innerType = type.GetGenericArguments()[0];
                return $"{innerType.GetDisplayName()}?";
            }

            if (type.IsGenericType) 
            {
                var baseType = type.GetGenericTypeDefinition();
                Type[] genericArgs = type.GetGenericArguments();

                if (_valueTupleTypes.Contains(baseType))
                {
                    var tupleNames = type.GetGenericArguments()
                        .Select(t => GetDisplayName(t));
                    return $"({string.Join(", ", tupleNames)})";
                }

                if (type.IsConstructedGenericType)
                {
                    var genericNames = new string[genericArgs.Length];
                    for (var i = 0; i < genericArgs.Length; i++) {
                        genericNames[i] = GetDisplayName(genericArgs[i], includeNamespace);
                    }

                    string baseName = GetDisplayName(baseType, includeNamespace).Split('<')[0];
                    return $"{baseName}<{string.Join(", ", genericNames)}>";
                }

                string typeName = includeNamespace
                    ? type.FullName
                    : type.Name;

                return $"{typeName?.Split('`')[0]}<{new string(',', genericArgs.Length - 1)}>";
            }

            var declaringType = type.DeclaringType;
            if (declaringType == null)
            {
                return includeNamespace
                    ? type.FullName
                    : type.Name;
            }

            string declaringName = GetDisplayName(declaringType, includeNamespace);
            return $"{declaringName}.{type.Name}";
        }
        
        public static bool IsGenericTypeWithDefinition(this Type type, Type genericTypeDefinition) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
    }
}