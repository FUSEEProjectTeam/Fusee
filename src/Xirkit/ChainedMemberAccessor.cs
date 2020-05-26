using System.Diagnostics;
using System.Reflection;
using Fusee.Math.Core;

namespace Fusee.Xirkit
{
    /// <summary>
    /// Class used inside Xirkit to access (read and write) nested members that are referenced
    /// by a chain of properties/fields separated by the object-access operator '.' (dot).
    /// </summary>
    /// <typeparam name="TPin">The type of the pin (the type to the outside world).</typeparam>
    /// <typeparam name="TObj">The type of the object referenced by the innermost member of the access chain.</typeparam>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainedMemberAccessor{TPin, TObj}"/> class.
        /// </summary>
        /// <param name="miList">A list of member info objects (sorted from outermost to innermost member along the access chain).</param>
        /// <param name="p2o">A converter converting from the pin type to and from the original type accessed by the pin.</param>
        /// <param name="o2p">A converter converting from the innermost type referenced by access chain to the outside pin type.</param>
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
                    _pre[i] = delegate (int ii) { _oList[ii + 1] = ((FieldInfo)_miList[ii]).GetValue(_oList[ii]); };
                    _post[i] = delegate (int ii) { ((FieldInfo)_miList[ii]).SetValue(_oList[ii], _oList[ii + 1]); };
                }
                else
                {
                    _pre[i] = delegate (int ii) { _oList[ii + 1] = ((PropertyInfo)_miList[ii]).GetValue(_oList[ii], null); };
                    _post[i] = delegate (int ii) { ((PropertyInfo)_miList[ii]).SetValue(_oList[ii], _oList[ii + 1], null); };
                }
            }

        }

        /// <summary>
        /// Assigns the value given in val to the object accessed by this member accessor.
        /// </summary>
        /// <param name="o">The object to which the access chain belongs to.</param>
        /// <param name="val">The value to be written to the end of the access chain starting in o.</param>
        public void Set(object o, TPin val)
        {
            _oList[0] = o;
            for (int i = 0; i < _miList.Length; i++)
                _pre[i](i);

            _oList[_miList.Length] = _p2o(val);
            for (int i = _miList.Length - 1; i >= 0; i--)
                _post[i](i);
        }

        /// <summary>
        /// Retrieves the value of the innermost member along the access chain from the specified object.
        /// </summary>
        /// <param name="o">The object to retrieve a value from.</param>
        /// <returns>The value contained in the given object at the end of the access chain starting in o.</returns>
        public TPin Get(object o)
        {
            _oList[0] = o;
            for (int i = 0; i < _miList.Length; i++)
                _pre[i](i);
            return _o2p((TObj)_oList[_miList.Length]);
        }
    }
}