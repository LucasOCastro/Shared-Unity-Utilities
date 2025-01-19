namespace SharedUtilities.StateMachines
{
    public class Transition : BaseTransition<IState>
    {
        public Transition(IState to, IPredicate condition) : base(to, condition)
        {
        }
    }
}