using System;

namespace C4d
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        public string Name;
        public PluginAttribute()
        {
        }

        public PluginAttribute(string name)
        {
            Name = name;
        }
    }
}
