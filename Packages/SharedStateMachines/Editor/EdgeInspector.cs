using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using SharedUtilities.StateMachines.Editor.Graph;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SharedUtilities.StateMachines.Editor
{
    public class EdgeInspector : IInspector
    {
        public ArrowEdge Edge { get; }
        private readonly Dictionary<TransitionAsset, UnityEditor.Editor> _editors;
        
        public EdgeInspector(ArrowEdge edge)
        {
            Edge = edge;
            _editors = edge.Assets.ToDictionary(t => t, UnityEditor.Editor.CreateEditor);
        }
        
        public void CreateGUI(VisualElement container)
        {
            container.Add(new Label { text = Edge.Assets.Aggregate("", (a, b) => a + b.name) });
            
            ListView list = new(Edge.Assets)
            {
                showAddRemoveFooter = true,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                showFoldoutHeader = true,
                headerTitle = "TEST",
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = MakeItem,
                bindItem = BindItem,
                unbindItem = UnbindItem,
                onAdd = OnAdd,
                overridingAddButtonBehavior = OverridingAddButtonBehavior
            };
            container.Add(list);
        }
        
        private static VisualElement MakeItem()
        {
            return new();
        }

        private void BindItem(VisualElement item, int index)
        {
            var asset = Edge.Assets[index];
            var editor = _editors[asset];
            item.Add(editor.CreateInspectorGUI());
        }
        
        private void UnbindItem(VisualElement item, int index)
        {
            item.Clear();
        }
        
        private void OnAdd(BaseListView obj)
        {
            Debug.Log("On add " + obj);
            
            //Edge.GraphView.Asset.transitions.Add();
        }
        
        private void OverridingAddButtonBehavior(BaseListView list, Button button)
        {
            var types = GetTransitionTypes(out var options);
            EditorUtility.DisplayCustomMenu(button.worldBound, 
                options, 
                -1,
                (_, _, i) => OnTypeSelected(types[i]),
                null);
        }

        private void OnTypeSelected(Type type)
        {
            var asset = ScriptableObject.CreateInstance<TransitionAsset>();
            asset.name = $"{type.GetDisplayName()} ({Edge.OutputState?.name} -> {Edge.InputState?.name})";
            asset.from = Edge.OutputState;
            asset.to = Edge.InputState;
            asset.transitionType = type.AssemblyQualifiedName;
            
            asset.transition = (IBaseTransition)TypeUtils.InstantiateObject(type);
            var state = asset.to.OrNull()?.state;
            asset.transition.To = state;
            
            Edge.GraphView.GetOrAddEdge(asset);
        }
        
        public void Dispose()
        {
            _editors.Values.ForEach(Object.DestroyImmediate);
        }

        private Type[] GetTransitionTypes(out GUIContent[] options)
        {
            var types = Edge.GraphView.Types.TransitionType
                .GetDerivedTypes()
                .Where(t => !t.IsAbstract)
                .ToArray();
            
            options = new GUIContent[types.Length];
            for (int i = 0; i < types.Length; i++)
                options[i] = new(types[i].GetDisplayName());

            return types;
        }
    }
}