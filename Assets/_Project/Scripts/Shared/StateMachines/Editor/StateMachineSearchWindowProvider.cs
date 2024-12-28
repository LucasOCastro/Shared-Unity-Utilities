using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shared.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityUtils;

namespace Shared.StateMachines.Editor
{
    public class StateMachineSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public event Action<StateAsset> OnStateAssetCreated;
        
        public StateMachineGraphView GraphView { get; set; }
        private MachineTypeInfo Types => GraphView.Types;

        private static Node GetTypeTree(Type baseType)
        {
            var result = new Dictionary<Type, Node>();
            foreach (var type in TypeCache.GetTypesDerivedFrom(baseType))
            {
                var node = result.GetOrAdd(type, () => new(type));
                
                if (type.BaseType != null)
                {
                    var parent = result.GetOrAdd(type.BaseType, () => new(type.BaseType));
                    parent.Children.Add(node);
                    node.Parent = parent;
                }
            }

            // If doesn't contain the base type, then the baseType is an interface, must create root manually
            if (!result.TryGetValue(baseType, out var root))
            {
                root = new(baseType);
                var roots = result.Values.Where(t => t.Type.DirectlyImplementsInterface(baseType));
                roots.ForEach(r => root.Children.Add(r));
            }

            return root;
        }
        
        private static IEnumerable<SearchTreeEntry> GetSubTree(Node node, int level)
        {
            var entry = node.ToEntry(level);
            if (entry != null)
            {
                yield return entry;
                level++;
            }
            
            var childTrees = node.Children
                .OrderBy(child => child.Name)
                .SelectMany(child => GetSubTree(child, level));
                
            foreach (var childEntry in childTrees)
                yield return childEntry;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var treeRoot = GetTypeTree(Types.StateType);
            
            string title = $"Create Node ({treeRoot.Name})";
            var result = new List<SearchTreeEntry> {new SearchTreeGroupEntry(new(title))};
            
            result.AddRange(GetSubTree(treeRoot, 1));
            return result;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            if (entry.userData is not Type { IsAbstract: false } type) 
                return true;
            
            var stateAsset = CreateInstance<StateAsset>();
            stateAsset.SetState(type);
            stateAsset.position = GraphView.GetLocalMousePosition(context.screenMousePosition, isScreenPosition: true);
            OnStateAssetCreated?.Invoke(stateAsset);

            return true;
        }
        
        private class Node
        {
            public string Name => Type.GetDisplayName();
            public readonly Type Type;
            public readonly HashSet<Node> Children = new();
            public Node Parent;
            
            public Node(Type type) => Type = type;

            [CanBeNull]
            public SearchTreeEntry ToEntry(int level)
            {
                if (Parent == null)
                    return null;
                
                bool hasEmptyConstructor = Type.GetConstructor(Type.EmptyTypes) != null;
                bool isBaseType = Type.IsAbstract || Type.IsInterface || !hasEmptyConstructor;
                if (isBaseType && Children.Count == 0)
                    return null;
                
                return Type.IsAbstract || Type.IsInterface
                    ? new SearchTreeGroupEntry(new(Name), level)
                    : new SearchTreeEntry(new(Name)) { level = level, userData = Type };
            }
        }
    }
}