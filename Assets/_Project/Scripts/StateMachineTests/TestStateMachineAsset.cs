using SharedUtilities.StateMachines;
using UnityEngine;

namespace SharedUtilitiesPackages.Game
{
    [CreateAssetMenu(fileName="TestStateMachine",menuName = "State Machines/Test State Machine")]
    public class TestStateMachineAsset : StateMachineAsset<MyStateMachine, MyStateBase, BaseTransition<MyStateBase>>
    {
    }
}