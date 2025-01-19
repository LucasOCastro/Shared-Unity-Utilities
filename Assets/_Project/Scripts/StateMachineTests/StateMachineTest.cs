using SharedUtilities.StateMachines;
using UnityEngine;

namespace SharedUtilitiesPackages.Game
{
    public class StateMachineTest : MonoBehaviour
    {
        private class TestState : IState
        {
            public string Name { get; set; }
            public TestState(string name) => Name = name;
            
            public void OnEnter()
            {
                Debug.Log($"Entered {Name}");
            }

            public void Update()
            {
                if (Input.GetKey(KeyCode.D))
                    Debug.Log($"Pressed D in {Name}");
            }

            public void FixedUpdate()
            {
            }

            public void OnExit()
            {
                Debug.Log($"Exited {Name}");
            }
        }
        
        
        private StateMachine _stateMachine;
        
        private void Start()
        {
            var a = new TestState("A");
            var b = new TestState("B");
            var c = new TestState("C");
            var d = new TestState("D");
            
            var fsm = new StateMachine(a);
            
            fsm.AddTransition(a, b, () => Input.GetKeyDown(KeyCode.Alpha1));
            fsm.AddTransition(b, c, () => Input.GetKeyDown(KeyCode.Alpha2));
            fsm.AddTransition(c, a, () => Input.GetKeyDown(KeyCode.Alpha3));
            fsm.AddAnyTransition(d, () => Input.GetKeyDown(KeyCode.Space));

            _stateMachine = fsm;
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
    }
}