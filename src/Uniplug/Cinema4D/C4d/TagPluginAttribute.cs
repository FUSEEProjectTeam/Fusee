using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4d
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class TagPluginAttribute : PluginBaseAttribute
    {
        public bool Visible;            // The tag can be seen in the Object Manager.
        public bool Multiple;           // Multiple copies of the tag allowed on a single object.
        public bool Hierarchical;       // The tag works hierarchical, so that sub-objects inherit its properties (e.g. the material tag).
        public bool Expression;         // The tag is an expression.
        // public bool Temporary;          //< @markPrivate
        // public bool Modifyobject;       //< @markPrivate


        public int Info
        {
            get
            {
                return
                    ((Visible) ? C4dApi.TAG_VISIBLE : 0) |
                    ((Multiple) ? C4dApi.TAG_MULTIPLE : 0) |
                    ((Hierarchical) ? C4dApi.TAG_HIERARCHICAL : 0) |
                    ((Expression) ? C4dApi.TAG_EXPRESSION : 0);
                    // ((Temporary) ? C4dApi.TAG_TEMPORARY : 0) |
                    // ((Modifyobject) ? C4dApi.TAG_MODIFYOBJECT : 0);
            }
        }

        public string Description;
        public TagPluginAttribute(int id) : base(id)
        {
        }
        public TagPluginAttribute(int id, string name) : base(id, name)
        {
        }
        public TagPluginAttribute(int id, string name, string iconFile)
            : base(id, name, iconFile)
        {
        }
        public TagPluginAttribute(int id, string name, string iconFile, string description)
            : base(id, name, iconFile)
        {
            Description = description;
        }
    }


}
