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

        public enum Side
        {
            Left = 0,
            Right = 1
        }
        
        public bool ShowToolbar = true;
        
        public bool ShowSpaceBefore = true;
        
        [Tooltip("Needs a recompile to take effect")]
        public Side ToolbarSide = Side.Right;
        
        public SceneListMode ScenesToShow = SceneListMode.ListedInBuildAndEnabled;
        
        public string[] AllInProjectDirectories = { "Assets/_Project" };
        
        public static SceneSelectorToolbarSettings GetOrCreate() => GetOrCreate<SceneSelectorToolbarSettings>();
    }
}