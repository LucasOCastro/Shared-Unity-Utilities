using System;
using UnityEngine;

namespace SharedUtilities.Serialization
{
    public class TypePickerAttribute : PropertyAttribute 
    {
        public Type[] AllowedTypes { get; set; }
        
        public TypePickerAttribute() {}
        
        public TypePickerAttribute(params Type[] types) => AllowedTypes = types;
    }
}