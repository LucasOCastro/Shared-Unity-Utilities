using SharedUtilities.Settings;

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
        
        public string[] AllInProjectDirectories = { "Assets/_Project" };
        
        public static SceneSelectorToolbarSettings GetOrCreate() => GetOrCreate<SceneSelectorToolbarSettings>();
    }
}