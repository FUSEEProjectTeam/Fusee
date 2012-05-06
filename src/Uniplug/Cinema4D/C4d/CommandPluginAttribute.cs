using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4d
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class CommandPluginAttribute : PluginBaseAttribute
    {
        public string HelpText;
        public CommandPluginAttribute(int id) : base(id)
        {
        }
        public CommandPluginAttribute(int id, string name) : base(id, name)
        {
        }
        public CommandPluginAttribute(int id, string name, string iconFile)
            :base(id, name, iconFile)
        {
        }
        public CommandPluginAttribute(int id, string name, string iconFile, string helpText)
            : base(id, name, iconFile)
        {
             HelpText = helpText;
        }
    }
}
