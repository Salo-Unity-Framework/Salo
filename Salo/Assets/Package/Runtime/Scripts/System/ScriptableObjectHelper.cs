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
            // Create a new instance - this will have default values
            var tempInstance = ScriptableObject.CreateInstance(obj.GetType());
            var defaultJsonString = JsonUtility.ToJson(tempInstance);

            // Assign values from the instance with default values
            JsonUtility.FromJsonOverwrite(defaultJsonString, obj);
            Object.DestroyImmediate(tempInstance);
        }
    }
}
