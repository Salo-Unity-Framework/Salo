using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Salo.Infrastructure
{
    public static class ReflectionHelper
    {
        public static void SetPrivateBackingField<T, TValue>(T obj, Expression<Func<T, TValue>> propertySelector, TValue value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (propertySelector.Body is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("Expression must target a property.", nameof(propertySelector));
            }

            // Get all private fields in the type
            FieldInfo[] privateFields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            // Find the one that is actually used by the getter of the selected property
            FieldInfo backingField = privateFields.FirstOrDefault(field =>
                propertyInfo.DeclaringType
                    .GetMethod(propertyInfo.GetMethod.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    ?.GetMethodBody()?.GetILAsByteArray()
                    ?.Contains(GetFieldToken(field)) == true
            );

            if (backingField == null)
            {
                throw new MissingFieldException($"Could not find a backing field for property '{propertyInfo.Name}' in {typeof(T)}.");
            }

            // Set the value
            backingField.SetValue(obj, value);
        }

        private static byte GetFieldToken(FieldInfo field)
        {
            return (byte)(field.MetadataToken & 0xFF);
        }
    }
}
