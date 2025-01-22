using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Extensions;
using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace SharedUtilities.Editor.SceneSelector
{
    [InitializeOnLoad]
    public static class SceneSelectorToolbarExtension
    {
        static SceneSelectorToolbarExtension()
        {
            ToolboxEditorToolbar.OnToolbarGui += OnToolbarGui;
        }

        private static IEnumerable<string> GetSceneGuids()
        {
            var settings = SceneSelectorToolbarSettings.GetOrCreate();
            return settings.ScenesToShow switch
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
                        settings.AllInProjectDirectories
                            .WhereNotNullOrEmpty()
                            .ToArray()
                    ),
                _ => Enumerable.Empty<string>()
            };
        }

        private static void OnToolbarGui()
        {
            GUILayout.FlexibleSpace();
            foreach (string guid in GetSceneGuids())
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                if (!sceneAsset) continue;
                
                if (GUILayout.Button(sceneAsset.name))
                {
                    EditorSceneUtility.StartScene(assetPath);
                }
            }
        }
    }
}