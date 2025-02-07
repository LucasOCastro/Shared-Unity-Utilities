﻿// Ideally, this should be in an editor only assembly def, but it is also used in runtime scripts 
#if UNITY_EDITOR

using System.IO;
using UnityEditor;

namespace SharedUtilities.Extensions
{
    public static class EditorAssetDatabaseUtils
    {
        private static void EnsureFolderExistsRecursive(string parentPath, string currentFolder)
        {
            if (parentPath == "")
                return;
            
            if (AssetDatabase.IsValidFolder(parentPath))
                return;
            
            EnsureFolderExistsRecursive(Path.GetDirectoryName(parentPath), Path.GetFileName(parentPath));
            AssetDatabase.CreateFolder(parentPath, currentFolder);
        }
        
        public static void EnsureFolderExists(string path)
        {
            // Make sure path is folder and not asset
            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);
            
            // If already valid, return
            if (AssetDatabase.IsValidFolder(path))
                return;
            
            // Recursively create parent folders
            string parentPath = Path.GetDirectoryName(path);
            string folderName = Path.GetFileName(path);
            EnsureFolderExistsRecursive(parentPath, folderName);
        }
    }
}

#endif