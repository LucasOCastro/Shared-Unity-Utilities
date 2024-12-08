using System;
using UnityEngine;

namespace Shared.StateMachines
{
    [CreateAssetMenu(fileName = "New State Machine", menuName = "StateMachines/State Machine")]
    public class BaseStateMachineAsset : ScriptableObject
    {
        [SerializeReference]
        public IState initialState;
        
        [SerializeReference]
        public IState[] states = Array.Empty<IState>();
        
        [SerializeReference]
        public TransitionAsset[] transitions = Array.Empty<TransitionAsset>();

        public StateMachine GenericConstruct()
        {
            var machine = new StateMachine(initialState);
            
            foreach (var transition in transitions)
            {
                machine.AddTransition(transition.from, transition.to, transition.condition);
            }
            
            machine.SetState(initialState);
            return machine;
        }
    }
}