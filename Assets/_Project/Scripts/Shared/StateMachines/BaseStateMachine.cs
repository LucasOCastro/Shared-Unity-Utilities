using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shared.Extensions;

namespace Shared.StateMachines
{
    [Serializable]
    public class BaseStateMachine<TState, TTransition> 
        where TState : IState 
        where TTransition : class, IBaseTransition<TState>
    {
        private readonly Dictionary<TState, Node> _nodes = new();
        private readonly List<TTransition> _anyTransitions = new();

        private Node _current;
        public TState Current => _current.State;
        
        public BaseStateMachine(TState initialState) => SetState(initialState);

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
                SetState(transition.To);
            
            _current?.State.Update();
        }
        
        public void FixedUpdate() => _current?.State.FixedUpdate();

        public void SetState(TState state)
        {
            if (_current != null && state.Equals(_current.State))
                return;
            
            _current?.State.OnExit();
            _current = GetOrAdd(state);
            _current.State.OnEnter();
        }

        [CanBeNull]
        private TTransition GetTransition()
        {
            return _anyTransitions.FirstOrDefault(t => t.Condition.Evaluate()) ??
                   _current?.Transitions.FirstOrDefault(t => t.Condition.Evaluate());
        }

        private Node GetOrAdd(TState state) => _nodes.GetOrAdd(state, () => new(state));

        public void AddTransition(TState from, TTransition transition)
        {
            GetOrAdd(transition.To);
            GetOrAdd(from).Transitions.Add(transition);
        }

        public void AddAnyTransition(TTransition transition)
        {
            GetOrAdd(transition.To);
            _anyTransitions.Add(transition);
        }

        private class Node
        {
            public TState State { get; }
            public HashSet<TTransition> Transitions { get; } = new();
            
            public Node(TState state)
            {
                State = state;
            }
        }
    }
}