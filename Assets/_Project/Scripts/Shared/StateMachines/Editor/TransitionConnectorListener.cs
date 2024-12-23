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
            var transition = (ArrowEdge)edge;
            var from = transition.OutputNode;
            var to = transition.InputNode;
            _graphView.OpenSearchWindow(position, newState =>
            {
                Debug.Log($"CREATED {newState} - edge is {transition} from {transition.output} to {transition.input}");
                if (from != null)
                    _graphView.Connect(from.Asset, newState);

                if (to != null)
                    _graphView.Connect(newState, to.Asset);
            });
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            var transition = (ArrowEdge)edge;
            if (transition.InputNode != null && transition.OutputNode != null)
                _graphView.Connect(transition.OutputNode.Asset, transition.InputNode.Asset);
        }
    }
}