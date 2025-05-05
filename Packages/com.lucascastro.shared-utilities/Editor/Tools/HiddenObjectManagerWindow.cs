using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SharedUtilities.Editor.Tools
{
    public class HiddenObjectManagerWindow : EditorWindow
    {
        private readonly List<GameObject> _hiddenObjects = new();
        private ListView _list;

        [MenuItem("Tools/Hidden Object Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<HiddenObjectManagerWindow>("Hidden Objects", true);
            window.minSize = new(300, 400);
            window.maxSize = new(600, 800);
        }
        
        private void UpdateHiddenObjects()
        {
            if (_list == null)
                return;
            
            var hiddenObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(obj => (obj.hideFlags & HideFlags.HideInHierarchy) != 0);
            _hiddenObjects.Clear();
            _hiddenObjects.AddRange(hiddenObjects);
            _list.RefreshItems();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            root.Add(new Label("Hidden Objects")
            {
                style =
                {
                    alignSelf = Align.Center,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });

            _list = new(_hiddenObjects)
            {
                headerTitle = "Hidden Objects",
                showAddRemoveFooter = false,
                makeItem = MakeItem,
                bindItem = BindItem,
                unbindItem = UnbindItem,
                makeNoneElement = static () => new HelpBox("No hidden objects in scene", HelpBoxMessageType.Info),
                makeFooter = MakeButtons,
                selectionType = SelectionType.Multiple,
                showBorder = true,
            };
            root.Add(_list);
            
            // Handle hotkeys
            _list.RegisterCallback<KeyDownEvent>(ev =>
            {
                switch (ev.keyCode)
                {
                    case KeyCode.Delete:
                        DestroySelected();
                        break;
                    case KeyCode.A:
                        SelectAll();
                        break;
                    case KeyCode.R:
                        UpdateHiddenObjects();
                        break;
                    default:
                        return;
                }
                ev.StopPropagation();
            });
            
            // Always maintain focus
            _list.RegisterCallback<FocusOutEvent>(_ => NextFrame.Do(() => _list.Focus()));
            _list.Focus();
            
            // Select selection and ping newest
            _list.selectionChanged += objs =>
            {
                Selection.objects = objs.OfType<Object>().ToArray();
            
                if (_list.selectedItem is GameObject mainObj && mainObj.transform.parent)
                    EditorGUIUtility.PingObject(mainObj.transform.parent.gameObject);
            };
            
            UpdateHiddenObjects();
        }

        private VisualElement MakeItem()
        {
            var label = new Label("-");
            return label;
        }
        
        private void BindItem(VisualElement element, int index)
        {
            var label = (Label)element;
            var obj = _hiddenObjects[index];
            string text = obj.name;
            if (obj.transform.parent)
                text += $" ({obj.transform.parent.name})";
            label.text = text;
        }

        private void UnbindItem(VisualElement element, int index)
        {
            var label = (Label)element;
            label.text = "-";
        }

        private VisualElement MakeButtons()
        {
            var horizontal = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            
            var selectAllButton = new Button(SelectAll) { text = "Select All", tooltip = "A" };
            horizontal.Add(selectAllButton);
            
            var destroyButton = new Button(DestroySelected) { text = "Destroy", tooltip = "DEL"};
            horizontal.Add(destroyButton);
            
            var refreshButton = new Button(UpdateHiddenObjects) { text = "Refresh", tooltip = "R" };
            horizontal.Add(refreshButton);
            
            return horizontal;
        }
        
        private void SelectAll()
        {
            var indices = Enumerable.Range(0, _hiddenObjects.Count);
            _list.SetSelection(indices);
        }
        
        private void DestroySelected()
        {
            _list.selectedItems.OfType<Object>().ForEach(Undo.DestroyObjectImmediate);
            UpdateHiddenObjects();
        }
    }
}