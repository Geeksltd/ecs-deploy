using System;
using System.Collections.Generic;
using System.Text;

namespace ECS_Deploy
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    class ArgumentNamePrefixAttribute : Attribute
    {
        public string Prefix { get; set; }

        public ArgumentNamePrefixAttribute()
        {

        }

        public ArgumentNamePrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}
