using System;
using System.Reflection;
using UnityEngine;

namespace Shared.StateMachines
{
    public class TransitionAsset : ScriptableObject
    {
        [SerializeReference]
        public StateAsset from;
        
        [SerializeReference]
        public StateAsset to;
        
        [SerializeReference]
        public IPredicate condition;
        
        public TTransition ToTransition<TTransition, TState>() 
            where TTransition: ITransition<TState>
            where TState: IState
        {
            ConstructorInfo constructor = typeof(TTransition).GetConstructor(new[] {typeof(TState), typeof(IPredicate)});
            if (constructor == null)
                throw new ArgumentException($"Type {typeof(TTransition).Name} does not have a constructor that accepts {typeof(TState).Name} and {nameof(IPredicate)}.");
            
            Preconditions.CheckOfType<IState, TState>(from.state);
            var toChecked = Preconditions.CheckOfType<IState, TState>(to.state);
            return (TTransition)constructor.Invoke(new object[] {toChecked, condition});
        }
    }
}