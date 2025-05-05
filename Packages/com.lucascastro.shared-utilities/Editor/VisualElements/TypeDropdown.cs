using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using UnityEditor;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace SharedUtilities.Editor.VisualElements
{
    public class TypeDropdown : DropdownField
    {
        public Type[] Types { get; }
        public Button ClearButton { get; }

        public bool ShowClearButton
        {
            get => ClearButton.visible;
            set => ClearButton.visible = value;
        }

        public TypeDropdown(Type[] types, SerializedProperty stringProperty) : 
            this(types, GetPropValueIndex(types, stringProperty))
        {
            this.RegisterValueChangedCallback(_ =>
            {
                int newIndex = index;
                string newName = newIndex >= 0 ? Types[newIndex].AssemblyQualifiedName : null;
                if (newName != stringProperty.stringValue)
                {
                    stringProperty.stringValue = newName;
                    stringProperty.serializedObject.ApplyModifiedProperties();
                }
            });
        }
        
        public TypeDropdown(Type[] types, int startIndex = -1) : base(GetChoices(types), startIndex)
        {
            Types = types.ToArray();
            ClearButton = new(() => index = -1) { text = "X" };
            Add(ClearButton);
        }
        
        private static List<string> GetChoices(Type[] types) => 
            types.Select(t => t.GetDisplayName()).ToList();

        private static int GetPropValueIndex(Type[] types, SerializedProperty property) =>
            types.FindIndex(t => t.AssemblyQualifiedName == property.stringValue);
    }
}