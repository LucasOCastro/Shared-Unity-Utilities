using UnityEngine;

namespace SharedUtilities.Extensions
{
    public static class Vector3Utils
    {
        public static Vector3 Flatten(this Vector3 v) => new(v.x, 0, v.z); 
    }
}