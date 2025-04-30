using UnityEngine;
using UnityEngine.UIElements;

namespace SharedUtilities.VisualElements
{
    public class EditableLabel : VisualElement
    {
        public delegate void ValueChangedDelegate(ref string value);
        public event ValueChangedDelegate ValueChanged;
        
        private readonly Label _displayLabel;
        private readonly TextField _editField;
        
        private string _currentValue;
        public string Value
        {
            get => _currentValue;
            set
            {
                ValueChanged?.Invoke(ref value);
                _currentValue = value;
                _displayLabel.text = value;
            }
        }
        
        public EditableLabel(string initialText)
        {
            _currentValue = initialText;
            
            // Create display label
            _displayLabel = new(initialText);
            Add(_displayLabel);
            
            // Create hidden text field
            _editField = new()
            {
                style =
                {
                    display = DisplayStyle.None
                }
            };
            Add(_editField);
            
            _displayLabel.RegisterCallback<MouseDownEvent>(OnLabelClicked);
            _editField.RegisterCallback<FocusOutEvent>(OnEditComplete);
            _editField.RegisterCallback<KeyDownEvent>(OnEditKeyPressed);
        }

        private void OnLabelClicked(MouseDownEvent evt)
        {
            if (evt.button == 0 && evt.clickCount == 2)
                StartEditing();
        }

        private void StartEditing()
        {
            _displayLabel.style.display = DisplayStyle.None;
            _editField.style.display = DisplayStyle.Flex;
            _editField.value = _currentValue;
            _editField.Q(TextField.textInputUssName).Focus();
            _editField.SelectAll();
        }

        private void OnEditComplete(FocusOutEvent evt)
        {
            FinishEditing();
        }

        private void OnEditKeyPressed(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return or KeyCode.KeypadEnter:
                    FinishEditing();
                    break;
                case KeyCode.Escape:
                    CancelEditing();
                    break;
            }
        }

        private void FinishEditing()
        {
            Value = _editField.value;
            _editField.style.display = DisplayStyle.None;
            _displayLabel.style.display = DisplayStyle.Flex;
        }

        private void CancelEditing()
        {
            _editField.style.display = DisplayStyle.None;
            _editField.value = _currentValue;
            _displayLabel.style.display = DisplayStyle.Flex;
        }
    }
}