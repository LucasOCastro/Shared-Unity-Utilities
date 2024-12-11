namespace Shared.StateMachines
{
    public interface IState
    {
        string Name { get; set; }
        
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();
    }
}