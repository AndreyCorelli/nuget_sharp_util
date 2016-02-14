using System;

namespace SharpExtensionsUtil.Reflection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JsonClassNameAttribute : Attribute
    {
        public string Name { get; set; }

        public JsonClassNameAttribute(string name)
        {
            Name = name;
        }
    }
}
