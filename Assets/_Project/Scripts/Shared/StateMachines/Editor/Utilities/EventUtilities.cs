using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shared.StateMachines.Editor.Utilities
{
    public static class EventUtilities
    {
        public static void SetPosition<T>(this MouseEventBase<T> ev, Vector2 position) where T: MouseEventBase<T>, new()
        {
            var prop = typeof(T)
                .GetProperty(nameof(MouseUpEvent.mousePosition), BindingFlags.Instance | BindingFlags.Public);

            if (prop == null)
                throw new MissingMemberException(typeof(MouseEventBase<>).FullName, nameof(MouseUpEvent.mousePosition));
            
            prop.SetValue(ev, position);
        }
    }
}