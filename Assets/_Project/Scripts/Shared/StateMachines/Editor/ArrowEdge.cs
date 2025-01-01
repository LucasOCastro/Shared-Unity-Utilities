using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Shared.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shared.StateMachines.Editor
{
    public class ArrowEdge : Edge
    {
        public ObservableCollection<TransitionAsset> Assets { get; } = new();
        
        public float ArrowHeight = 15f;
        public float ArrowBase = 10f;

        [CanBeNull] public BaseNodeView InputNode => (input as StateTransitionPort)?.Node;
        [CanBeNull] public StateAsset InputState => (InputNode as StateNodeView)?.Asset;
        
        [CanBeNull] public BaseNodeView OutputNode => (output as StateTransitionPort)?.Node;
        [CanBeNull] public StateAsset OutputState => (OutputNode as StateNodeView)?.Asset;
        

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
            edgeControl.generateVisualContent += DrawArrow;
            
            Assets.CollectionChanged += (_, _) => MarkDirtyRepaint();
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            //Offset to node edge
            Vector2 dir = (PointsAndTangents[3] - PointsAndTangents[0]).normalized;
            PointsAndTangents[0] = OffsetToEdge(PointsAndTangents[0], dir);
            PointsAndTangents[3] = OffsetToEdge(PointsAndTangents[3], -dir);

            //Offset for arrow
            PointsAndTangents[3] -= dir * ArrowHeight;

            // Make straight
            PointsAndTangents[1] = PointsAndTangents[0];
            PointsAndTangents[2] = PointsAndTangents[3];

            // Disable the round caps
            edgeControl.drawToCap = edgeControl.drawFromCap = false;
        }

        private void DrawArrow(MeshGenerationContext meshGenerationContext)
        {
            var painter = meshGenerationContext.painter2D;

            painter.strokeColor = painter.fillColor = edgeControl.inputColor;
            painter.lineWidth = edgeControl.edgeWidth;

            Vector2 basePoint = edgeControl.parent.ChangeCoordinatesTo(edgeControl, PointsAndTangents[3]);
            Vector2 dir = (PointsAndTangents[3] - PointsAndTangents[0]).normalized;
            Vector2 tip = basePoint + dir * ArrowHeight;

            Vector2 baseDir = Vector2.Perpendicular(dir);
            Vector2 a = basePoint + baseDir * ArrowBase * .5f;
            Vector2 b = basePoint - baseDir * ArrowBase * .5f;

            painter.BeginPath();
            painter.MoveTo(tip);
            painter.LineTo(a);
            painter.LineTo(b);
            painter.LineTo(tip);
            painter.Fill(FillRule.OddEven);
        }

        private Vector2 OffsetToEdge(Vector2 localPos, Vector2 dir)
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