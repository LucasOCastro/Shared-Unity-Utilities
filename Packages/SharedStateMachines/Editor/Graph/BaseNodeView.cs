using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SharedUtilities.StateMachines.Editor.Graph
{
    public abstract class BaseNodeView : Node
    {
        public readonly Port InputPort;
        public readonly Port OutputPort;

        public StateMachineGraphView GraphView { get; }

        public BaseNodeView(StateMachineGraphView graphView, bool hasInput, bool hasOutput)
        {
            GraphView = graphView;

            if (hasInput)
                InputPort = CreatePort(Direction.Input);
            
            if (hasOutput)
                OutputPort = CreatePort(Direction.Output);
            
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            CenterPort(InputPort);
            CenterPort(OutputPort);
         
            return;
            
            void CenterPort([CanBeNull] Port port)
            {
                if (port == null) return;
                
                var position = GetPosition();
                position.position = Vector2.zero;//this.WorldToLocal(this.GetGlobalCenter());
                port.SetPosition(position);
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            foreach (var (label, action, disabled) in GetContextMenuActions())
            {
                evt.menu.AppendAction(label, _ => action(),
                    disabled ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
            }
            
            evt.menu.AppendSeparator();
        }

        protected virtual IEnumerable<(string label, Action action, bool disabled)> GetContextMenuActions()
        {
            if (OutputPort != null)
                yield return ("Add transition", BeginDragEdge, false);
        }

        private void BeginDragEdge()
        {
            var ev = MouseDownEvent.GetPooled(OutputPort.GetGlobalCenter(), 0, 1, Vector2.zero);
            ev.target = OutputPort;
            OutputPort.SendEvent(ev);
        }

        private Port CreatePort(Direction direction)
        {
            var port = InstantiatePort(Orientation.Vertical, direction, Port.Capacity.Multi, typeof(bool));
            port.portName = "";
            
            // Remove label
            port.Q<TextElement>().RemoveFromHierarchy();
            
            // Make sure port can center nicely
            port.style.position = Position.Absolute;
            //port.StretchToParentSize();
            port.style.justifyContent = Justify.Center;
            port.style.alignContent = Align.Center;
            port.style.paddingLeft = port.style.paddingRight = 0;

            // Add and hide
            Add(port);
            port.visible = false;
            
            return port;
        }

        public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            return StateTransitionPort.Create(direction, GraphView);
        }
    }
}