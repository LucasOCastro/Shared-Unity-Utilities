using System;
using Shared.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shared.StateMachines.Editor
{
    public class StateNodeView : Node
    {
        public StateAsset Asset { get; private set; }
        public IState State => Asset.state;

        public readonly Port InputPort;
        public readonly Port OutputPort;

        public float RootBorderWidth = 2f;
        public Color RootBorderColor = Color.yellow;

        private bool _isRoot;

        public bool IsRoot
        {
            get => _isRoot;
            set
            {
                _isRoot = value;
                if (value)
                {
                    style.SetBorder(RootBorderWidth, RootBorderColor);
                    capabilities -= Capabilities.Deletable;
                }
                else
                {
                    style.SetBorder(0f, Color.clear);
                    capabilities |= Capabilities.Deletable;
                }
            }
        }

        public StateMachineGraphView GraphView { get; }

        public StateNodeView(StateMachineGraphView graphView)
        {
            GraphView = graphView;

            InputPort = CreatePort(Direction.Input);
            OutputPort = CreatePort(Direction.Output);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            CenterPort(InputPort);
            CenterPort(OutputPort);
        }

        private void CenterPort(Port port)
        {
            var position = GetPosition();
            position.position = Vector2.zero;//this.WorldToLocal(this.GetGlobalCenter());
            port.SetPosition(position);
        }

        public void SetState(StateAsset state)
        {
            Asset = state;
            SetPosition(new(GetPosition()) { position = state.position });
            title = state.name;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Asset.position = newPos.position;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("Add transition", _ => BeginDragEdge());
            evt.menu.AppendAction("Make root", _ => GraphView.SetRoot(Asset),
                IsRoot ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);

            evt.menu.AppendSeparator();
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