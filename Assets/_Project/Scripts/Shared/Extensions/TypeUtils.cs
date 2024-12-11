using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions
{
    public static class TypeUtils
    {
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
    }
}