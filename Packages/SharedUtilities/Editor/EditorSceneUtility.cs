using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SharedUtilities.Editor
{
    public static class EditorSceneUtility
    {
        public static void StartScene([NotNull] string scenePath)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
            
            if (string.IsNullOrEmpty(scenePath) || EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
            
            EditorSceneManager.OpenScene(scenePath);
        }

    }
}