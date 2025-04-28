using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using SharedUtilities.StateMachines.Editor.Graph;
using SharedUtilities.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SharedUtilities.StateMachines.Editor
{
    public class EdgeInspector : IInspector
    {
        public ArrowEdge Edge { get; }
        
        public EdgeInspector(ArrowEdge edge)
        {
            Edge = edge;
        }
        
        public void CreateGUI(VisualElement container)
        {
            ListView list = new(Edge.Assets)
            {
                showAddRemoveFooter = true,
                reorderable = true,
                reorderMode = ListViewReorderMode.Simple,
                headerTitle = "Transitions",
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = MakeItem,
                bindItem = BindItem,
                unbindItem = UnbindItem,
                overridingAddButtonBehavior = OverridingAddButtonBehavior
            };
            
            list.itemsAdded += OnItemsAdded;
            list.itemsRemoved += OnItemsRemoved;
            
            container.Add(list);
        }
        
        private static VisualElement MakeItem()
        {
            return new();
        }

        private void BindItem(VisualElement item, int index)
        {
            var asset = Edge.Assets[index];
            var subElements = GetFieldSubElements(asset.transition, nameof(asset.transition), asset).ToList();
            
            // TODO make label and foldout text editable
            if (subElements.Count == 0)
            {
                //item.Add(new Label(asset.name));
                var label = new EditableLabel(asset.name);
                label.ValueChanged += (ref string name) =>
                {
                    if (string.IsNullOrWhiteSpace(name))
                        name = GenerateNameForTransition(asset);
                    asset.name = name;
                };
                item.Add(label);
                return;
            }
            
            var foldout = new Foldout
            {
                text = asset.name, 
                value = false
            };
            subElements.ForEach(foldout.Add);
            item.Add(foldout);
        }
        
        private static void UnbindItem(VisualElement item, int index)
        {            
            item.Clear();
        }
        
        private static void OnItemsAdded(IEnumerable<int> indices)
        {
            Debug.Log("On add " + indices.ToCommaSeparatedString());
        }

        private void OnItemsRemoved(IEnumerable<int> indices)
        {
            foreach (int index in indices.OrderByDescending(i => i))
            {
                var asset = Edge.Assets[index];
                Edge.GraphView.Asset.RemoveAndDestroy(asset);
            }
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
            asset.from = Edge.OutputState;
            asset.to = Edge.InputState;
            asset.transitionType = type.AssemblyQualifiedName;
            asset.name = GenerateNameForTransition(asset);
            
            asset.transition = (IBaseTransition)ObjectUtils.InstantiateObject(type);
            var state = asset.to.OrNull()?.state;
            asset.transition.To = state;
            
            Edge.GraphView.GetOrAddEdge(asset);
        }
        
        public void Dispose()
        {
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

        private static IEnumerable<VisualElement> GetFieldSubElements(object obj, string name, Object parent)
        {
            if (obj is Object o)
            {
                yield return new InspectorElement(o);
                yield break;
            }
            
            var so = new SerializedObject(parent);
            var prop = so.FindProperty(name);
            while (prop.NextVisible(true))
            {
                var field = new PropertyField(prop.Copy());
                field.Bind(so);
                yield return field;
            }
        }
        
        private static string GenerateNameForTransition(TransitionAsset asset)
        {
            var from = asset.from;
            var to = asset.to;
            var transitionType = Type.GetType(asset.transitionType).GetDisplayName();
            return $"{transitionType} ({from.name} -> {to.name})";
        }
    }
}