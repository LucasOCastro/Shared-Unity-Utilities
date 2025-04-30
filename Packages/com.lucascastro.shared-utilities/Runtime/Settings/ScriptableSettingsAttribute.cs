using System;

namespace SharedUtilities.Settings
{
    public class ScriptableSettingsAttribute : Attribute
    {
        public string MenuPath;
        public string AssetFolderPath = "Assets/_Project/Settings/ScriptableSettings";

        public ScriptableSettingsAttribute(string menuPath)
        {
            MenuPath = menuPath;
        }
    }
}