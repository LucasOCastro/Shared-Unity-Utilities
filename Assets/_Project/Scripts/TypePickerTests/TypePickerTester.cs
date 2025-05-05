using System;
using SharedUtilities.Serialization;
using UnityEngine;

namespace SharedUtilitiesPackages.TypePickerTests
{
    [Serializable]
    public class BaseClass
    {
        public int BaseClassA;
    }

    [Serializable]
    public class DerivedClass : BaseClass
    {
        public int DerivedClassA;
    }

    [Serializable]
    public abstract class AbstractDerivedClass : BaseClass
    {
        public int AbstractDerivedClassA;
    }
    
    [Serializable]
    public class AbstractDerivedClass2 : AbstractDerivedClass
    {
        public int AbstractDerivedClass2A;
    }
    
    public class TypePickerTester : MonoBehaviour
    {
        [SerializeReference, TypePicker] public BaseClass AnyTypeBasically;
        [SerializeReference, TypePicker] public AbstractDerivedClass FromAbstract;
        [SerializeReference, TypePicker(typeof(AbstractDerivedClass2))] public BaseClass Limited;
    }
}