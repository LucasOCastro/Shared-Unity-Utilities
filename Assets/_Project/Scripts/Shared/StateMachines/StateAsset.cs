using System;
using UnityEngine;

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
        
        public void SetState(Type stateType) => SetState(Activator.CreateInstance(stateType) as IState);
        
        public void SetState<T>() where T : IState => SetState(typeof(T));
    }
}