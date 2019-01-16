using System;
using System.Collections.Generic;
using System.Text;

namespace ECS_Deploy
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    class ArgumentAttribute : Attribute
    {
        public string DefaultValue;
        public string Description;
        public bool Required;

        public ArgumentAttribute(string defaultValue, bool required = false, string description = null)
        {
            DefaultValue = defaultValue;
            Description = description;
            Required = required;
        }
    }
}
