using System;
using UnityEngine.Scripting;

[Preserve] // Prevent stripping in IL2CPP since the attribute is referenced to in PersistenceHelper.SetPrivateField
[AttributeUsage(AttributeTargets.Field)]
public class PersistedAttribute : Attribute { }
