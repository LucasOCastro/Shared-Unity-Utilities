using SharedUtilities.Attributes;
using UnityEngine;
using SerializedType = SharedUtilities.SerializedType;

namespace SharedUtilitiesPackages.SerializedTypeTests
{
    public class TypeTester : MonoBehaviour
    {
        [SerializeField] private KeyCode _testKey;
        
        [SerializedType(typeof(MonoBehaviour))] public string StringMonoBehaviour;
        [SerializedType(typeof(ScriptableObject))] public string StringScriptableObject;
        
        [SerializedType(typeof(MonoBehaviour))] public SerializedType StructMonoBehaviour;
        [SerializedType(typeof(ScriptableObject))] public SerializedType StructScriptableObject;

        [SerializedType(typeof(MonoBehaviour))] public int AnotherType;

        private void Awake()
        {
            Test();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_testKey))
                Test();
        }

        private void Test()
        {
            Debug.Log("Printing string types:");
            Debug.Log(StringMonoBehaviour);
            Debug.Log(StringScriptableObject);
            
            Debug.Log("Printing struct types:");
            Debug.Log(StructMonoBehaviour.Type);
            Debug.Log(StructScriptableObject.Type);
        }
    }
}