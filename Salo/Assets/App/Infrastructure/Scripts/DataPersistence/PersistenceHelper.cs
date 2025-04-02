using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

public static class PersistenceHelper
{
    // Store dynamically created wrapper types so they can be reused. These
    // are classes with only the [Persisted] fields from the original.
    private static Dictionary<Type, Type> wrapperTypes = new();

    // Get an instance (of a dynamically created type) with only [Persisted] fields
    public static object GetPersistedObject(IPersistable persistable)
    {
        var wrapperType = getWrapperType(persistable.GetType());
        var persistedObject = Activator.CreateInstance(wrapperType);

        // Copy the values of [Persisted] fields
        var fieldInfos = persistable.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            // Process only [Persisted] fields
            if (!Attribute.IsDefined(fieldInfo, typeof(PersistedAttribute))) continue;

            var wrapperFieldInfo = wrapperType.GetField(fieldInfo.Name);
            wrapperFieldInfo.SetValue(persistedObject, fieldInfo.GetValue(persistable));
        }

        return persistedObject;
    }

    // Get the dynamically created wrapper Type. Create a new one if needed
    private static Type getWrapperType(Type persistableType)
    {
        if (wrapperTypes.ContainsKey(persistableType))
        {
            return wrapperTypes[persistableType];
        }

        var typeName = persistableType.Name + "Wrapper";

        var typeBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("RuntimePersistence"), AssemblyBuilderAccess.Run)
            .DefineDynamicModule("PersistenceModule")
            .DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

        var fieldInfos = persistableType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            if (Attribute.IsDefined(fieldInfo, typeof(PersistedAttribute)))
            {
                typeBuilder.DefineField(fieldInfo.Name, fieldInfo.FieldType, FieldAttributes.Public);
            }
        }

        var wrapperType = typeBuilder.CreateType();
        wrapperTypes[persistableType] = wrapperType;

        return wrapperType;
    }
}
