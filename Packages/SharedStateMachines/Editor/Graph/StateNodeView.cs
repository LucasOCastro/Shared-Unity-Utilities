using System;
using System.Collections.Generic;
using SharedUtilities.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SharedUtilities.StateMachines.Editor.Graph
{
    public class StateNodeView : BaseNodeView
    {
        public StateAsset Asset { get; private set; }
        public IState State => Asset.state;
        
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
                    style.ClearBorder();
                    capabilities |= Capabilities.Deletable;
                }
            }
        }
        
        public StateNodeView(StateMachineGraphView graphView) : base(graphView, hasInput: true, hasOutput: true)
        {
            capabilities |= Capabilities.Renamable;
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

        public override void OnSelected()
        {
            base.OnSelected();

            GraphView.SelectObject(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            
            GraphView.DeselectObject(this);
        }

        public override string title
        {
            get => base.title;
            set
            {
                base.title = value; 
                Asset.name = value;
            }
        }

        protected override IEnumerable<(string label, Action action, bool disabled)> GetContextMenuActions()
        {
            foreach (var baseAction in base.GetContextMenuActions())
                yield return baseAction;

            yield return ("Make root", () => GraphView.SetRoot(Asset), disabled: IsRoot);
        }
    }
}