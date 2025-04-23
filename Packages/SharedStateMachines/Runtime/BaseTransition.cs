namespace SharedUtilities.StateMachines
{
    public class BaseTransition<TState> : ITransition<TState> where TState : IState
    {
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