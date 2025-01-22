using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SharedUtilities.Editor.Interfaces
{
    internal static class InterfaceReferenceLabelDrawer
    {
        private static readonly GUIStyle _labelStyle = new(EditorStyles.label)
        {
            font = EditorStyles.objectField.font,
            fontSize = EditorStyles.objectField.fontSize,
            fontStyle = EditorStyles.objectField.fontStyle,
            alignment = TextAnchor.MiddleRight,
            padding = new(0, 2, 0, 0)
        };

        public static void OnGUI(Rect position, Object value, Type interfaceType)
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive) - 1;
            string displayString = ShouldDisplayTypeLabel(position, value) ? $"({interfaceType.Name})" : "";
            DrawInterfaceNameLabel(position, displayString, controlID);
        }

        private static bool ShouldDisplayTypeLabel(Rect position, Object value)
        {
            bool isHovering = position.Contains(Event.current.mousePosition);
            return value == null || isHovering;
        }

        private static void DrawInterfaceNameLabel(Rect position, string displayString, int controlId)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            const int additionalLeftWidth = 3;
            const int verticalIndent = 1;

            var content = EditorGUIUtility.TrTextContent(displayString);
            var size = _labelStyle.CalcSize(content);
            var labelPos = new Rect(position)
            {
                width = size.x + additionalLeftWidth,
                height = position.height - verticalIndent * 2
            };
            labelPos.x += position.width - labelPos.width - 18;
            labelPos.y += verticalIndent;
            
            _labelStyle.Draw(labelPos, content, controlId, DragAndDrop.activeControlID == controlId, false);
        }

    }
}