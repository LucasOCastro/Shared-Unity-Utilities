using SharedUtilities.Interfaces;
using UnityEngine;

namespace SharedUtilitiesPackages.SerializedInterfaceTests
{
    public class InterfaceTester : MonoBehaviour
    { 
        [SerializeField] private int _a = 1;
        [SerializeField] private KeyCode _testKey = KeyCode.Space;
        
        public InterfaceReference<IInterfaceTest> InterfaceReference;

        private void Start() => Test();

        private void Update()
        {
            if (Input.GetKeyDown(_testKey))
                Test();
        }

        private void Test()
        {
            if (InterfaceReference?.Value == null)
            {
                Debug.Log("Null");
                return;
            }
            
            InterfaceReference.Value.Foo(1);
            Debug.Log($"Tested with value = {InterfaceReference.Value.Value}");
        }
    }
}