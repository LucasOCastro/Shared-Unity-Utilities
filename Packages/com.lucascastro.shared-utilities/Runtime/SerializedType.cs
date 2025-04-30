using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SharedUtilities
{
    [Serializable]
    public struct SerializedType
    {
        [field: SerializeField]
        public string AssemblyQualifiedName { get; private set; }

        private Type _type;
        [CanBeNull]
        public Type Type
        {
            get
            {
                if (string.IsNullOrEmpty(AssemblyQualifiedName))
                    _type = null;
                else if (_type == null || _type.AssemblyQualifiedName != AssemblyQualifiedName)
                    _type = Type.GetType(AssemblyQualifiedName);
                
                return _type;
            }
            set
            {
                _type = value;
                AssemblyQualifiedName = value?.AssemblyQualifiedName;
            }
        }
        
        public SerializedType([CanBeNull] Type type)
        {
            AssemblyQualifiedName = type?.AssemblyQualifiedName;
            _type = type;
        }
        
        [CanBeNull]
        public static implicit operator Type(SerializedType serializedType) => serializedType.Type;
        public static implicit operator SerializedType(Type type) => new(type);
    }
}