using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SharedUtilities.Editor.Interfaces
{
    internal static class InterfaceReferenceUtils
    {
        private static readonly GUIStyle _labelStyle = new(EditorStyles.label)
        {
            font = EditorStyles.objectField.font,
            fontSize = EditorStyles.objectField.fontSize,
            fontStyle = EditorStyles.objectField.fontStyle,
            alignment = TextAnchor.MiddleRight,
            padding = new(0, 2, 0, 0)
        };
        
        public static void DrawProperty(Rect position, SerializedProperty property, GUIContent label, Type objectType,
            Type interfaceType)
        {
            var oldValue = property.objectReferenceValue;
            var selected = EditorGUI.ObjectField(position, label, oldValue, objectType, true);
            var extracted = ExtractValueAndValidate(selected, oldValue, interfaceType);
            property.objectReferenceValue = extracted;
            
            DrawInterfaceTypeLabel(position, selected, interfaceType);
        }
        
        [CanBeNull]
        public static Object ExtractValueAndValidate([CanBeNull] Object selected, [CanBeNull] Object oldValue, 
            Type interfaceType)
        {
            switch (selected)
            {
                case null:
                    return null;
                case GameObject go:
                    var component = go.GetComponent(interfaceType);
                    if (!component)
                    {
                        Debug.LogWarning($"Could not find component which implements {interfaceType.Name} on {go.name}", go);
                        return oldValue;
                    }
                    return component;
                default:
                    if (!selected.GetType().GetInterfaces().Contains(interfaceType))
                    {
                        Debug.LogWarning($"{selected.name} does not implement {interfaceType.Name}", selected);
                        return oldValue;
                    }
                    return selected;
            }
        }
        
        private static bool ShouldDisplayTypeLabel(Rect position, Object value)
        {
            bool isHovering = position.Contains(Event.current.mousePosition);
            return value == null || isHovering;
        }

        public static void DrawInterfaceTypeLabel(Rect position, Object value, Type interfaceType)
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive) - 1;
            string displayString = ShouldDisplayTypeLabel(position, value) ? $"({interfaceType.Name})" : "";
            DrawInterfaceNameLabel(position, displayString, controlID);
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