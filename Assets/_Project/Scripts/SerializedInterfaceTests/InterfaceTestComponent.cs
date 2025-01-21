using UnityEngine;

namespace SharedUtilitiesPackages.SerializedInterfaceTests
{
    public class InterfaceTestComponent : MonoBehaviour, IInterfaceTest
    {
        [SerializeField] private string _message = "Component's Test Message";

        public void Foo(int a)
        {
            Debug.Log($"{a} - {_message}");
        }

        public float Value => 1f;
    }
}