using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// A test implementation of DataPersistorSOBase. This logs data-to-save
    /// to the console. To be replaced by a proper implementation - replace
    /// on the DataPersistenceConfig asset
    /// </summary>
    [CreateAssetMenu(fileName = "LoggingPersistor", menuName = "Salo/Logging Persistor")]
    public class LoggingPersistorSO : DataPersistorSOBase
    {
        public override bool HasKey(string key)
        {
            return false;
        }

        public override bool TryClearData(string key)
        {
            return true;
        }

        public override UniTask<(bool isSuccess, string value)> TryReadString(string key)
        {
            return UniTask.FromResult((false, (string)null));
        }

        public override bool TryWriteString(string key, string value)
        {
            Debug.Log($"LoggingPersistor received data to save to {key}:\n{value}");
            return true;
        }
    }
}
