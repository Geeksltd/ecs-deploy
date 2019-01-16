using System;
using System.Collections.Generic;
using System.Linq;
using Olive;

namespace ECS_Deploy
{
    class Parameters
    {
        Dictionary<string, string> Options;
        Parameters(IEnumerable<string> args)
        {
            Options = args.Where(i => i.StartsWith("--"))
                          .Select((item, index) => new { Key = item.TrimStart("--"), Value = args.ElementAt(index + 1).Unless(item) })
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

        internal string GetOrDefault(string key) => GetOrDefault<string>(key);

        internal static string GetArgumentName(string key) => key.WithPrefix("--");

    }

}
