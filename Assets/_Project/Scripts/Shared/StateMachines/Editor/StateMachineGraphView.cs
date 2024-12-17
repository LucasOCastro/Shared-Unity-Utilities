using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shared.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Shared.StateMachines.Editor
{
    public class StateMachineGraphView : GraphView
    {
        private const string StylesPath = "Assets/_Project/Scripts/Shared/StateMachines/Editor/Styles/";
        private const string GraphViewStylePath = StylesPath + "GraphViewStyles.uss";
        
        private readonly Vector2 _nodeSize = new(200, 150);
        
        public BaseStateMachineAsset Asset { get; private set; }
        public MachineTypeInfo Types { get; private set; }
        
        [CanBeNull] 
        private StateNodeView RootNode => Asset.initialState != null ? GetOrAddState(Asset.initialState) : null;
        
        private readonly Dictionary<StateAsset, StateNodeView> _nodes = new();
        private readonly StateMachineSearchWindowProvider _searchWindowProvider;
        
        public StateMachineGraphView()
        {
            graphViewChanged = OnGraphViewChanged;
            nodeCreationRequest = context => OpenSearchWindow(context.screenMousePosition);
            
            SetupBackground();
            SetupStyles();
            SetupManipulators();
            _searchWindowProvider = SetupSearchWindow();
            
            //TODO if this is reused without instantiating a new one it will cause issues
            RegisterCallbackOnce<DetachFromPanelEvent>(_ => OnDestroy());
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var result = new List<Port>();
            
            foreach (var port in ports)
            {
                if(port.direction == startPort.direction)
                    continue;

                if(startPort.connections.Any(e => e.input == port))
                    continue;

                result.Add(port);
            }

            return result;
        }
        
        public void SetRoot(StateAsset state)
        {
            if (RootNode != null)
            {
                RootNode.IsRoot = false;
                Asset.initialState = null;
            }

            if (state != null)
            {
                // Set initial state before adding node to avoid infinite recursion
                Asset.initialState = state;
                var node = GetOrAddState(state);
                node.IsRoot = true;
            }
        }

#region SETUP
        
        private void SetupBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
            SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
        }

        private void SetupStyles()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(GraphViewStylePath));
        }

        private void SetupManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new ContentZoomer());
        }

        private StateMachineSearchWindowProvider SetupSearchWindow()
        {
            var provider = ScriptableObject.CreateInstance<StateMachineSearchWindowProvider>();
            provider.GraphView = this;
            provider.OnStateAssetCreated += newState =>
            {
                newState.position = GetLocalMousePosition(newState.position);
                GetOrAddState(newState);
            };
            return provider;
        }
        
#endregion

#region CALLBACKS
        private void OnDestroy()
        {
            Object.DestroyImmediate(_searchWindowProvider);
        }

        /// <summary>
        /// Handles changes in the graph view.
        /// Ensures that the initial state node is not removed and updates the internal state node dictionary.
        /// </summary>
        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.elementsToRemove != null)
            {
                change.elementsToRemove.Remove(RootNode);
                foreach (var element in change.elementsToRemove)
                {
                    switch (element)
                    {
                        case StateNodeView node:
                            _nodes.Remove(node.Asset);
                            Asset.RemoveAndDestroy(node.Asset);
                            break;
                    }
                }
            }
            
            return change;
        }
        
#endregion
        
        /// <summary>
        /// Gets or adds a <see cref="StateNodeView"/> for the given <paramref name="state"/>.
        /// If the state is not already in the dictionary, adds it to the graph view and the asset's states.
        /// If the asset's initial state is not set, sets the given state as the initial state.
        /// Returns the <see cref="StateNodeView"/> for the given <paramref name="state"/>.
        /// </summary>
        private StateNodeView GetOrAddState(StateAsset state)
        {
            if (!Asset.states.Contains(state))
                Asset.states.Add(state);
            
            if (!Asset.initialState)
                SetRoot(state);
            
            if (_nodes.TryGetValue(state, out var node))
                return node;
            
            if (!AssetDatabase.IsSubAsset(state))
            {
                AssetDatabase.AddObjectToAsset(state, Asset);
                AssetDatabase.SaveAssets();
            }
            
            node = new(this);
            node.SetState(state);
            node.SetPosition(new(state.position, _nodeSize));
            AddElement(node);
            
            _nodes.Add(state, node);
            
            node.IsRoot = state == Asset.initialState;
            
            return node;
        }

        /// <summary>
        /// Sets the state machine asset for the graph view, clearing existing nodes and adding new ones.
        /// Initializes nodes for each state in the asset and sets the initial state node if defined.
        /// </summary>
        public void SetAsset(BaseStateMachineAsset asset)
        {
            graphElements.ForEach(RemoveElement);
            _nodes.Clear();
            
            Asset = asset;
            if (!asset)
            {
                Types = default;
                return;
            }
            
            Types = MachineTypeInfo.From(asset.GetType());
            
            if (Asset.initialState)
                GetOrAddState(Asset.initialState);
            
            foreach (var state in asset.states)
                GetOrAddState(state);
        }
        
        public void OpenSearchWindow(Vector2 position)
        {
            var context = new SearchWindowContext(position);
            SearchWindow.Open(context, _searchWindowProvider);
        }
        
        private Vector2 GetLocalMousePosition(Vector2 mousePosition)
        {
            Vector2 worldMousePosition = mousePosition;
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }

        [CanBeNull]
        public StateNodeView GetNodeAt(Vector2 pos) =>
            _nodes.Values.FirstOrDefault(n => n.worldBound.Contains(pos));
    }
}