using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SharedUtilities.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Unity.EditorCoroutines.Editor;

namespace SharedUtilities.StateMachines.Editor.Graph
{
    public class StateMachineGraphView : GraphView
    {
        private const string StylesPath = "Packages/com.lucascastro.shared-state-machines/Editor/Styles/";
        private const string GraphViewStylePath = StylesPath + "GraphViewStyles.uss";

        private readonly Vector2 _nodeSize = new(200, 150);

        public StateMachineEditorWindow Window { get; }

        public BaseStateMachineAsset Asset { get; private set; }

        public MachineTypeInfo Types { get; private set; }

        [CanBeNull]
        private StateNodeView RootNode => Asset.initialState != null ? GetOrAddState(Asset.initialState) : null;
        
        private readonly AnyNodeView _anyNode;

        private readonly Dictionary<StateAsset, StateNodeView> _nodes = new();
        private readonly Dictionary<(StateAsset from, StateAsset to), ArrowEdge> _edges = new();
        private readonly StateMachineSearchWindowProvider _searchWindowProvider;

        #region INITIALIZATION

        public StateMachineGraphView(StateMachineEditorWindow editorWindow)
        {
            Window = editorWindow;

            graphViewChanged = OnGraphViewChanged;
            nodeCreationRequest = context => OpenSearchWindow(context.screenMousePosition);

            SetupBackground();
            SetupStyles();
            SetupManipulators();
            _searchWindowProvider = SetupSearchWindow();

            _anyNode = CreateAnyNode();

            RegisterCallbackOnce<DetachFromPanelEvent>(_ => OnDestroy());
        }

        /// <summary>
        /// Sets the state machine asset for the graph view, clearing existing nodes and adding new ones.
        /// Initializes nodes for each state in the asset and sets the initial state node if defined.
        /// </summary>
        public void SetAsset(BaseStateMachineAsset asset)
        {
            graphElements.Where(x => x != _anyNode).ForEach(RemoveElement);
            _nodes.Clear();
            _edges.Clear();

            Asset = asset;
            if (!asset)
            {
                Types = default;
                return;
            }

            _anyNode.SetPosition(new(Asset.anyNodePosition, _nodeSize));
            
            Types = MachineTypeInfo.From(asset.GetType());

            if (Asset.initialState)
                GetOrAddState(Asset.initialState);

            foreach (var state in asset.states)
                GetOrAddState(state);

            foreach (var transition in asset.transitions)
                GetOrAddEdge(transition);
        }

        #endregion


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
                GetOrAddState(newState);
            };
            return provider;
        }

        private AnyNodeView CreateAnyNode()
        {
            var node = new AnyNodeView(this);
            AddElement(node);
            return node;
        }

        #endregion


        #region OVERRIDES

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var result = new List<Port>();

            foreach (var port in ports)
            {
                if (port.direction == startPort.direction)
                    continue;

                if (startPort.connections.Any(e => e.input == port))
                    continue;

                result.Add(port);
            }

            return result;
        }

        #endregion


        #region EVENTS

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
                        case ArrowEdge edge:
                            foreach (var transition in edge.Assets)
                                Asset.RemoveAndDestroy(transition);
                            _edges.Remove((edge.OutputState, edge.InputState));
                            break;
                    }
                }
            }

            return change;
        }

        #endregion


        #region NODES

        /// <summary>
        /// Gets or adds a <see cref="BaseNodeView"/> for the given <paramref name="state"/>.
        /// If the state is not already in the dictionary, adds it to the graph view and the asset's states.
        /// If the asset's initial state is not set, sets the given state as the initial state.
        /// Returns the <see cref="BaseNodeView"/> for the given <paramref name="state"/>.
        /// </summary>
        private StateNodeView GetOrAddState(StateAsset state)
        {
            if (!Asset.states.Contains(state))
                Asset.states.Add(state);

            if (!Asset.initialState)
                SetRoot(state);

            if (!AssetDatabase.IsSubAsset(state))
            {
                AssetDatabase.AddObjectToAsset(state, Asset);
                AssetDatabase.SaveAssets();
            }

            if (_nodes.TryGetValue(state, out var node))
                return node;

            node = new(this);
            node.SetState(state);
            node.SetPosition(new(state.position, _nodeSize));
            AddElement(node);

            _nodes.Add(state, node);

            node.IsRoot = state == Asset.initialState;

            return node;
        }

        #endregion


        #region EDGES

        public ArrowEdge GetOrAddEdge(StateAsset from, StateAsset to)
        {
            if (_edges.TryGetValue((from, to), out var edge))
                return edge;
            
            BaseNodeView fromNode = from ? GetOrAddState(from) : _anyNode;
            BaseNodeView toNode = GetOrAddState(to);

            edge = new()
            {
                output = fromNode.OutputPort,
                input = toNode.InputPort
            };
            AddElement(edge);

            _edges.Add((from, to), edge);

            return edge;
        }

        public ArrowEdge GetOrAddEdge(TransitionAsset transition)
        {
            if (!Asset.transitions.Contains(transition))
                Asset.transitions.Add(transition);

            if (!AssetDatabase.IsSubAsset(transition))
            {
                AssetDatabase.AddObjectToAsset(transition, Asset);
                AssetDatabase.SaveAssets();
            }

            var edge = GetOrAddEdge(transition.from, transition.to);
            if (!edge.Assets.Contains(transition))
                edge.Assets.Add(transition);
            
            return edge;
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

        #endregion

        
        #region SELECTION
                
                [CanBeNull] public GraphElement SelectedObject { get; private set; }
                public event Action<GraphElement> OnSelectionChanged;
                        
                public void SelectObject([CanBeNull] GraphElement element)
                {
                    SelectedObject = element;
                    OnSelectionChanged?.Invoke(element);
                }

                public void DeselectObject(GraphElement element)
                {
                    if (SelectedObject == null)
                        return;
                    
                    if (element == SelectedObject)
                        SelectObject(null);
                }
         
        #endregion


        #region UTILITIES

        public void OpenSearchWindow(Vector2 position, [CanBeNull] Action<StateAsset> onStateCreated = null)
        {
            var context = new SearchWindowContext(position);
            SearchWindow.Open(context, _searchWindowProvider);

            if (onStateCreated != null)
            {
                _searchWindowProvider.OnStateAssetCreated += onStateCreated;
                Window.StartCoroutine(WaitForWindowCloseThenUnsubscribe());
            }

            return;

            IEnumerator WaitForWindowCloseThenUnsubscribe()
            {
                EditorWindow window = null;
                yield return new WaitUntil(() => window = EditorWindow.GetWindow<SearchWindow>());
                yield return new WaitUntil(() => !window || !window.hasFocus);
                _searchWindowProvider.OnStateAssetCreated -= onStateCreated;
            }
        }

        public Vector2 ScreenPositionToLocal(Vector2 screenMousePosition)
        {
            Vector2 worldMousePosition = screenMousePosition - Window.position.position;
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }

        public Vector2 WorldPositionToScreen(Vector2 worldMousePosition)
        {
            Vector2 screenMousePosition = Window.position.position + worldMousePosition;
            return screenMousePosition;
        }

        [CanBeNull]
        public BaseNodeView GetNodeAt(Vector2 pos)
        {
            var node = _nodes.Values.FirstOrDefault(n => n.worldBound.Contains(pos));
            if (node != null)
                return node;

            if (_anyNode.worldBound.Contains(pos))
                return _anyNode;

            return null;
        }

        #endregion
    }
}