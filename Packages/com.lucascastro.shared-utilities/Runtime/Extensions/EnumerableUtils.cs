﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        
        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> col, Predicate<T> predicate) => 
            col.Where(t => !predicate(t));
        
        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> col, Func<T, int, bool> predicate) => 
            col.Where((t, i) => !predicate(t, i));
        
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> col) => 
            col.Where(t => t != null);
        
        public static IEnumerable<string> WhereNotNullOrEmpty(this IEnumerable<string> col) => 
            col.WhereNot(string.IsNullOrEmpty);
        
        public static IEnumerable<string> WhereNotNullOrWhitespace(this IEnumerable<string> col) =>
            col.WhereNot(string.IsNullOrWhiteSpace);

        public static string ToSeparatedString<T>(this IEnumerable<T> col, string separator) =>
            col.Select(x => x.ToString()).Aggregate((a, b) => a + separator + b);
        
        public static string ToCommaSeparatedString<T>(this IEnumerable<T> col) => ToSeparatedString(col, ", ");
    }
}