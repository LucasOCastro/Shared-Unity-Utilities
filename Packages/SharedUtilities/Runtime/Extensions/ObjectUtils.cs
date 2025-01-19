using JetBrains.Annotations;
using UnityEngine;

namespace SharedUtilities.Extensions
{
    public static class ObjectUtils
    {
        [CanBeNull]
        public static T OrNull<T>(this T obj) where T : Object
            => obj ? obj : null;
    }
}