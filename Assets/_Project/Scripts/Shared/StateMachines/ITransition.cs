namespace Shared.StateMachines
{
    public interface ITransition<out TState> : IBaseTransition where TState : IState 
    {
        new TState To { get; }
        
        IState IBaseTransition.To => To;
    }
}