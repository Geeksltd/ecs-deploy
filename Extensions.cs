using ECS_Deploy;
using Olive;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    static class Extensions
    {
        static bool IsArgumentedType(this Type type) => type.GetCustomAttribute<ArgumentNamePrefixAttribute>() != null;
        internal static T LoadFromParameters<T>(this T @this, string prefix = null)
        {
            var actualType = typeof(T);

            if (actualType.IsArgumentedType())
                prefix = prefix.WithSuffix("-").WithSuffix(actualType.GetCustomAttribute<ArgumentNamePrefixAttribute>().Prefix);

            foreach (var prop in actualType.GetProperties())
            {
                var propertyArgumentAttribute = prop.GetCustomAttribute<ArgumentAttribute>();
                if (propertyArgumentAttribute != null)
                {
                    var parameterKey = prop.Name.SeparateAtUpperCases().Replace(" ", "-").ToLower();
                    var value = Parameters.Current.GetOrDefault(parameterKey).Or(propertyArgumentAttribute.DefaultValue);

                    if (propertyArgumentAttribute.Required && value.IsEmpty())
                        throw new Exception($"Please provide a value for {Parameters.GetArgumentName(parameterKey)}");

                    prop.SetValue(@this, Convert.ChangeType(value, prop.PropertyType));
                }
                else if (prop.PropertyType.IsArgumentedType())
                {
                    prop.GetValue(@this).LoadFromParameters();
                }
            }


            return @this;
        }
    }
}
