using System;
using System.Collections.Generic;

namespace Shared.Extensions
{
    public static class DictionaryUtils
    {
        public static TVal GetOrAdd<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key, TVal defaultValue = default)
            => dictionary.TryGetValue(key, out var value) ? value : dictionary[key] = defaultValue;
        
        public static TVal GetOrAdd<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key, Func<TVal> valueFactory)
            => dictionary.TryGetValue(key, out var value) ? value : dictionary[key] = valueFactory();
    }
}