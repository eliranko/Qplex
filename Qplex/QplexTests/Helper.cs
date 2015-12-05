using System;
using System.Reflection;
using Qplex.Messages;

namespace QplexTests
{
    public static class Helper
    {
        private const BindingFlags BindFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        /// <summary>
        /// Uses reflection to get the field value from an object.
        /// </summary>
        /// <param name="type">The instance type.</param>
        /// <param name="instance">The instance object.</param>
        /// <param name="fieldName">The field's name which is to be fetched.</param>
        /// <param name="newValue">The desired field value.</param>
        /// <returns>The field value from the object.</returns>
        public static void SetInstanceField(Type type, object instance, string fieldName, object newValue)
        {
            var fieldInfo = type.GetField(fieldName, BindFlags);
            fieldInfo?.SetValue(instance, newValue);
        }
    }

    public class MockMessage : Message
    {
        
    }
}
