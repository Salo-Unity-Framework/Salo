using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// Base class for Config SOs. Subclass SOs should contain data
    /// that are static (mostly) during runtime, as opposed to
    /// Runtime SOs that will contain runtime data
    /// </summary>
    public abstract class ConfigSOBase : ScriptableObject
    {
    }
}
