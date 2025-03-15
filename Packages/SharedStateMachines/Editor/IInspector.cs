using System;
using UnityEngine.UIElements;

namespace SharedUtilities.StateMachines.Editor
{
    internal interface IInspector : IDisposable
    {
        void CreateGUI(VisualElement container);
    }
}