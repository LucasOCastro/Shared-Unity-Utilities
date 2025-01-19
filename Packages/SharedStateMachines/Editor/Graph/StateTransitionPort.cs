using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace SharedUtilities.StateMachines.Editor.Graph
{
    public class StateTransitionPort : Port
    {
        public BaseNodeView Node => (BaseNodeView)node;
        public StateMachineGraphView GraphView { get; }
        
        private StateTransitionPort(Orientation orientation, Direction direction, Capacity capacity, System.Type type, StateMachineGraphView graphView)
            : base(orientation, direction, capacity, type)
        {
            GraphView = graphView;
            m_EdgeConnector = new StateTransitionEdgeConnector(this);
        }
        
        public static StateTransitionPort Create(Direction direction, StateMachineGraphView graphView)
        {
            var port = new StateTransitionPort(Orientation.Vertical, direction, Capacity.Single, typeof(bool), graphView);
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }
    }
}