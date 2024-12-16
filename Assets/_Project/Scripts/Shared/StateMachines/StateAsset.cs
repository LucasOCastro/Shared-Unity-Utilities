using System;
using UnityEngine;
using UnityUtils;

namespace Shared.StateMachines
{
    public class StateAsset : ScriptableObject
    {
        [SerializeReference]
        public IState state;
        
        public Vector2 position;
        
        public void SetState(IState newState)
        {
            state = newState;
            name = state.GetType().Name;
        }
        
        public void SetState(Type stateType)
        {
            if (stateType == null || !stateType.IsDerivedTypeOf(typeof(IState)))
                throw new ArgumentException($"Type {stateType} must be a subclass of {nameof(IState)}.");
            
            SetState((IState)Activator.CreateInstance(stateType));
        }
        
        public void SetState<T>() where T : IState => SetState(typeof(T));
    }
}