using Shared.StateMachines;
using UnityEngine;

namespace SpaceWizardry.Game
{
    public abstract class MyStateBase : IState
    {
        [field: SerializeField]
        public string Name { get; set; }
        
        public virtual void OnEnter()
        {
            Debug.Log($"Entered {Name}");
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void OnExit()
        {
            Debug.Log($"Exited {Name}");
        }
    }
    
    public class LogState : MyStateBase
    {
        public override void Update()
        {
            if (Input.GetKey(KeyCode.K))
                Debug.Log($"Update - Pressed K in {Name}");
        }

        public override void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.K))
                Debug.Log($"FixedUpdate - Pressed K in {Name}");
        }
    }
    
    public class MyStateMachine : BaseStateMachine<MyStateBase, BaseTransition<MyStateBase>>{}

    public class StateMachineAssetTest : MonoBehaviour
    {
        [SerializeField] private TestStateMachineAsset stateMachineAsset;
    }
}