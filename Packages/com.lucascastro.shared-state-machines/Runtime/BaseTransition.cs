using System;

namespace SharedUtilities.StateMachines
{
    [Serializable]
    public class BaseTransition<TState> : ITransition<TState> where TState : IState
    {
        public string test;
        
        public TState To { get; set; }
        
        public IPredicate Condition { get; set; }

        public BaseTransition()
        {
        }
        
        public BaseTransition(TState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}