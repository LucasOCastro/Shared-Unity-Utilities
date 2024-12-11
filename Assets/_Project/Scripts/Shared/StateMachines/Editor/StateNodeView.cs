using Shared.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Shared.StateMachines.Editor
{
    public class StateNodeView : Node
    {
        public StateAsset State { get; private set; }

        private bool _isInitial;
        public bool IsInitial
        {
            get => _isInitial;
            set
            {
                _isInitial = value;
                if (value)
                    style.SetBorder(5f, Color.yellow);
                else 
                    style.SetBorder(0f, Color.clear);
            }
        }

        public void SetState(StateAsset state)
        {
            State = state;
            SetPosition(new(state.position, GetPosition().size));
            title = state.name;
        }
        
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            State.position = newPos.position;
        }
    }
}