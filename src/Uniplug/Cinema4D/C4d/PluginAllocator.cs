using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace C4d
{
    public delegate IntPtr NodeDataAllocator();
    
    class PluginAllocator
    {
        private ConstructorInfo _ctor;
        internal static  List<object> _pluginInstanceList;

        public PluginAllocator(ConstructorInfo ctor)
        {
            _ctor = ctor;
        }

        public IntPtr Allocate()
        {
            if (_ctor != null)
            {
                object o = _ctor.Invoke(null);
                _pluginInstanceList.Add(o);
                return NodeData.getCPtr((NodeData)o).Handle;
                // return 1;
            } 
            return IntPtr.Zero;
        }
    }
}
