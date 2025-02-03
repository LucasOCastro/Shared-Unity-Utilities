using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SharedUtilities.Extensions
{
    public static class TaskUtils
    {
        public static Task YieldNull() => Task.Yield().AsTask();
        
        public static async Task AsTask(this YieldAwaitable yieldAwaitable) => await yieldAwaitable;

        public static void ContinueWith(this Task task, Action continuationAction) =>
            task.ContinueWith(_ => continuationAction());
    }
}