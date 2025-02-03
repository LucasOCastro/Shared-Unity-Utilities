using System;
using UnityEngine.Events;

namespace SharedUtilities.Extensions
{
    public static class EventUtils
    {
        //TODO turn this into roslyn stuff
        public static void RegisterOnce(this UnityEvent ev, Action action)
        {
            ev.AddListener(LocalAction);
            
            return;
            
            void LocalAction()
            {
                ev.RemoveListener(LocalAction);
                action();
            }
        }
        
        public static void RegisterOnce(Action action, Action<Action> addAction, Action<Action> removeAction)
        {
            addAction(LocalAction);

            return; 
            
            void LocalAction()
            {
                removeAction(LocalAction);
                action();
            }
        }
    }
}