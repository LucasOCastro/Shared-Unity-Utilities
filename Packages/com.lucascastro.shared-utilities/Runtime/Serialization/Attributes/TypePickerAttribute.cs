using System;
using UnityEngine;

namespace SharedUtilities.Serialization.Attributes
{
    public class TypePickerAttribute : PropertyAttribute 
    {
        public Type[] AllowedTypes { get; }

        public bool AllowNull { get; set; } = true;
        
        public TypePickerAttribute() {}
        
        public TypePickerAttribute(params Type[] types) => AllowedTypes = types;
    }
}