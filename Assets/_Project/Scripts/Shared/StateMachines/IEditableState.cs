namespace Shared.StateMachines
{
    public interface IEditableState : IState
    {
        bool CanBeRoot { get; }
        
        int InputCount { get; }
        int OutputCount { get; }
        
        bool IsVertical { get; }
    }
}