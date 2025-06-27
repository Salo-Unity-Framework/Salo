using Cysharp.Threading.Tasks;
using Salo.SimpleJSON;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

namespace Salo.Infrastructure
{
    public static partial class PersistenceHelper
    {
        // For each persistable type, cache the list of non-[Persisted] fields
        // to avoid using reflection every time to get the fields.
        private static readonly Dictionary<Type, string[]> nonPersistedFields = new();

        // Get a JSON string of only the [Persisted] fields
        public static string GetPersistedJson(IPersistable persistable)
        {
            // Convert all to JSON to leverage Unity's JSON coversion
            var fullJson = JsonUtility.ToJson(persistable);

            // Parse into SimpleJSON's JSONObject so it can be operated on
            var jsonObject = JSON.Parse(fullJson) as JSONObject;

            // Remove the non-[Persisted] fields
            var nonPersistedFieldNames = getNonPersisedFields(persistable.GetType());
            foreach (var key in nonPersistedFieldNames)
            {
                jsonObject.Remove(key);
            }

            return jsonObject.ToString();
        }

        private static string[] getNonPersisedFields(Type type)
        {
            if (nonPersistedFields.ContainsKey(type))
            {
                return nonPersistedFields[type];
            }

            var list = ListPool<string>.Get();
            list.Clear();

            var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                // Save fields without the [Persisted] attribute
                if (!Attribute.IsDefined(fieldInfo, typeof(PersistedAttribute)))
                {
                    list.Add(fieldInfo.Name);
                }
            }

            nonPersistedFields[type] = list.ToArray();
            return nonPersistedFields[type];
        }

        // If a persistable is cast as IPersistable, calling Save or Load on it will call the extension method instead
        // of the method on the concrete implementation. Make sure to call the concrete class's method if it exists.
        public static void CallConcreteSave(IPersistable persistable)
        {
            var type = persistable.GetType(); // The concrete type
            var methodName = nameof(PersistableExtensions.Save); // We just need the name as string
            var methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            if (null != methodInfo)
            {
                // If found, call the "override" method on the concrete type
                methodInfo.Invoke(persistable, null);
            }
            {
                // Method not found declared on the concrete implementing task. Call the default extension method
                persistable.Save();
            }
        }

        // If a persistable is cast as IPersistable, calling Save or Load on it will call the extension method instead
        // of the method on the concrete implementation. Make sure to call the concrete class's method if it exists.
        public static async UniTask CallConcreteLoad(IPersistable persistable)
        {
            var type = persistable.GetType(); // The concrete type
            var methodName = nameof(PersistableExtensions.Load); // We just need the name as string
            var methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            if (null != methodInfo)
            {
                // If found, call the "override" method on the concrete type
                var result = methodInfo.Invoke(persistable, null);
                if (result is UniTask task)
                {
                    await task;
                }
            }
            else 
            {
                // Method not found declared on the concrete implementing task. Call the default extension method
                await persistable.Load();
            }
        }
    }

    //public static partial class PersistenceHelper
    //{
    //    // Obsolete code is moved here

    //    // Store dynamically created wrapper types so they can be reused. These
    //    // are classes with only the [Persisted] fields from the original.
    //    [Obsolete("Used by the obsolete GetPersistedObject method")]
    //    private static Dictionary<Type, Type> wrapperTypes = new();

    //    // Get an instance (of a dynamically created type) with only [Persisted] fields
    //    [Obsolete("Use GetPersistedJson instead. This uses System.Reflection.Emit which is unsupooreted on IL2CPP")]
    //    public static object GetPersistedObject(IPersistable persistable)
    //    {
    //        var wrapperType = getWrapperType(persistable.GetType());
    //        var persistedObject = Activator.CreateInstance(wrapperType);

    //        // Copy the values of [Persisted] fields
    //        var fieldInfos = persistable.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    //        foreach (FieldInfo fieldInfo in fieldInfos)
    //        {
    //            // Process only [Persisted] fields
    //            if (!Attribute.IsDefined(fieldInfo, typeof(PersistedAttribute))) continue;

    //            var wrapperFieldInfo = wrapperType.GetField(fieldInfo.Name);
    //            wrapperFieldInfo.SetValue(persistedObject, fieldInfo.GetValue(persistable));
    //        }

    //        return persistedObject;
    //    }

    //    // Get the dynamically created wrapper Type. Create a new one if needed
    //    [Obsolete("Used by the obsolete GetPersistedObject method")]
    //    private static Type getWrapperType(Type persistableType)
    //    {
    //        if (wrapperTypes.ContainsKey(persistableType))
    //        {
    //            return wrapperTypes[persistableType];
    //        }

    //        var typeName = persistableType.Name + "Wrapper";

    //        var typeBuilder = AssemblyBuilder
    //            .DefineDynamicAssembly(new AssemblyName("RuntimePersistence"), AssemblyBuilderAccess.Run)
    //            .DefineDynamicModule("PersistenceModule")
    //            .DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

    //        var fieldInfos = persistableType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //        foreach (FieldInfo fieldInfo in fieldInfos)
    //        {
    //            if (Attribute.IsDefined(fieldInfo, typeof(PersistedAttribute)))
    //            {
    //                typeBuilder.DefineField(fieldInfo.Name, fieldInfo.FieldType, FieldAttributes.Public);
    //            }
    //        }

    //        var wrapperType = typeBuilder.CreateType();
    //        wrapperTypes[persistableType] = wrapperType;

    //        return wrapperType;
    //    }
    //}
}
