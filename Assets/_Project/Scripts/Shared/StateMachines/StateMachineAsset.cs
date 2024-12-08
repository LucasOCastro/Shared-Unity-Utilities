namespace Shared.StateMachines
{
    public abstract class StateMachineAsset<TMachine, TState, TTransition> : BaseStateMachineAsset
        where TMachine : BaseStateMachine<TState, TTransition>, new()
        where TState : class, IState
        where TTransition : class, ITransition<TState>
    {
        public TMachine Construct()
        {
            var machine = new TMachine();
            
            foreach (var transition in transitions)
            {
                TState from = Preconditions.CheckOfType<IState, TState>(transition.from);
                Preconditions.CheckOfType<IState, TState>(transition.to);
                TTransition t = transition.ToTransition<TTransition, TState>();
                machine.AddTransition(from, t);
            }
            
            TState state = Preconditions.CheckOfType<IState, TState>(initialState);
            machine.SetState(state);
            
            return machine;
        }
    }
}