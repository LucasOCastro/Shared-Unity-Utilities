using System;
using System.Linq;
using SharedUtilities.StateMachines.Editor.Graph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SharedUtilities.StateMachines.Editor
{
    public interface IInspector : IDisposable
    {
        void CreateGUI(VisualElement container);
    }
    
    public class NodeInspector : IInspector
    {
        public StateNodeView Node { get; }
        private UnityEditor.Editor _editor;
        
        public NodeInspector(StateNodeView node)
        {
            Node = node;
            _editor = UnityEditor.Editor.CreateEditor(node.Asset);
        }

        public void CreateGUI(VisualElement container)
        {
            container.Add(new Label() { text = Node.Asset.name });
            
            var inspectorGUI = _editor.CreateInspectorGUI();
            container.Add(inspectorGUI);
        }
        
        public void Dispose()
        {
            Object.DestroyImmediate(_editor);
        }
    }
    
    public class EdgeInspector : IInspector
    {
        public ArrowEdge Edge { get; }
        private UnityEditor.Editor _editor;
        
        public EdgeInspector(ArrowEdge edge)
        {
            Edge = edge;
            _editor = UnityEditor.Editor.CreateEditor(edge.Assets.OfType<Object>().ToArray());
        }
        
        public void CreateGUI(VisualElement container)
        {
            container.Add(new Label() { text = Edge.Assets.Aggregate("", (a, b) => a + b.name) });
        }
        
        public void Dispose()
        {
            Object.DestroyImmediate(_editor);
        }
    }
    
    public class StateMachineEditorWindow : EditorWindow
    {
        public static void Open(BaseStateMachineAsset target)
        {
            var window = GetWindow<StateMachineEditorWindow>();
            window.titleContent = new("State Machine Editor");
            window.SetTarget(target);
            window.Show();
        }
        
        private BaseStateMachineAsset _target;
        private StateMachineGraphView _graphView;
        
        private ObjectField _targetField;
        private VisualElement _inspectorContainer;
        private IInspector _currentInspector;
        
        private void SetTarget(BaseStateMachineAsset target) 
        {
            _target = target;
            
            if (_targetField != null && _targetField.value != _target)
                _targetField.value = _target;

            _graphView?.SetAsset(_target);
        }

        private void CreateGUI()
        {
            // Create a two-pane view with the left pane being fixed.
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            splitView.Add(CreateInspector());
            splitView.Add(CreateGraphView());
            
            rootVisualElement.Add(splitView);
        }
        
        private void HandleSelectionChanged(GraphElement element)
        {
            _inspectorContainer.Clear();
            _currentInspector?.Dispose();

            switch (element)
            {
                case StateNodeView node:
                    var inspector = new NodeInspector(node);
                    inspector.CreateGUI(_inspectorContainer);
                    break;
                case ArrowEdge edge:
                    var edgeInspector = new EdgeInspector(edge);
                    edgeInspector.CreateGUI(_inspectorContainer);
                    break;
            }
        }

        private VisualElement CreateInspector()
        {
            var root = new VisualElement();
            
            // Currently selected state machine
            _targetField = new()
            {
                value = _target,
                objectType = typeof(BaseStateMachineAsset)
            };
            _targetField.RegisterValueChangedCallback(v => SetTarget(v.newValue as BaseStateMachineAsset));
            root.Add(_targetField);
            
            // Selected element in the graph view
            _inspectorContainer = new();
            root.Add(_inspectorContainer);

            return root;
        }
        
        private VisualElement CreateGraphView()
        {
            var root = new VisualElement();
            
            _graphView = new(this);
            root.Add(_graphView);
            
            _graphView.StretchToParentSize();
            _graphView.SetAsset(_target);
            
            _graphView.OnSelectionChanged += HandleSelectionChanged;
            
            return root;
        }
    }
}