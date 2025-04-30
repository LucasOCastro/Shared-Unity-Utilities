using System;
using UnityEngine;

namespace SharedUtilities.Interfaces
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