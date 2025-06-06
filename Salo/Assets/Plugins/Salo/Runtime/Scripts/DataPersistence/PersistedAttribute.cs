using System;
using UnityEngine.Scripting;

namespace Salo.Infrastructure
{
    [Preserve] // Prevent stripping in IL2CPP since the attribute is referenced to in PersistenceHelper.SetPrivateField
    [AttributeUsage(AttributeTargets.Field)]
    public class PersistedAttribute : Attribute { }
}
