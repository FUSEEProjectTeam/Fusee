using System.Diagnostics;
using System.Reflection;
using System;

namespace Fusee.Xirkit
{
    [DebuggerDisplay("{_propertyInfo.Name}")]
    public class FieldAccesssor<T> : IMemberAccessor<T>
    {
        private readonly FieldInfo _fieldInfo;
        public FieldAccesssor(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public void Set(object o, T val)
        {
            // NOTE: no checking done here.
            _fieldInfo.SetValue(o, val);
        }

        public T Get(object o)
        {
            // NOTE: no checking done here.
            return (T) _fieldInfo.GetValue(o);
        }
    }

    [DebuggerDisplay("{_fieldInfo.Name}")]
    public class ConvertingFieldAccessor<TPin, TObj> : IMemberAccessor<TPin>
    {
        private readonly FieldInfo _fieldInfo;
        Converter<TPin, TObj> _p2o;
        Converter<TObj, TPin> _o2p;
        public ConvertingFieldAccessor(FieldInfo fieldInfo, Converter<TPin, TObj> p2o, Converter<TObj, TPin> o2p)
        {
            _fieldInfo = fieldInfo;
            _p2o = p2o;
            _o2p = o2p;
        }

        public void Set(object o, TPin val)
        {
            _fieldInfo.SetValue(o, (object)_p2o(val));
        }

        public TPin Get(object o)
        {
            // NOTE: no checking done here.
            return _o2p((TObj)_fieldInfo.GetValue(o));
        }
    }


}
