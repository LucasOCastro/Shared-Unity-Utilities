using System;
using SharedUtilities.Serialization;
using SharedUtilities.Serialization.Attributes;
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
        public Vector3 Vector;
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
        [SerializeReference, TypePicker(typeof(AbstractDerivedClass))] public BaseClass Empty;
        [SerializeReference, TypePicker(AllowNull = false)] public BaseClass NoNull;
        [SerializeReference, TypePicker(typeof(AbstractDerivedClass), AllowNull = false)] public BaseClass NoNullEmpty;
        [SerializeReference, TypePicker(typeof(AbstractDerivedClass2))] public BaseClass WrongStart = new BaseClass();
        [SerializeReference, TypePicker(typeof(AbstractDerivedClass2), AllowNull = false)] public BaseClass WrongStartNoNull = new BaseClass();
    }
}