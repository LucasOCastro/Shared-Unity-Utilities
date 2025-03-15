using SharedUtilities.StateMachines.Editor.Graph;
using UnityEngine;
using UnityEngine.UIElements;

namespace SharedUtilities.StateMachines.Editor
{
    internal class NodeInspector : IInspector
    {
        public StateNodeView Node { get; }
        private readonly UnityEditor.Editor _editor;
        
        public NodeInspector(StateNodeView node)
        {
            Node = node;
            _editor = UnityEditor.Editor.CreateEditor(node.Asset);
        }

        public void CreateGUI(VisualElement container)
        {
            container.Add(new Label { text = Node.Asset.name });
            
            var inspectorGUI = _editor.CreateInspectorGUI();
            container.Add(inspectorGUI);
        }
        
        public void Dispose()
        {
            Object.DestroyImmediate(_editor);
        }
    }
}