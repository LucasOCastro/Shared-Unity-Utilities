using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace SharedUtilities.Editor.SceneSelector
{
    [InitializeOnLoad]
    public static class SceneSelectorToolbarExtension
    {
        private static SceneSelectorToolbarSettings Settings => SceneSelectorToolbarSettings.GetOrCreate();
        
        static SceneSelectorToolbarExtension()
        {
            InjectGui();
        }

        private static void InjectGui()
        {
            ToolbarExtender.OnLeftGui -= OnToolbarGui;
            ToolbarExtender.OnRightGui -= OnToolbarGui;
            
            switch (Settings.ToolbarSide)
            {
                case SceneSelectorToolbarSettings.Side.Left:
                    ToolbarExtender.OnLeftGui += OnToolbarGui;
                    break;
                case SceneSelectorToolbarSettings.Side.Right:
                    ToolbarExtender.OnRightGui += OnToolbarGui;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static void OnToolbarGui()
        {
            if (!Settings.ShowToolbar)
                return;
            
            if (Settings.ShowSpaceBefore)
                GUILayout.FlexibleSpace();
            
            foreach (string guid in GetSceneGuids())
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                if (!sceneAsset) continue;

                float width = GUI.skin.button.CalcSize(new(sceneAsset.name)).x;
                if (GUILayout.Button(sceneAsset.name, GUILayout.Width(width)))
                {
                    EditorSceneUtility.StartScene(assetPath);
                }
            }
        }

        private static IEnumerable<string> GetSceneGuids() =>
            Settings.ScenesToShow switch
            {
                SceneSelectorToolbarSettings.SceneListMode.ListedInBuildAndEnabled =>
                    EditorBuildSettings.scenes
                        .Where(s => s.enabled)
                        .Select(s => s.guid.ToString()),
                SceneSelectorToolbarSettings.SceneListMode.ListedInBuild =>
                    EditorBuildSettings.scenes
                        .Select(s => s.guid.ToString()),
                SceneSelectorToolbarSettings.SceneListMode.AllInProject =>
                    AssetDatabase.FindAssets("t:scene",
                        Settings.AllInProjectDirectories
                            .WhereNotNullOrEmpty()
                            .ToArray()
                    ),
                _ => Enumerable.Empty<string>()
            };
    }
}