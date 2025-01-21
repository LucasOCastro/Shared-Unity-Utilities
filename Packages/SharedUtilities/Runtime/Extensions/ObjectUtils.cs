using JetBrains.Annotations;

namespace SharedUtilities.Extensions
{
    public static class ObjectUtils
    {
        [CanBeNull]
        public static T OrNull<T>(this T obj) where T : class
            => obj.Equals(null) ? null : obj;
    }
}