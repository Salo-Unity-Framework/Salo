using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Salo.Infrastructure
{
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
            {
                // Method not found declared on the concrete implementing task. Call the default extension method
                await persistable.Load();
            }
        }
    }
}
