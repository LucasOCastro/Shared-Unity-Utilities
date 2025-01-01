using System.Linq;
using JetBrains.Annotations;
using Shared.StateMachines.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shared.StateMachines.Editor
{
    public class StateTransitionEdgeConnector : EdgeConnector<ArrowEdge>
    {
        private readonly StateTransitionPort _port;
        public StateMachineGraphView GraphView => _port.GraphView;
        
        public StateTransitionEdgeConnector(StateTransitionPort port) : base(new TransitionConnectorListener(port.GraphView))
        {
            _port = port;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            AdjustPos(e);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseMoveEvent e)
        {
            AdjustPos(e);
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseDownEvent e)
        {
            AdjustPos(e);
            base.OnMouseDown(e);
        }

        private void AdjustPos<T>(MouseEventBase<T> ev) where T : MouseEventBase<T>, new()
        {
            var edge = edgeDragHelper.edgeCandidate;
            if (edge == null)
                return;
            
            Port port = GetPortAt(ev.mousePosition, GraphView, edge);
            if (port == null)
                return;
            
            ev.SetPosition(port.GetGlobalCenter());
            ev.target = port;
        }

        [CanBeNull]
        private static Port GetPortAt(Vector2 pos, StateMachineGraphView graphView, Edge edge)
        {
            var node = graphView.GetNodeAt(pos);

            if (node == null)
                return null;
            
            if (edge.input == null && edge.output != null)
                return node.InputPort;
            
            if (edge.input != null && edge.output == null)
                return node.OutputPort;

            return null;
        }
    }
}