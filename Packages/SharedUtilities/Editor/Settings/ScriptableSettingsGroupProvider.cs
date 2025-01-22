using System;
using System.Linq;
using System.Reflection;
using SharedUtilities.Settings;
using UnityEditor;
using UnityEngine.Pool;

namespace SharedUtilities.Editor.Settings
{
    internal static class ScriptableSettingsGroupProvider
    {
        [SettingsProviderGroup]
        public static SettingsProvider[] GetSettingsProviders()
        {
            var types = TypeCache.GetTypesWithAttribute<ScriptableSettingsAttribute>()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ScriptableSettings)));
         
            using var _ = ListPool<SettingsProvider>.Get(out var result);
            result.AddRange(types.Select(CreateFor));

            return result.ToArray();
        }
        
        private static SettingsProvider CreateFor(Type type)
        {
            var attribute = type.GetCustomAttribute<ScriptableSettingsAttribute>();
            ScriptableSettings settings = ScriptableSettings.GetOrCreate(type);
            return new(attribute.MenuPath, SettingsScope.Project)
            {
                activateHandler = (_, rootElement) => settings.CreateSettingsGui_Editor(rootElement)
            };
        }
    }
}