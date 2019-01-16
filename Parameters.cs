using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Olive;

namespace ECS_Deploy
{
    class Parameters
    {
        Dictionary<string, string> Options;
        Parameters(IEnumerable<string> args)
        {
            Options = args.Where(i => i.StartsWith("--"))
                          .Select((item, index) => new { Key = item.TrimStart("--"), Value = args.ElementAt(args.IndexOf(item) + 1).Unless(item) })
                          .ToDictionary(i => i.Key, i => i.Value);
        }

        internal static Parameters Current { get; private set; }
        internal static void Load(IEnumerable<string> args) => Current = new Parameters(args);

        internal string this[string key] => Options.GetOrDefault(key) ?? throw new Exception($"The mandatory argument {key} not provided.");

        internal T GetOrDefault<T>(string key)
        {
            var value = Options.GetOrDefault(key);
            if (value.HasValue() == false)
                return default(T);

            return value.To<T>();
        }

        internal string GetOrDefault(string key) => GetOrDefault<string>(key.TrimStart("--"));

        internal static string GetArgumentName(string key) => key.WithPrefix("--");

        internal static string GetParametersInfo() => GetParametersInfo(typeof(DefaultSettings)).OrderByDescending(i => i.IsMandatory).ToString(Environment.NewLine);

        static IEnumerable<ParameterInfo> GetParametersInfo(Type type, string prefix = null)
        {
            prefix = GetArgumentPrefix(type, prefix);

            foreach (var p in type.GetProperties())
            {
                if (p.IsArgumented())
                {
                    var argInfo = p.GetArgumentAttribute();
                    yield return new ParameterInfo { Name = GetArgumentName(p, prefix), Description = argInfo.Description, IsMandatory = argInfo.Required };
                }
                else if (p.PropertyType.IsArgumentsWrapper())
                    foreach (var info in GetParametersInfo(p.PropertyType, prefix))
                        yield return info;
            }
        }

        internal static T LoadProperties<T>(T instance, string prefix = null)
        {
            var actualType = instance.GetType();

            if (actualType.IsArgumentsWrapper())
                prefix = GetArgumentPrefix(actualType, prefix);

            foreach (var prop in actualType.GetProperties())
            {
                if (prop.IsArgumented())
                {
                    var propertyArgumentAttribute = prop.GetArgumentAttribute();
                    var parameterKey = GetArgumentName(prop, prefix);
                    var value = Parameters.Current.GetOrDefault(parameterKey).Or(propertyArgumentAttribute.DefaultValue);

                    if (propertyArgumentAttribute.Required && value.IsEmpty())
                        throw new Exception($"Please provide a value for {parameterKey}");

                    prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType));
                }
                else if (prop.PropertyType.IsArgumentsWrapper())
                {
                    LoadProperties(prop.GetValue(instance), prefix);
                }
            }

            return instance;
        }

        static string GetArgumentName(PropertyInfo property, string prefix) => property.Name.SeparateAtUpperCases().Replace(" ", "-").ToLower().WithPrefix(prefix.EnsureEndsWith("-"));

        static string GetArgumentPrefix(Type type, string prefix = null) =>
            (prefix.EnsureWithSuffix("-") + type.GetCustomAttribute<ArgumentNamePrefixAttribute>()?.Prefix).EnsureStartsWith("--");


        class ParameterInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public bool IsMandatory { get; set; }

            public override string ToString() => $"{Name}{"*".OnlyWhen(IsMandatory)}\t {Description}";
        }
    }

}
