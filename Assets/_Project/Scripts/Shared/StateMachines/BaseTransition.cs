namespace Shared.StateMachines
{
    public class BaseTransition<TState> : ITransition<TState> where TState : IState
    {
        public TState To { get; }
        
        public IPredicate Condition { get; }
        
        public BaseTransition(TState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}