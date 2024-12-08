namespace Shared.StateMachines
{
    public class Transition : ITransition<IState>
    {
        public IState To { get; }
        
        public IPredicate Condition { get; }
        
        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}