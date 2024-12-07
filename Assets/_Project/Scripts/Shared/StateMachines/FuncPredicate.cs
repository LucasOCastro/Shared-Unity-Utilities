using System;

namespace Shared.StateMachines
{
    public class FuncPredicate : IPredicate
    {
        private readonly Func<bool> _predicate;
        
        public FuncPredicate(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public bool Evaluate() => _predicate();
        
        public static implicit operator FuncPredicate(Func<bool> predicate) => new(predicate);
    }
}