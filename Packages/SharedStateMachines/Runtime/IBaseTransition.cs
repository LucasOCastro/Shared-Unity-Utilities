namespace SharedUtilities.StateMachines
{
    public interface IBaseTransition
    {
        IState To { get; set; }
        
        IPredicate Condition { get; set; }
    }
}