using System;
using System.Collections;

namespace SharedUtilities.Extensions
{
    public static class CoroutineUtils
    {
        public static IEnumerator ContinueWith(this IEnumerator enumerator, Action onComplete)
        {
            yield return enumerator;
            onComplete();
        }

        public static IEnumerator YieldNull()
        {
            yield return null;
        }
    }
}