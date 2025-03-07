﻿using UnityEditor;
using UnityEngine;

namespace SharedUtilities.StateMachines.Editor
{
    [CustomEditor(typeof(BaseStateMachineAsset), editorForChildClasses: true)]
    public class StateMachineEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                base.OnInspectorGUI();
            }
            
            if (target is not BaseStateMachineAsset asset)
                return;
            
            if (GUILayout.Button("Open Editor"))
                StateMachineEditorWindow.Open(asset);

            if (GUILayout.Button("Clear"))
            {
                asset.anyNodePosition = Vector2.zero;
                
                asset.states.ForEach(DestroyAsset);
                asset.states.Clear();
                
                asset.transitions.ForEach(DestroyAsset);
                asset.transitions.Clear();
                
                if (asset.initialState)
                    DestroyAsset(asset.initialState);
                asset.initialState = null;
                
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }
        }
        
        private static void DestroyAsset(Object obj)
        {
            if (!obj) return;
            DestroyImmediate(obj, true);
        }
    }
}