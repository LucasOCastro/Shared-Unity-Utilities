using System;
using UnityEngine;
using UnityUtils;

namespace Shared.StateMachines
{
    public class StateAsset : ScriptableObject
    {
        [SerializeReference]
        public IEditableState state;
        
        public Vector2 position;
        
        public void SetState(IEditableState newState)
        {
            state = newState;
            name = state.GetType().Name;
        }
        
        public void SetState(Type stateType)
        {
            if (stateType == null || !stateType.IsDerivedTypeOf(typeof(IEditableState)))
                throw new ArgumentException($"Type {stateType} must be a subclass of {nameof(IEditableState)}.");
            
            SetState((IEditableState)Activator.CreateInstance(stateType));
        }
        
        public void SetState<T>() where T : IEditableState => SetState(typeof(T));
    }
}