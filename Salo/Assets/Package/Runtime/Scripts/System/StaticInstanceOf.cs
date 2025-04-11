using System;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// Singleton type implementation for static references to bootstrapped systems.
    /// Since this is meant for bootstrapped systems, duplicate instances are not
    /// expected (as is in Singletons), and should be corrected if encountered.
    /// </summary>
    public abstract class StaticInstanceOf<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    Debug.LogWarning($"{typeof(T).Name}'s instance is null. Either it is not bootstrapped, or it is accessed before or on its Awake.");
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (null != instance)
            {
                // Duplicate instances are not expected at all and are not
                // part of the architecture design. So no Destroys
                throw new Exception($"Duplicate instance of {typeof(T).Name} encountered");
            }

            instance = this as T;
        }
    }
}
