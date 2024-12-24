using UnityEngine;

namespace Shared.StateMachines
{
    public class TransitionAsset : ScriptableObject
    {
        public StateAsset from;
        
        public StateAsset to;
        
        public string transitionType;
        
        [SerializeReference]
        public IBaseTransition transition;

        public TTransition ToTransition<TTransition, TState>()
            where TTransition : ITransition<TState>
            where TState : IState
        {
            return Preconditions.CheckOfType<TTransition>(transition);
        }
    }
}