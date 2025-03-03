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
        static SceneSelectorToolbarExtension()
        {
            ToolbarExtender.OnRightGui += OnToolbarGui;
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

                float width = EditorStyles.toolbarButton.CalcSize(new(sceneAsset.name)).x;
                if (GUILayout.Button(sceneAsset.name, GUILayout.Width(width)))
                {
                    EditorSceneUtility.StartScene(assetPath);
                }
            }
        }
    }
}