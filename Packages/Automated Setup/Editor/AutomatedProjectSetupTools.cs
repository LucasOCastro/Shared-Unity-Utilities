using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace AutomatedSetup.Editor
{
    public static class AutomatedProjectSetupTools
    {
        [MenuItem("Tools/Setup/Import Essential Assets")]
        private static void ImportEssentialAssets()
        {
            Assets.ImportAsset("Kyrylo Kuzyk/Editor ExtensionsAnimation", 
                "PrimeTween High-Performance Animations and Sequences.unitypackage");
            
            Assets.ImportAsset("Staggart Creations/Editor ExtensionsUtilities", 
                "Replace Selected.unitypackage",
                "Selection History.unitypackage");
            
            Assets.ImportAsset("Toaster Head/Editor ExtensionsUtilities", 
                "Better Hierarchy.unitypackage");
            
            /*Assets.ImportAsset("v0lt/ScriptingGUI", 
                "EditorAttributes.unitypackage");*/
        }
        
        [MenuItem("Tools/Setup/Install Essential Packages")]
        private static void InstallEssentialPackages()
        {
            Packages.InstallPackages(new[]
            {
                "git+https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
                "git+https://github.com/adammyhre/Unity-Utils.git",
                "git+https://github.com/arimger/Unity-Editor-Toolbox.git#upm"
            });
        }
        
        [MenuItem("Tools/Setup/Setup Folder Structure")]
        private static void SetupFolderStructure()
        {
            Folders.Create("_Project", "Animation", "Art", "Materials", "Prefabs", "Scripts");
            AssetDatabase.Refresh();
            
            Folders.Move("_Project", "Scenes");
            Folders.Move("_Project", "Settings");
            Folders.Delete("TutorialInfo");
            AssetDatabase.Refresh();

            const string oldPathToInputActions = "Assets/InputSystem_Actions.inputactions";
            const string newPathToInputActions = "Assets/_Project/Settings/InputSystem_Actions_Original.inputactions";
            AssetDatabase.MoveAsset(oldPathToInputActions, newPathToInputActions);

            const string pathToReadme = "Assets/Readme.asset";
            AssetDatabase.DeleteAsset(pathToReadme);

            AssetDatabase.Refresh();
        }

        private static class Assets
        {
            public static void ImportAsset(string folder, params string[] assets)
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string assetsFolder = System.IO.Path.Combine(basePath, "Unity/Asset Store-5.x");
            
                foreach (string asset in assets)
                {
                    AssetDatabase.ImportPackage(System.IO.Path.Combine(assetsFolder, folder, asset), false);
                }
            }
        }
        
        private static class Packages
        {
            private static async Task InstallPackagesAsync(Queue<string> packagesToInstall)
            {
                while (packagesToInstall.Count > 0)
                {
                    var request = Client.Add(packagesToInstall.Dequeue());
                    while (!request.IsCompleted)
                        await Task.Delay(1000);

                    if (request.Status == StatusCode.Success)
                        Debug.Log("Installed: " + request.Result.packageId);
                    else if (request.Status >= StatusCode.Failure) 
                        Debug.LogError(request.Error.message);
                    
                    await Task.Delay(1000);
                }
                
                Debug.Log($"Finished installing packages.");
            }

            public static void InstallPackages(string[] packages)
            {
                _ = InstallPackagesAsync(new(packages));
            }
        }
        
        private static class Folders
        {
            public static void Delete(string folderName)
            {
                string pathToDelete = $"Assets/{folderName}";
                
                if (AssetDatabase.IsValidFolder(pathToDelete)) 
                    AssetDatabase.DeleteAsset(pathToDelete);
            }
            
            public static void Move(string newParent, string folderName)
            {
                string sourcePath = $"Assets/{folderName}";
                if (!AssetDatabase.IsValidFolder(sourcePath))
                    return;
                
                string destinationPath = $"Assets/{newParent}/{folderName}";
                string error = AssetDatabase.MoveAsset(sourcePath, destinationPath);
                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"Failed to move {folderName}: {error}");
            }
            
            private static void CreateSubFolders(string root, string folderHierarchy)
            {
                var folders = folderHierarchy.Split('/');
                var currentPath = root;
                foreach (var folder in folders)
                {
                    currentPath = System.IO.Path.Combine(currentPath, folder);
                    if (!System.IO.Directory.Exists(currentPath))
                    {
                        System.IO.Directory.CreateDirectory(currentPath);
                    }
                }
            }
            public static void Create(string root, params string[] folders)
            {
                var fullPath = System.IO.Path.Combine(Application.dataPath, root);
                if (!System.IO.Directory.Exists(fullPath))
                {
                    System.IO.Directory.CreateDirectory(fullPath);
                }
                
                foreach (string folder in folders)
                {
                    CreateSubFolders(fullPath, folder);
                }
            }
        }
    }
}