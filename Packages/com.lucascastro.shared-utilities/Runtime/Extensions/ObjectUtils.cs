using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SharedUtilities.Extensions
{
    public static class ObjectUtils
    {
        [CanBeNull]
        public static T OrNull<T>([CanBeNull] this T obj) where T : class
        {
            if (ReferenceEquals(obj, null) || obj.Equals(null))
                return null;
            return obj;
        }
        
        /// <summary>
        ///     Instantiates an object of the given type. <br/>
        ///     If the type is a ScriptableObject, it will be created as a new asset. <br/>
        ///     If the type is a MonoBehaviour, it will be added to a new GameObject.
        /// </summary>
        /// <param name="type">The type of object to instantiate.</param>
        /// <returns>The instantiated object.</returns>
        public static object InstantiateObject(Type type)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type))
                return ScriptableObject.CreateInstance(type);
            
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
                return new GameObject().AddComponent(type);
            
            return Activator.CreateInstance(type);
        }
    }
}