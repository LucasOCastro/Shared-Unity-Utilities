using UnityEngine;

namespace SharedUtilities.Extensions
{
    public static class Vector2Utils
    {
        public static Vector3 ToVector3WithZ(this Vector2 v, float z) => new(v.x, v.y, z);
        public static Vector3 ToVector3WithY(this Vector2 v, float y) => new(v.x, y, v.y);
    }
}