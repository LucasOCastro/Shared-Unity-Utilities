using System;
using UnityEngine;

namespace SharedUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializedTypeAttribute : PropertyAttribute
    {
        public Type BaseType { get; set; }
        
        public Type[] Types { get; set; }
        
        public SerializedTypeAttribute(Type baseType) => BaseType = baseType;
        public SerializedTypeAttribute(params Type[] types) => Types = types;
    }
}