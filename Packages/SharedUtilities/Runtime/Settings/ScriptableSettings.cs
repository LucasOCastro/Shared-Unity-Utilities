﻿#if UNITY_EDITOR
using SharedUtilities.Extensions;
using UnityEditor;
#endif

using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharedUtilities.Settings
{
    public abstract class ScriptableSettings : ScriptableObject
    {
        private static readonly Dictionary<Type, ScriptableSettings> _instances = new();
        
        public static ScriptableSettings GetOrCreate(Type type)
        {
            Preconditions.CheckState(type.IsSubclassOf(typeof(ScriptableSettings)));
            Preconditions.CheckState(!type.IsAbstract);
            
            if (TryGetExistingInstance(type, out var instance))
                return instance;
            
            instance = (ScriptableSettings)CreateInstance(type);
            
            // If we're in the editor, save the asset locally
            #if UNITY_EDITOR
            string assetPath = GetAssetPathFor(type);
            EditorAssetDatabaseUtils.EnsureFolderExists(assetPath);
            AssetDatabase.CreateAsset(instance, assetPath);
            AssetDatabase.SaveAssets();
            #endif
            
            _instances[type] = instance;
            return instance;
        }
        
        public static T GetOrCreate<T>() where T : ScriptableSettings
        {
            return (T)GetOrCreate(typeof(T));
        }
        
        private static string GetAssetPathFor(Type type)
        {
            var attribute = type.GetCustomAttribute<ScriptableSettingsAttribute>();
            return $"{attribute.AssetFolderPath}/{type.Name}.asset";
        }
        //TODO must preload settings and store then from editor to application, serializing the references somehow
        private static bool TryGetExistingInstance(Type type, out ScriptableSettings instance)
        {
            // if already cached
            if (_instances.TryGetValue(type, out instance))
                return true;
            
#if !UNITY_EDITOR
            return false;
#else
            // if exists in target path
            string expectedAssetPath = GetAssetPathFor(type);
            EditorAssetDatabaseUtils.EnsureFolderExists(expectedAssetPath);
            instance = AssetDatabase.LoadAssetAtPath<ScriptableSettings>(expectedAssetPath);
            if (instance)
            {
                _instances[type] = instance;
                return true;
            }
            
            // if exists elsewhere in project
            
            var foundGuids = AssetDatabase.FindAssets($"t:{type.Name}");
            switch (foundGuids.Length)
            {
                // does not exist
                case 0:
                    return false;
                // found multiple, shouldn't happen, use first
                case > 1:
                    Debug.LogWarning($"Found multiple assets of type {type.Name}:\n" +
                                     $"{string.Join("\n", foundGuids.Select(AssetDatabase.GUIDToAssetPath))}"
                    );
                    break;
            }

            // load and cache
            string guid = foundGuids[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            instance = AssetDatabase.LoadAssetAtPath<ScriptableSettings>(path);
            _instances[type] = instance;
            
            // move to correct path
            AssetDatabase.MoveAsset(path, expectedAssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Moved {path} to {expectedAssetPath}");
            
            return true;
#endif
        }
        
        
#if UNITY_EDITOR
        public void CreateSettingsGui_Editor(VisualElement root)
        {
            var obj = new SerializedObject(this);
            root.Clear();
            CreateSettingsGui_Editor(root, obj);
        }
        
        protected virtual void CreateSettingsGui_Editor(VisualElement root, SerializedObject obj)
        {
            var inspector = new InspectorElement(obj);
            root.Add(inspector);
        }
#endif
    }
}