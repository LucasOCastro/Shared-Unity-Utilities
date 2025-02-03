using System;
using System.Threading.Tasks;
using UnityEngine.Device;

namespace SharedUtilities.Extensions
{
    public static class NextFrame
    {
        public static void Do(Action action, bool forceEditor = false)
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
#endif

            Task.Yield().AsTask().ContinueWith(action);
        }
    }
}