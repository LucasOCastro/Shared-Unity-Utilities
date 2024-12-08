using System;
using Shared.StateMachines;
using UnityEditor;
using UnityEngine;

namespace Shared.Editor
{
    [CustomEditor(typeof(BaseStateMachineAsset), editorForChildClasses: true)]
    public class StateMachineEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (target is not BaseStateMachineAsset stateMachineAsset)
                return;
            
            if (GUILayout.Button("Open Editor"))
                StateMachineEditorWindow.Open(stateMachineAsset);
        }
    }

    public struct MachineTypeInfo
    {
        public Type MachineType;
        public Type StateType;
        public Type TransitionType;

        public static MachineTypeInfo From(Type assetType)
        {
            if (assetType == null || !assetType.IsSubclassOf(typeof(BaseStateMachineAsset)))
                throw new ArgumentException($"Type must be a subclass of {nameof(BaseStateMachineAsset)}.");
            
            if (assetType.IsConstructedGenericType && assetType.GetGenericTypeDefinition() == typeof(StateMachineAsset<,,>))
            {
                var genericArguments = assetType.GetGenericArguments();
                return new()
                {
                    MachineType = genericArguments[0],
                    StateType = genericArguments[1],
                    TransitionType = genericArguments[2]
                };
            }

            if (assetType == typeof(BaseStateMachineAsset))
            {
                return new()
                {
                    MachineType = typeof(StateMachine),
                    StateType = typeof(IState),
                    TransitionType = typeof(Transition)
                };
            }

            return From(assetType.BaseType);
        }
    }
}