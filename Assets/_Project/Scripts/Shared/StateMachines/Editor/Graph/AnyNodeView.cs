using Shared.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Shared.StateMachines.Editor
{
    public class AnyNodeView : BaseNodeView
    {
        public Color AnyBorderColor = Color.blue;
        public float AnyBorderWidth = 2f;

        public AnyNodeView(StateMachineGraphView graphView) : base(graphView, hasInput: false, hasOutput: true)
        {
            capabilities -= Capabilities.Deletable;
            style.SetBorder(AnyBorderWidth, AnyBorderColor);
            
            title = "Any";
        }
        
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            
            if (GraphView.Asset)
                GraphView.Asset.anyNodePosition = newPos.position;
        }
    }
}