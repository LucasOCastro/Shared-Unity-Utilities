using SharedUtilities.Settings;
using UnityEngine;

namespace SharedUtilities.Editor.SceneSelector
{
    [ScriptableSettings("Project/Scene Selector Toolbar")]
    public class SceneSelectorToolbarSettings : ScriptableSettings
    {
        public enum SceneListMode
        {
            ListedInBuildAndEnabled = 0,
            ListedInBuild = 1,
            AllInProject = 2
        }
        
        public SceneListMode ScenesToShow = SceneListMode.ListedInBuildAndEnabled;
        
        [ReorderableList]
        [ShowIf(nameof(ScenesToShow), SceneListMode.AllInProject)]
        public string[] AllInProjectDirectories = { "Assets/_Project" };
        
        public static SceneSelectorToolbarSettings GetOrCreate() => GetOrCreate<SceneSelectorToolbarSettings>();
    }
}