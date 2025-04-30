using System.Collections.Generic;
using UnityEngine;

namespace SharedUtilities.Extensions
{
    public static class RectUtils
    {
        /// <summary>
        /// Gets the edges of a rectangle as pairs of vectors. <br/>
        /// Top, right, bottom, left
        /// </summary>
        public static IEnumerable<(Vector2 a, Vector2 b)> GetEdges(this Rect rect)
        {
            // top
            yield return (rect.min, new(rect.xMax, rect.yMin));
            // right
            yield return (rect.max, new(rect.xMax, rect.yMin));
            // bottom
            yield return (rect.max, new(rect.xMin, rect.yMax));
            // left
            yield return (rect.min, new(rect.xMin, rect.yMax));
        }
    }
}