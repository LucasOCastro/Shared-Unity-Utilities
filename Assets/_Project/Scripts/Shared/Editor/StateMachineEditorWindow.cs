using Shared.StateMachines;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Shared.Editor
{
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
        private ObjectField _targetField;
        
        
        private void SetTarget(BaseStateMachineAsset target) 
        {
            _target = target;
            
            if (_targetField != null && _targetField.value != _target)
                _targetField.value = _target;
        }

        private void CreateGUI()
        {
            // Create a two-pane view with the left pane being fixed.
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            splitView.Add(CreateInspector());
            splitView.Add(CreateGraphView());
            
            rootVisualElement.Add(splitView);
        }
        
        private VisualElement CreateInspector()
        {
            VisualElement root = new VisualElement();
            
            _targetField = new()
            {
                value = _target,
                objectType = typeof(BaseStateMachineAsset)
            };
            _targetField.RegisterValueChangedCallback(v => SetTarget(v.newValue as BaseStateMachineAsset));
            root.Add(_targetField);

            return root;
        }
        
        private VisualElement CreateGraphView()
        {
            return new();
        }
    }
}