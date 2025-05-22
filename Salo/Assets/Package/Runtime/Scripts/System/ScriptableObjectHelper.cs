using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Salo.Infrastructure
{
    public static class ScriptableObjectHelper
    {
        /// <summary>
        /// Reset the Serializable values on a ScriptableObject asset to their type's defaults.
        /// Note there there is no super-simple way to reset the runtime values to
        /// the on-disk values in builds. Instead, each concrete implementation of
        /// IPersistable will have its own Reset. Most RuntimeData assets will be
        /// fine with just resetting to default values with this method. Configs
        /// however, will have non-default values.
        /// </summary>
        public static void ResetToTypeDefaults(ScriptableObject obj)
        {
            Assert.IsNotNull(obj);

            // Reset instance's public and private fields
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var type = obj.GetType();
            foreach (var field in type.GetFields(flags))
            {
                // Skip fields marked with [NonSerialized]
                if (field.IsNotSerialized) continue;

                // Default value is a new instance for Value types, null for others
                var defaultValue = field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null;

                field.SetValue(obj, defaultValue);
            }
        }
    }
}
