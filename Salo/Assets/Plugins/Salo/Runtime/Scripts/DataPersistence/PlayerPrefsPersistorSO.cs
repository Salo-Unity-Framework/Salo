using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// A basic implementation of DataPersistorSOBase to persist data using PlayerPrefs
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerPrefsPersistor", menuName = "Salo/PlayerPrefs Persistor")]
    public class PlayerPrefsPersistorSO : DataPersistorSOBase
    {
        public override bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public override bool TryClearData(string key)
        {
            PlayerPrefs.DeleteKey(key);
            return true;
        }

        // Note: Not making this an async method since PlayerPrefs.GetString is fast,
        // and also can only be called from the main thread.
        public override UniTask<(bool isSuccess, string value)> TryReadString(string key)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return UniTask.FromResult((false, (string)null));
            }

            string value = null;

            try
            {
                value = PlayerPrefs.GetString(key);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return UniTask.FromResult((true, value));
        }

        public override bool TryWriteString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            return true;
        }
    }
}
