using System.Linq;
using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace Shared.Editor
{
    [InitializeOnLoad]
    public static class SceneSelectorToolbarExtension
    {
        static SceneSelectorToolbarExtension()
        {
            ToolboxEditorToolbar.OnToolbarGui += OnToolbarGui;
        }

        private static void OnToolbarGui()
        {
            GUILayout.FlexibleSpace();
            foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(scene.guid);
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