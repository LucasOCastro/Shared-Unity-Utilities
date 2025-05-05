using System;
using System.Linq;
using SharedUtilities.Editor.Extensions;
using SharedUtilities.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SharedUtilities.Editor.VisualElements
{
    public class ObjectPropertyFieldWithTypePicker : VisualElement
    {
        public SerializedProperty Property { get; }
        
        public TypeDropdown TypeDropdown { get; }
        private readonly Foldout _labelFoldout;
        private readonly VisualElement _emptyElement;
        private readonly Type[] _types;
        
        public ObjectPropertyFieldWithTypePicker(SerializedProperty property, Type[] allowedTypes)
        {
            Property = property;
            _types = allowedTypes;
            
            TypeDropdown = new(allowedTypes);
            TypeDropdown.RegisterValueChangedCallback(_ => RefreshTypeValue());
            
            _labelFoldout = new()
            {
                text = property.displayName,
                value = property.isExpanded
            };
            _labelFoldout.RegisterValueChangedCallback(evt => Property.isExpanded = evt.newValue);

            _emptyElement = new HelpBox
            {
                text = "No type has been selected.",
                messageType = HelpBoxMessageType.Info
            };
            
            Add(_labelFoldout);
            GetTypePickerContainer(_labelFoldout).Add(TypeDropdown);

            object value = Property.managedReferenceValue;
            int index = value != null ? _types.IndexOf(value.GetType()) : -1;
            TypeDropdown.index = index;
            FillFields();
        }

        private void RefreshTypeValue()
        {
            object oldValue = Property.managedReferenceValue;
            int oldIndex = _types.IndexOf(oldValue?.GetType());
            int newIndex = TypeDropdown.index;
            if (TypeDropdown.index == oldIndex)
                return;
            
            object newValue = newIndex >= 0 ? Activator.CreateInstance(_types[newIndex]) : null;
            Property.managedReferenceValue = newValue;
            Property.serializedObject.ApplyModifiedProperties();
            
            FillFields();
        }

        private void FillFields()
        {
            _labelFoldout.Clear();
            if (Property.managedReferenceValue == null)
                _labelFoldout.Add(_emptyElement);
            else
                Property.GetFieldSubElements().ForEach(_labelFoldout.Add);
            this.Bind(Property.serializedObject);
        }

        private static VisualElement GetTypePickerContainer(VisualElement root)
        {
            return root.Q<Foldout>()?.hierarchy.Children().First() ?? root.Q<Label>();
        }
    }
}