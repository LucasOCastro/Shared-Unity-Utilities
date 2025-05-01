using System;
using UnityEngine;

namespace SharedUtilities.Serialization.Attributes
{
    public class RequiresInterfaceAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; }

        public RequiresInterfaceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}