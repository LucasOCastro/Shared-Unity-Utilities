using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Shared.StateMachines.Editor
{
    public class ArrowEdge : Edge
    {
        public ArrowEdge()
        {
            edgeControl.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            // Make straight
            PointsAndTangents[1] = PointsAndTangents[0];
            PointsAndTangents[2] = PointsAndTangents[3];
        }
    }
}