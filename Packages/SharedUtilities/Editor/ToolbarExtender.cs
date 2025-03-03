using System;
using System.Collections;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SharedUtilities.Editor
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        public static event Action OnLeftGui;
        public static event Action OnRightGui;

        private static VisualElement _toolbarRoot;

        static ToolbarExtender()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(InjectGui());
        }

        private static IEnumerator FindToolbar()
        {
            Object toolbar = null;
            while (toolbar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar"));
                if (toolbars == null || toolbars.Length == 0)
                {
                    yield return null;
                    continue;
                }
                
                toolbar = toolbars[0];
            }
            
            var rootField = toolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField == null)
            {
                Debug.LogError("Could not find m_Root field on toolbar");
                yield break;
            }
            
            _toolbarRoot = rootField.GetValue(toolbar) as VisualElement;
        }

        private static IEnumerator InjectGui()
        {
            yield return FindToolbar();
            
            var left = _toolbarRoot.Q("ToolbarZoneLeftAlign");
            var right = _toolbarRoot.Q("ToolbarZoneRightAlign");

            InjectGui(left, OnLeftGui);
            InjectGui(right, OnRightGui);
        }

        private static void InjectGui(VisualElement parent, Action onGui)
        {
            var element = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                }
            };

            var container = new IMGUIContainer
            {
                style =
                {
                    flexGrow = 1,
                },
                onGUIHandler = onGui
            };

            element.Add(container);
            parent.Add(element);
        }
    }
}