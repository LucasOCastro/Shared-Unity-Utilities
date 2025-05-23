﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using SharedUtilities.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SharedUtilities.Settings
{
    public abstract class ScriptableSettings : ScriptableObject
    {
        [Tooltip("If true, the asset will be preloaded for the build. Set to false if the setting is editor only.")]
        public bool PreloadAsset = true;
        
        private static readonly Dictionary<Type, ScriptableSettings> _instances = new();
        
        public static ScriptableSettings GetOrCreate(Type type)
        {
            Preconditions.CheckState(type.IsSubclassOf(typeof(ScriptableSettings)),
                $"Type {type.Name} is not a subclass of {nameof(ScriptableSettings)}");
            Preconditions.CheckState(!type.IsAbstract, 
                $"Type {type.Name} is abstract");
            
            //if (_instances.TryGetValue(type, out var instance))
            if (TryGetExistingInstance(type, out var instance))
                return instance;
            
            instance = (ScriptableSettings)CreateInstance(type);
            
            // If we're in the editor, save the asset locally
            #if UNITY_EDITOR
            string assetPath = GetAssetPathFor(type);
            EditorAssetDatabaseUtils.EnsureFolderExistsAndSkipFrame(assetPath, () =>
            {
                Debug.Log("Create folder and save");
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            });
            #endif
            
            _instances[type] = instance;
            return instance;
        }
        
        public static T GetOrCreate<T>() where T : ScriptableSettings
        {
            return (T)GetOrCreate(typeof(T));
        }

        static ScriptableSettings()
        {
            _instances.Clear();
        }

        private void OnEnable()
        {
            if (!this)
                return;
            
            if (TryGetExistingInstance(GetType(), out var instance) && instance)
            {
                if (instance != this)
                    Debug.LogError($"Tried adding {GetType().Name} - {this} but already contains {GetType().Name} - {instance}", this);
            }
            else _instances[GetType()] = this;
        }

        private void OnDisable()
        {
            if (_instances.ContainsValue(this))
                _instances.Remove(GetType());
        }

        private static string GetAssetPathFor(Type type)
        {
            var attribute = type.GetCustomAttribute<ScriptableSettingsAttribute>();
            return $"{attribute.AssetFolderPath}/{type.Name}.asset";
        }
        
        
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
            EditorAssetDatabaseUtils.EnsureFolderExistsAndSkipFrame(expectedAssetPath, () =>
            {
                string moveResult = AssetDatabase.MoveAsset(path, expectedAssetPath);
                Debug.Log(moveResult);
                AssetDatabase.SaveAssets();
                Debug.Log($"Moved {path} to {expectedAssetPath}: {(string.IsNullOrEmpty(moveResult) ? "Success" : moveResult)}");
            });
            
            return true;
#endif
        }
        
#if UNITY_EDITOR
        private bool? _oldPreload;
#endif
        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (_oldPreload == PreloadAsset)
                return;
            
            using var _ = ListPool<Object>.Get(out var preloaded);
            
            preloaded.AddRange(PlayerSettings.GetPreloadedAssets());
            if (PreloadAsset)
                preloaded.AddIfNotContains(this);
            else
                preloaded.Remove(this);
            PlayerSettings.SetPreloadedAssets(preloaded.ToArray());
            
            _oldPreload = PreloadAsset;
#endif
        }
    }
}