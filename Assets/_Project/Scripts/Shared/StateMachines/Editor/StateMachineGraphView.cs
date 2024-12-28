using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils;
using Object = UnityEngine.Object;

namespace Shared.StateMachines.Editor
{
    public class StateMachineGraphView : GraphView
    {
        private const string StylesPath = "Assets/_Project/Scripts/Shared/StateMachines/Editor/Styles/";
        private const string GraphViewStylePath = StylesPath + "GraphViewStyles.uss";

        private readonly Vector2 _nodeSize = new(200, 150);

        public StateMachineEditorWindow Window { get; }

        public BaseStateMachineAsset Asset { get; private set; }

        public MachineTypeInfo Types { get; private set; }

        [CanBeNull]
        private StateNodeView RootNode => Asset.initialState != null ? GetOrAddState(Asset.initialState) : null;
        
        private readonly AnyNodeView _anyNode;

        private readonly Dictionary<StateAsset, StateNodeView> _nodes = new();
        private readonly Dictionary<TransitionAsset, ArrowEdge> _edges = new();
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
                newState.position = GetLocalMousePosition(newState.position);
                GetOrAddState(newState);
            };
            return provider;
        }

        private AnyNodeView CreateAnyNode()
        {
            var node = new AnyNodeView(this);
            AddElement(node);
            node.SetPosition(new(Vector2.zero, _nodeSize));
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
                            _edges.Remove(edge.Asset);
                            Asset.RemoveAndDestroy(edge.Asset);
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

        public ArrowEdge GetOrAddEdge(TransitionAsset transition)
        {
            if (!Asset.transitions.Contains(transition))
                Asset.transitions.Add(transition);

            if (!AssetDatabase.IsSubAsset(transition))
            {
                AssetDatabase.AddObjectToAsset(transition, Asset);
                AssetDatabase.SaveAssets();
            }

            if (_edges.TryGetValue(transition, out var edge))
                return edge;

            BaseNodeView fromNode = transition.from ? GetOrAddState(transition.from) : _anyNode;
            var toNode = GetOrAddState(transition.to);

            edge = new()
            {
                output = fromNode.OutputPort,
                input = toNode.InputPort
            };
            edge.SetAsset(transition);
            AddElement(edge);

            _edges.Add(transition, edge);

            _anyNode.SetPosition(new(_anyNode.GetPosition()) { position = Asset.anyNodePosition });

            return edge;
        }

        public void Connect(StateAsset from, StateAsset to)
        {
            var transition = ScriptableObject.CreateInstance<TransitionAsset>();
            transition.from = from;
            transition.to = to;

            GetOrAddEdge(transition);
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


        #region UTILITIES

        public void OpenSearchWindow(Vector2 position, [CanBeNull] Action<StateAsset> onStateCreated = null)
        {
            var context = new SearchWindowContext(position);
            SearchWindow.Open(context, _searchWindowProvider);

            if (onStateCreated != null)
            {
                _searchWindowProvider.OnStateAssetCreated += onStateCreated;
                WaitForWindowClose()
                    .ContinueWith(() => _searchWindowProvider.OnStateAssetCreated -= onStateCreated)
                    .Forget();
            }

            return;

            async UniTask WaitForWindowClose()
            {
                EditorWindow window = null;
                await UniTask.WaitUntil(() => window = EditorWindow.GetWindow<SearchWindow>());
                await UniTask.WaitUntil(() => !window || !window.hasFocus);
            }
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isScreenPosition = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isScreenPosition)
            {
                worldMousePosition = Window.rootVisualElement.ChangeCoordinatesTo(Window.rootVisualElement.parent,
                    mousePosition - Window.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
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