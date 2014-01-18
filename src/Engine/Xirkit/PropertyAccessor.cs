using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System;

namespace Fusee.Xirkit
{
    [DebuggerDisplay("{_propertyInfo.Name}")]
    public class PropertyAccessor<T> : IMemberAccessor<T>
    {
        private readonly PropertyInfo _propertyInfo;
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public void Set(object o, T val)
        {
            // NOTE: no checking done here.
            _propertyInfo.SetValue(o, val, null);
        }

        public T Get(object o)
        {
            // NOTE: no checking done here.
            return (T)_propertyInfo.GetValue(o, null);
        }
    }

    [DebuggerDisplay("{_propertyInfo.Name}")]
    public class ConvertingPropertyAccessor<TPin, TObj> : IMemberAccessor<TPin>
    {
        private readonly PropertyInfo _propertyInfo;
        Converter<TPin, TObj> _p2o;
        Converter<TObj, TPin> _o2p;
        public ConvertingPropertyAccessor(PropertyInfo propertyInfo, Converter<TPin, TObj> p2o, Converter<TObj, TPin> o2p)
        {
            _propertyInfo = propertyInfo;
            _p2o = p2o;
            _o2p = o2p;
        }

        public void Set(object o, TPin val)
        {
            _propertyInfo.SetValue(o, (object)_p2o(val), null);
        }

        public TPin Get(object o)
        {
            // NOTE: no checking done here.
            return _o2p((TObj)_propertyInfo.GetValue(o, null));
        }
    }
    /* probably done by the pin */
}
