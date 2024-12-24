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
                TTransition t = transition.ToTransition<TTransition, TState>();
                if (transition.from)
                    machine.AddTransition(Preconditions.CheckOfType<TState>(transition.from.state), t);
                else
                    machine.AddAnyTransition(t);
            }
            
            TState state = Preconditions.CheckOfType<TState>(initialState.state);
            machine.SetState(state);
            
            return machine;
        }
    }
}