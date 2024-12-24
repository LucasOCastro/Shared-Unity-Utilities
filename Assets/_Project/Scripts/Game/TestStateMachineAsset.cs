using Shared.StateMachines;
using UnityEngine;

namespace SpaceWizardry.Game
{
    [CreateAssetMenu(fileName="TestStateMachine",menuName = "State Machines/Test State Machine")]
    public class TestStateMachineAsset : StateMachineAsset<MyStateMachine, MyStateBase, BaseTransition<MyStateBase>>
    {
    }
}