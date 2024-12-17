using System;
using Shared.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shared.StateMachines.Editor
{
    public class ArrowEdge : Edge
    {
        public StateMachineGraphView GraphView
        {
            get
            {
                var port = input ?? output;
                if (port == null)
                    throw new InvalidOperationException("Edge has no input or output port.");

                return ((StateTransitionPort)port).GraphView;
            }
        }
        
        public ArrowEdge()
        {
            edgeControl.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            //Offset to node edge
            Vector2 dir = (PointsAndTangents[3] - PointsAndTangents[0]).normalized;
            PointsAndTangents[0] = Offset(PointsAndTangents[0], dir);
            PointsAndTangents[3] = Offset(PointsAndTangents[3], -dir);
            
            // Make straight
            PointsAndTangents[1] = PointsAndTangents[0];
            PointsAndTangents[2] = PointsAndTangents[3];
        }

        private Vector2 Offset(Vector2 localPos, Vector2 dir)
        {
            Vector2 viewPos = this.LocalToWorld(localPos);
            var node = GraphView.GetNodeAt(viewPos);
            if (node == null)
                return localPos;
            
            var ray = new Ray2D(viewPos, dir);
            var bound = node.worldBound;
            
            float intersection = ray.Intersection(bound);
            
            return float.IsNaN(intersection) 
                ? localPos 
                : this.WorldToLocal(ray.GetPoint(intersection));
        }
    }
}