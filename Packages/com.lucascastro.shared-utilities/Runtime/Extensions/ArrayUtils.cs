using System.Collections.Generic;
using System.Linq;

namespace SharedUtilities.Extensions
{
    public static class ArrayUtils
    {
        public static T[] With<T>(this T[] array, T item)
        {
            var result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = item;
            return result;
        }
        
        public static T[] Without<T>(this T[] array, T item)
        {
            var result = new T[array.Length - 1];
            int index = 0;
            foreach (var t in array)
            {
                if (t != null && t.Equals(item))
                    continue;
                result[index++] = t;
            }

            return result;
        }

        public static T[] With<T>(this T[] array, IReadOnlyCollection<T> items)
        {
            var result = new T[array.Length + items.Count];
            array.CopyTo(result, 0);
            items.CopyTo(result, array.Length);
            return result;
        }

        public static T[] Without<T>(this T[] array, IReadOnlyCollection<T> items)
        {
            var result = new T[array.Length - items.Count];
            int index = 0;
            foreach (var t in array)
            {
                if (items.Contains(t))
                    continue;
                result[index++] = t;
            }

            return result;
        }
    }
}