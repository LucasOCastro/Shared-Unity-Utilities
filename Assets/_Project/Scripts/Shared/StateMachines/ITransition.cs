namespace Shared.StateMachines
{
    public interface ITransition<out TState> where TState : IState
    {
        TState To { get; }
        
        IPredicate Condition { get; }
    }
}