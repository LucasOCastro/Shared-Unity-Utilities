using UnityEngine;
using UnityEngine.UIElements;

namespace Shared.Extensions
{
    public static class VisualElementUtils
    {
        public static void SetBorderWidth(this IStyle style, float width) =>
            style.SetBorderWidth(width, width, width, width);
        
        public static void SetBorderWidth(this IStyle style, float top, float right, float bottom, float left)
        {
            style.borderTopWidth = top;
            style.borderRightWidth = right;
            style.borderBottomWidth = bottom;
            style.borderLeftWidth = left;
        }
        
        public static void SetBorderColor(this IStyle style, Color color) =>
            style.SetBorderColor(color, color, color, color);

        public static void SetBorderColor(this IStyle style, Color top, Color right, Color bottom, Color left)
        {
            style.borderTopColor = top;
            style.borderRightColor = right;
            style.borderBottomColor = bottom;
            style.borderLeftColor = left;
        }

        public static void SetBorder(this IStyle style, float width, Color color)
        {
            style.SetBorderWidth(width);
            style.SetBorderColor(color);
        }
    }
}