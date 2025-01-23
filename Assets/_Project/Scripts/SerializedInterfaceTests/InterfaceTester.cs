﻿using SharedUtilities.Interfaces;
using UnityEngine;

namespace SharedUtilitiesPackages.SerializedInterfaceTests
{
    public class InterfaceTester : MonoBehaviour
    { 
        [SerializeField] private int _a = 1;
        [SerializeField] private KeyCode _testKey = KeyCode.Space;
        
        public InterfaceReference<IInterfaceTest> GeneralRef;
        public InterfaceReference<IInterfaceTest, Component> CompRef;
        public InterfaceReference<IInterfaceTest, ScriptableObject> SORef;
        
        [RequiresInterface(typeof(IInterfaceTest))]
        public Object GeneralAttribute;
        [RequiresInterface(typeof(IInterfaceTest))]
        public Component CompAttribute;
        [RequiresInterface(typeof(IInterfaceTest))]
        public ScriptableObject SOAttribute;

        private void Start() => Test();

        private void Update()
        {
            if (Input.GetKeyDown(_testKey))
                Test();
        }

        private void Test()
        {
            if (GeneralRef?.Value == null)
            {
                Debug.Log("Null");
                return;
            }
            
            GeneralRef.Value.Foo(1);
            Debug.Log($"Tested with value = {GeneralRef.Value.Value}");
        }
    }
}