using System;
using UnityEngine;

namespace SharedUtilitiesPackages.SerializedInterfaceTests
{
    [CreateAssetMenu(menuName = "Create InterfaceTestScriptableObject", fileName = "InterfaceTestScriptableObject", order = 0)]
    public class InterfaceTestScriptableObject : ScriptableObject, IInterfaceTest
    {
        [SerializeField] private string _message = "ScriptableObject's Test Message";
        
        public void Foo(int a)
        {
            Debug.Log($"{a} - {_message}");
        }
        
        public float Value => 10f;
    }
}