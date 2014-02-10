using System;
using System.Diagnostics;
using System.Reflection;

namespace Fusee.Xirkit
{
    [DebuggerDisplay("{_miList[0].Name}...{_miList[_miList.Length-1].Name}")]
    public class ChainedMemberAccessor<TPin, TObj> : IMemberAccessor<TPin>
    {
        //private readonly PropertyInfo _propertyInfo;
        Converter<TPin, TObj> _p2o;
        Converter<TObj, TPin> _o2p;
        private MemberInfo[] _miList;
        private object[] _oList;

        private delegate void PrePost(int i);

        private PrePost[] _pre;
        private PrePost[] _post;

        public ChainedMemberAccessor(MemberInfo[] miList, Converter<TPin, TObj> p2o, Converter<TObj, TPin> o2p)
        {
            _miList = miList;
            _oList = new object[miList.Length + 1];
            _pre = new PrePost[miList.Length];
            _post = new PrePost[miList.Length];

            _p2o = p2o;
            _o2p = o2p;

            for (int i = 0; i < _miList.Length; i++)
            {
                MemberInfo m = _miList[i];
                if (m is FieldInfo)
                {
                    _pre[i] = delegate(int ii) { _oList[ii + 1] = ((FieldInfo)_miList[ii]).GetValue(_oList[ii]); };
                    _post[i] = delegate(int ii) { ((FieldInfo)_miList[ii]).SetValue(_oList[ii], _oList[ii + 1]); };
                }
                else
                {
                    _pre[i] = delegate(int ii) { _oList[ii + 1] = ((PropertyInfo)_miList[ii]).GetValue(_oList[ii], null); };
                    _post[i] = delegate(int ii) { ((PropertyInfo)_miList[ii]).SetValue(_oList[ii], _oList[ii + 1], null); };
                }
            }

        }

        public void Set(object o, TPin val)
        {
            _oList[0] = o;
            for (int i = 0; i < _miList.Length; i++)
                _pre[i](i);

            _oList[_miList.Length] = _p2o(val);
            for (int i = _miList.Length - 1; i >= 0; i--)
                _post[i](i);
        }

        public TPin Get(object o)
        {
            _oList[0] = o;
            for (int i = 0; i < _miList.Length; i++)
                _pre[i](i);
            return _o2p((TObj)_oList[_miList.Length]);
        }
    }
}