namespace SharedUtilities.StateMachines
{
    public interface ITransition<TState> : IBaseTransition where TState : IState 
    {
        new TState To { get; set; }

        IState IBaseTransition.To
        {
            get => To;
            set
            {
                Preconditions.CheckOfType<TState>(value);
                To = (TState)value;
            }
        }
    }
}