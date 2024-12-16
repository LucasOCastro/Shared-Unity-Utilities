using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Shared.StateMachines.Editor
{
    public class TransitionConnectorListener : IEdgeConnectorListener
    {
        private readonly GraphView _graphView;

        public TransitionConnectorListener(GraphView graphView)
        {
            _graphView = graphView;
        }
        
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // TODO request new node
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            Debug.Log($"On drop from {edge.output} to {edge.input}");
        }
    }
}