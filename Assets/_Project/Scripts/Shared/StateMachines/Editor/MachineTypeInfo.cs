using System;
using UnityUtils;

namespace Shared.StateMachines.Editor
{
    public struct MachineTypeInfo
    {
        public Type MachineType;
        public Type StateType;
        public Type TransitionType;

        public static MachineTypeInfo From(Type assetType)
        {
            if (!assetType.IsDerivedTypeOf(typeof(BaseStateMachineAsset)))
                throw new ArgumentException($"Type must be a subclass of {nameof(BaseStateMachineAsset)}.");
                
            if (assetType == null)
                throw new ArgumentNullException(nameof(assetType));

            Type type = assetType;
            while (type != null)
            {
                if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(StateMachineAsset<,,>))
                {
                    var genericArguments = type.GetGenericArguments();
                    return new()
                    {
                        MachineType = genericArguments[0],
                        StateType = genericArguments[1],
                        TransitionType = genericArguments[2]
                    };
                }
                
                if (type == typeof(BaseStateMachineAsset))
                {
                    return new()
                    {
                        MachineType = typeof(StateMachine),
                        StateType = typeof(IState),
                        TransitionType = typeof(Transition)
                    };
                }
                
                type = type.BaseType;
            }
            
            throw new ArgumentException($"Type must be a subclass of {nameof(BaseStateMachineAsset)}.");
        }
    }
}