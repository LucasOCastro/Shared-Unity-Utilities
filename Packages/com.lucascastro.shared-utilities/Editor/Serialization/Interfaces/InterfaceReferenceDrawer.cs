﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtilities.Editor.Extensions;
using SharedUtilities.Extensions;
using SharedUtilities.Serialization;
using UnityEditor;
using UnityEngine;

namespace SharedUtilities.Editor.Serialization.Interfaces
{
    [CustomPropertyDrawer(typeof(InterfaceReference<,>))]
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        private const string UnderlyingValuePropertyName = nameof(InterfaceReference<object>.UnderlyingValue);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var (interfaceType, objectType) = GetTypeArguments(fieldInfo.FieldType);
            if (interfaceType == null || objectType == null)
            {
                Debug.LogError($"Could not get {typeof(InterfaceReference<,>).Name} type arguments for {fieldInfo.FieldType.Name}");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            
            var underlyingProp = property.FindBackingFieldPropertyRelative(UnderlyingValuePropertyName);
            InterfaceReferenceUtils.DrawProperty(position, underlyingProp, label, objectType, interfaceType);
            
            EditorGUI.EndProperty();

        }
        
        private static (Type interfaceType, Type objectType) GetTypeArguments(Type fieldType)
        {
            // Extract inner type from collection
            var collectionInterface = fieldType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericTypeWithDefinition(typeof(IReadOnlyCollection<>)));
            if (collectionInterface != null)
                fieldType = collectionInterface.GetGenericArguments()[0]; 
            
            // Make sure we have the type with two generic arguments
            if (fieldType.IsGenericTypeWithDefinition(typeof(InterfaceReference<>)))
                fieldType = fieldType.BaseType;
            
            if (fieldType == null || !fieldType.IsGenericTypeWithDefinition(typeof(InterfaceReference<,>)))
            {
                Debug.LogError($"Expected {typeof(InterfaceReference<,>).Name} but got {fieldType?.Name}");
                return default;
            }
            
            var genericArguments = fieldType.GetGenericArguments();
            return (genericArguments[0], genericArguments[1]);
        }
    }
}