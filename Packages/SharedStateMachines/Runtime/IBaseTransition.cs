namespace SharedUtilities.StateMachines
{
    public interface IBaseTransition
    {
        IState To { get; }
        
        IPredicate Condition { get; }
    }
}