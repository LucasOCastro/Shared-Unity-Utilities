using System;
using System.Threading.Tasks;
using SharedUtilities.Extensions;
using UnityEngine.Device;

namespace SharedUtilities
{
    public static class NextFrame
    {
        // TODO might use a service locator or injection
        public static void Do(Action action, bool forceEditor = false, bool inBuild = true)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying || forceEditor)
            {
                UnityEditor.EditorApplication.update += ActionThenUnregister;

                return;

                void ActionThenUnregister()
                {
                    action();
                    UnityEditor.EditorApplication.update -= ActionThenUnregister;
                }
            }
#else
            if (!inBuild)
                return;
#endif

            Task.Yield().AsTask().ContinueWith(action);
        }
    }
}