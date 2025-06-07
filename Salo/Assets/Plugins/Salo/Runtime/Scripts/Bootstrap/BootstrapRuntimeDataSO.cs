using System.Collections.Generic;
using UnityEngine;

namespace Salo.Infrastructure
{
    [CreateAssetMenu(fileName = "BootstrapRuntimeData", menuName = "Salo/Runtime Data/Bootstrap Runtime Data")]
    public class BootstrapRuntimeDataSO : RuntimeDataSOBase
    {
        // Subclasses of BootstrapResourceLoaderBase will add and remove themselves from this list
        private List<BootstrapResourceLoaderBase> bootstrapResourceLoaders;
        public List<BootstrapResourceLoaderBase> BootstrapResourceLoaders
        {
            get
            {
                if (null == bootstrapResourceLoaders) bootstrapResourceLoaders = new();
                return bootstrapResourceLoaders;
            }
        }
    }
}
