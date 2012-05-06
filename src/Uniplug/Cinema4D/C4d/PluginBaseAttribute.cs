using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4d
{
    public class PluginBaseAttribute : Attribute
    {
        public int ID;
        public string Name;
        public string IconFile;

        public PluginBaseAttribute(int id)
        {
            ID = id;
        }
        public PluginBaseAttribute(int id, string name)
        {
            ID = id;
            Name = name;
        }
        public PluginBaseAttribute(int id, string name, string iconFile)
        {
            ID = id;
            Name = name;
            IconFile = iconFile;
        }
    }
}
