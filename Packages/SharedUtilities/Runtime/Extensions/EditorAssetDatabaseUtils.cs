// Ideally, this should be in an editor only assembly def, but it is also used in runtime scripts 
#if UNITY_EDITOR

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SharedUtilities.Extensions
{
    public static class EditorAssetDatabaseUtils
    {
        private static void EnsureFolderExistsRecursive(string parentPath, string currentFolder)
        {
            if (parentPath == "")
                return;
            
            // If already exists, return
            if (AssetDatabase.IsValidFolder(Path.Join(parentPath, currentFolder)))
                return;
            
            EnsureFolderExistsRecursive(Path.GetDirectoryName(parentPath), Path.GetFileName(parentPath));
            AssetDatabase.CreateFolder(parentPath, currentFolder);
        }

        /// <summary>
        /// Ensures that a folder exists. <br/>
        /// If the folder already exists, nothing is done.
        /// If the folder does not exist, it is created.
        /// </summary>
        /// <param name="path">The path to the folder to ensure exists.</param>
        /// <returns>True if the folder was created, false if the folder already existed.</returns>
        public static bool EnsureFolderExists(string path)
        {
            // Make sure path is folder and not asset
            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);
            
            // If already exists, return
            Debug.Log(AssetDatabase.IsValidFolder(path));
            if (AssetDatabase.IsValidFolder(path))
                return false;
            
            // Recursively create parent folders
            string parentPath = Path.GetDirectoryName(path);
            string folderName = Path.GetFileName(path);
            EnsureFolderExistsRecursive(parentPath, folderName);
            return true;
        }

        
        /// <summary>
        /// <inheritdoc cref="EnsureFolderExists"/> <br/>
        /// Skips one frame if the folder was created.
        /// Use this when you are creating a folder and then immediately trying to access it.
        /// </summary>
        /// <returns>True if the folder was created, false if the folder already existed.</returns>
        public static bool EnsureFolderExistsAndSkipFrame(string path, Action<bool> callback)
        {
            bool created = EnsureFolderExists(path);
            if (created)
                NextFrame.Do(() => callback(true));
            else
                callback(false);

            return created;
        }
        
        /// <inheritdoc cref="EnsureFolderExistsAndSkipFrame(string, Action{bool})"/>
        public static bool EnsureFolderExistsAndSkipFrame(string path, Action callback) => 
            EnsureFolderExistsAndSkipFrame(path, _ => callback());
        
        /// <inheritdoc cref="EnsureFolderExistsAndSkipFrame(string, Action{bool})"/>
        public static async Task<bool> EnsureFolderExistsAndSkipFrameAsync(string path)
        {
            bool created = EnsureFolderExists(path);
            if (created) 
                await Task.Yield();
            
            return created;
        }
    }
}

#endif