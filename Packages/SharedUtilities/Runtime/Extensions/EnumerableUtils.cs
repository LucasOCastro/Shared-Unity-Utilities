using System;
using System.Collections.Generic;

namespace SharedUtilities.Extensions
{
    public static class EnumerableUtils
    {
        public static void CopyTo<T>(this IEnumerable<T> from, IList<T> to, int index = 0)
        {
            foreach (var item in from)
                to[index++] = item;
        }
        
        public static int IndexOf<T>(this IEnumerable<T> col, T item)
        {
            int i = 0;
            foreach (var t in col)
            {
                if (t.Equals(item))
                    return i;
                i++;
            }

            return -1;
        }

        public static int LastIndexOf<T>(this IEnumerable<T> col, T item)
        {
            int i = 0;
            int result = -1;
            foreach (var t in col)
            {
                if (t.Equals(item))
                    result = i;
                i++;
            }

            return result;
        }
        
        public static int FindIndex<T>(this IEnumerable<T> col, Predicate<T> predicate)
        {
            int i = 0;
            foreach (var t in col)
            {
                if (predicate(t))
                    return i;
                i++;
            }
            
            return -1;
        }
        
        public static int FindLastIndex<T>(this IEnumerable<T> col, Predicate<T> predicate)
        {
            int i = 0;
            int result = -1;
            foreach (var t in col)
            {
                if (predicate(t))
                    result = i;
                i++;
            }
            
            return result;
        }
        
        public static void ForEach<T>(this IEnumerable<T> col, Action<T> action)
        {
            foreach (var t in col)
                action(t);
        }
        
        public static void ForEach<T>(this IEnumerable<T> col, Action<T, int> action)
        {
            int i = 0;
            foreach (var t in col)
                action(t, i++);
        }
    }
}