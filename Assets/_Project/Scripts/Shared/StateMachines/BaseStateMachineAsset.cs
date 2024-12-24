using System.Collections.Generic;
using UnityEngine;

namespace Shared.StateMachines
{
    [CreateAssetMenu(fileName = "New State Machine", menuName = "StateMachines/State Machine")]
    public class BaseStateMachineAsset : ScriptableObject
    {
        public Vector2 anyNodePosition;
        
        public StateAsset initialState;
        
        public List<StateAsset> states = new();
        
        public List<TransitionAsset> transitions = new();

        public StateMachine GenericConstruct()
        {
            var machine = new StateMachine(initialState.state);
            
            foreach (var transition in transitions)
            {
                var checkedTransition = Preconditions.CheckOfType<Transition>(transition.transition);
                if (transition.from)
                    machine.AddTransition(transition.from.state, checkedTransition);
                else
                    machine.AddAnyTransition(checkedTransition);
            }
            
            return machine;
        }
        
        public void RemoveAndDestroy(StateAsset state)
        {
            states.Remove(state);
            DestroyImmediate(state, true);
        }
    }
}