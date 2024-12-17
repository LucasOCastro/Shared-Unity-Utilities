using System.Linq;
using UnityEngine;

namespace Shared.Extensions
{
    public static class RayUtils
    {
        public static Ray2D FromPoints(Vector2 start, Vector2 end) => new(start, end - start);
        
        /// <summary>
        /// Gets the t value of the intersection point between two line segments described as rays.
        /// Returns float.NaN if there is no intersection.
        /// </summary>
        /// <param name="ray1">The first ray.</param>
        /// <param name="ray2">The second ray.</param>
        public static float Intersection(this Ray2D ray1, Ray2D ray2)
        {
             Vector2 a1 = ray1.origin;
             Vector2 a2 = ray1.origin + ray1.direction;
             Vector2 b1 = ray2.origin;
             Vector2 b2 = ray2.origin + ray2.direction;
            
            // The lines are defined by two points each: a1 and a2 for A, b1 and b2 for B.
            // The intersection point is the solution to the system of equations:
            //   a1 + t * (a2 - a1) = b1 + u * (b2 - b1)
            //   t \in [0, 1] and u \in [0, 1]
            
            // Calculate the denominator of the intersection formula
            // denominator = (b2.x - b1.x) * (a2.y - a1.y) - (b2.y - b1.y) * (a2.x - a1.x)
            // If the denominator is 0, lines are parallel or coincident
            float denominator = (b2.x - b1.x) * (a2.y - a1.y) - (b2.y - b1.y) * (a2.x - a1.x);

            // If the denominator is 0, lines are parallel or coincident
            if (denominator == 0)
                return float.NaN; 
            
            // t represents the relative position on line A (from a1 to a2)
            float t = ((b2.x - b1.x) * (a1.y - b1.y) + (b2.y - b1.y) * (b1.x - a1.x)) / denominator;
            return t;
        }

        /// <summary>
        /// Calculates the intersection point of the ray with the rectangle edges.
        /// The intersection point is given by <paramref name="ray"/>.origin + <paramref name="ray"/>.direction * t.
        /// Returns float.NaN if there is no intersection.
        /// </summary>
        public static float Intersection(this Ray2D ray, Rect rect)
        {
            return rect.GetEdges()
                .Select(e => FromPoints(e.a, e.b))
                .Select(e => ray.Intersection(e))
                .Where(t => !float.IsNaN(t) && t >= 0)
                .DefaultIfEmpty(float.NaN)
                .Min();
        }
    }
}