using System;
using JetBrains.Annotations;
using SharedUtilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SharedUtilities.Interfaces
{
    [Serializable]
    public class InterfaceReference<TInterface, TObject> where TInterface : class where TObject : Object
    {
        [field: SerializeField] [CanBeNull]
        public TObject UnderlyingValue { get; set; }

        [CanBeNull]
        public TInterface Value
        {
            get => UnderlyingValue.OrNull() switch
            {
                null => null,
                TInterface interfaceValue => interfaceValue,
                _ => throw new InvalidCastException($"Underlying value {UnderlyingValue} of type {typeof(TObject).Name} needs to implement {typeof(TInterface).Name}")
            };

            set => UnderlyingValue = value.OrNull() switch
            {
                null => null,
                TObject objectValue => objectValue,
                _ => throw new InvalidCastException($"Value {value} of type {typeof(TInterface).Name} needs to be of type {typeof(TObject).Name}")
            };
        }

        public InterfaceReference()
        {
        }
        
        public InterfaceReference(TObject underlyingValue) => UnderlyingValue = underlyingValue;
        
        public InterfaceReference(TInterface interfaceValue) => Value = interfaceValue;
    }
    
    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class { }
}