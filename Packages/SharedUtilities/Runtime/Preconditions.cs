using System;
using JetBrains.Annotations;
using SharedUtilities.Extensions;

namespace SharedUtilities
{
    // By 'git-amend' on YouTube
    // https://gist.github.com/adammyhre/82d495ab99e2c59a19362119b2d43194
    public static class Preconditions
    {
        public static T CheckOfType<T>(object value)
        {
            if (value is not T result)
            {
                throw new ArgumentException($"Value {value} of type {value.GetType().Name} is not of type {typeof(T).Name}.");
            }
            
            return result;
        }

        public static TValue CheckOfType<TValue>(TValue value, Type type)
        {
            if (!type.IsInstanceOfType(value))
            {
                throw new ArgumentException($"Value {value} of type {value.GetType().Name} is not of type {type.Name}.");
            }
            
            return value;
        }

        public static T CheckNotNull<T>(T reference, [CanBeNull] string message = null)
        {
            if (reference is UnityEngine.Object obj && obj.OrNull() == null)
            {
                throw new ArgumentNullException(message);
            }

            if (reference is null)
            {
                throw new ArgumentNullException(message);
            }

            return reference;
        }

        public static void CheckState(bool expression, string messageTemplate, params object[] messageArgs)
        {
            CheckState(expression, string.Format(messageTemplate, messageArgs));
        }

        public static void CheckState(bool expression, [CanBeNull] string message = null)
        {
            if (expression)
            {
                return;
            }

            throw message == null ? new InvalidOperationException() : new(message);
        }

        public static void CheckStateNamed(bool expression, string name)
        {
            CheckState(expression, $"{name} = false");
        }
    }
}