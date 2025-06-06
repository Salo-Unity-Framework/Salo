using System.Collections.Generic;
using UnityEngine;

namespace Salo.Infrastructure
{
    [CreateAssetMenu(fileName = "BootstrapRuntimeData", menuName = "Salo/Runtime Data/Bootstrap Runtime Data")]
    public class BootstrapRuntimeDataSO : RuntimeDataSOBase
    {
        [Tooltip("Subclasses of BootstrapResourceLoaderBase will add and remove themselves from this list")]
        public List<BootstrapResourceLoaderBase> BootstrapResourceLoaders = new();
    }
}
