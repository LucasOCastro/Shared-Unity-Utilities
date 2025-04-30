using System;
using SharedUtilities.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;
using SerializedType = SharedUtilities.SerializedType;

namespace SharedUtilitiesPackages.SerializedTypeTests
{
    public class TypeTester : MonoBehaviour
    {
        [SerializeField] private KeyCode _testKey = KeyCode.Space;
        
        [Header("String Tests")]
        [SerializedType(typeof(MonoBehaviour))] public string StringMonoBehaviour;
        [SerializedType(typeof(ScriptableObject))] public string StringScriptableObject;
        
        [Header("Struct Tests")]
        [SerializedType(typeof(MonoBehaviour))] public SerializedType StructMonoBehaviour;
        [SerializedType(typeof(ScriptableObject))] public SerializedType StructScriptableObject;
        
        [Header("Filter Tests")]
        [SerializedType(typeof(MonoBehaviour), nameof(IsTypeAllowed))] public string StringWithFilterMb;
        [SerializedType(typeof(ScriptableObject), nameof(IsTypeAllowed))] public string StringWithFilterSo;
        [SerializedType(typeof(Object), nameof(IsTypeAllowed))] public SerializedType StructWithFilter;
        [SerializedType(typeof(MonoBehaviour), "Nah")] public string InvalidFilter;
        
        

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
            Debug.Log(StructMonoBehaviour.Type + " - " + StructMonoBehaviour.AssemblyQualifiedName);
            Debug.Log(StructScriptableObject.Type + " - " + StructScriptableObject.AssemblyQualifiedName);
        }
        
        private static bool IsTypeAllowed(Type type) => type.Name.StartsWith('A');
    }
}