using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Shared.StateMachines.Editor
{
    public class TransitionConnectorListener : IEdgeConnectorListener
    {
        private readonly StateMachineGraphView _graphView;

        public TransitionConnectorListener(StateMachineGraphView graphView)
        {
            _graphView = graphView;
        }
        
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Vector2 screenPosition = _graphView.WorldPositionToScreen(position);
            
            var transition = (ArrowEdge)edge;
            var from = transition.OutputState;
            var to = transition.InputState;
            _graphView.OpenSearchWindow(screenPosition, newState =>
            {
                if (to == null)
                    _graphView.Connect(from, newState);
                else if (from == null)
                    _graphView.Connect(newState, to);
            });
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            var transition = (ArrowEdge)edge;
            if (transition.InputState != null)
                _graphView.Connect(transition.OutputState, transition.InputState);
        }
    }
}