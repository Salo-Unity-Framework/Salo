using System;

namespace Salo.Infrastructure
{
    public static class DataPersistenceEvents
    {
        /// <summary>
        /// Request to reset all persisted data. Handled by DataPersistenceManager
        /// </summary>
        public static event Action OnResetAllAndSaveRequested;
        public static void ResetAllAndSaveRequested()
            => OnResetAllAndSaveRequested?.Invoke();
    }
}
