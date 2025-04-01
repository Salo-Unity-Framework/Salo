using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

public static class PersistenceHelper
{
    private static Dictionary<Type, Type> wrapperTypes = new();

    public static object GetPersistedObject(ScriptableObject scriptableObject)
    {
        var wrapperType = getWrapperType(scriptableObject.GetType());
        var persistedObject = Activator.CreateInstance(wrapperType);

        // Copy the values of [Persisted] fields
        var fieldInfos = scriptableObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            // Process only [Persisted] fields
            if (!Attribute.IsDefined(fieldInfo, typeof(PersistedAttribute))) continue;

            var wrapperFieldInfo = wrapperType.GetField(fieldInfo.Name);
            wrapperFieldInfo.SetValue(persistedObject, fieldInfo.GetValue(scriptableObject));
        }

        return persistedObject;
    }

    private static Type getWrapperType(Type soType)
    {
        if (wrapperTypes.ContainsKey(soType))
        {
            return wrapperTypes[soType];
        }

        var typeName = soType.Name + "Wrapper";

        var typeBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("RuntimePersistence"), AssemblyBuilderAccess.Run)
            .DefineDynamicModule("PersistenceModule")
            .DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

        var fieldInfos = soType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            if (Attribute.IsDefined(fieldInfo, typeof(PersistedAttribute)))
            {
                typeBuilder.DefineField(fieldInfo.Name, fieldInfo.FieldType, FieldAttributes.Public);
            }
        }

        var wrapperType = typeBuilder.CreateType();
        wrapperTypes[soType] = wrapperType;

        return wrapperType;
    }
}
