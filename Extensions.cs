using ECS_Deploy;
using Olive;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    static class Extensions
    {
        internal static bool IsArgumentsWrapper(this Type @this) => @this.GetCustomAttribute<ArgumentNamePrefixAttribute>() != null;
        internal static bool IsArgumented(this PropertyInfo @this) => @this.GetCustomAttribute<ArgumentAttribute>() != null;
        internal static ArgumentAttribute GetArgumentAttribute(this PropertyInfo @this) => @this.GetCustomAttribute<ArgumentAttribute>();
        internal static string EnsureWithSuffix(this string @this, string prefix) => @this.HasValue() ? @this.EnsureEndsWith(prefix) : @this;
    }
}
