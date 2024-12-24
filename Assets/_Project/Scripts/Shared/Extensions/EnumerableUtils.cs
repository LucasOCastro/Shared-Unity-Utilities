using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions
{
    public static class EnumerableUtils
    {
        public static IEnumerable<T1> Except<T1, T2>(this IEnumerable<T1> col, T2 obj) => 
            col.Where(x => obj != null ? !obj.Equals(x) : x != null);

        public static void ForEach<T>(this IEnumerable<T> col, Action<T> action)
        {
            foreach (var item in col)
                action(item);
        }
        
        public static void ForEach<T>(this IEnumerable<T> col, Action<T, int> action)
        {
            var index = 0;
            foreach (var item in col)
                action(item, index++);
        }
    }
}