using System;
using System.Collections.Generic;

namespace SharedUtilities.Extensions
{
    public static class ReadOnlyListUtils
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Equals(item))
                    return i;

            return -1;
        }

        public static int LastIndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i].Equals(item))
                    return i;

            return -1;
        }

        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
                if (predicate(list[i]))
                    return i;

            return -1;
        }
        
        public static int FindLastIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (predicate(list[i]))
                    return i;
            
            return -1;
        }
        
        public static T2[] MapToArray<T1, T2>(this IReadOnlyList<T1> array, Func<T1, T2> converter)
        {
            var result = new T2[array.Count];
            for (int i = 0; i < array.Count; i++) 
                result[i] = converter(array[i]);
            return result;
        }
    }
}