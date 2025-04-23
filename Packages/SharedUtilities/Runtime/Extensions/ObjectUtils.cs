using JetBrains.Annotations;

namespace SharedUtilities.Extensions
{
    public static class ObjectUtils
    {
        [CanBeNull]
        public static T OrNull<T>([CanBeNull] this T obj) where T : class
        {
            if (ReferenceEquals(obj, null) || obj.Equals(null))
                return null;
            return obj;
        }
    }
}