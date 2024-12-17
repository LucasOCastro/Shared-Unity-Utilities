using System.Collections.Generic;
using UnityEngine;

namespace Shared.Extensions
{
    public static class RectUtils
    {
        /// <summary>
        /// Gets the edges of a rectangle as pairs of vectors.
        /// </summary>
        public static IEnumerable<(Vector2 a, Vector2 b)> GetEdges(this Rect rect)
        {
            // top
            yield return (rect.min, new(rect.xMax, rect.yMin));
            // left
            yield return (rect.min, new(rect.xMin, rect.yMax));
            // bottom
            yield return (rect.max, new(rect.xMin, rect.yMax));
            // right
            yield return (rect.max, new(rect.xMax, rect.yMin));
        }
    }
}