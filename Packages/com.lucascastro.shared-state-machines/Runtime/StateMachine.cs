using System;

namespace SharedUtilities.StateMachines
{
    public class StateMachine : BaseStateMachine<IState, Transition>
    {
        public StateMachine(IState initialState) : base(initialState)
        {
        }

        public void AddTransition(IState from, IState to, IPredicate condition) =>
            base.AddTransition(from, new(to, condition));

        public void AddTransition(IState from, IState to, Func<bool> condition) =>
            AddTransition(from, to, new FuncPredicate(condition));
        
        public void AddAnyTransition(IState to, IPredicate condition) => 
            base.AddAnyTransition(new(to, condition));
        
        public void AddAnyTransition(IState to, Func<bool> condition) => 
            AddAnyTransition(to, new FuncPredicate(condition));
    }
}