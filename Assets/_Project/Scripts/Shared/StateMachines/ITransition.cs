namespace Shared.StateMachines
{
    public interface IBaseTransition<out TState> where TState : IState
    {
        TState To { get; }
        
        IPredicate Condition { get; }
    }

    public class BaseTransition<TState> : IBaseTransition<TState> where TState : IState
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