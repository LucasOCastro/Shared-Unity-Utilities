using System.Collections.Generic;

namespace SharedUtilities.Extensions
{
    public static class ListUtils
    {
        public static void AddIfNotContains<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }
    }
}