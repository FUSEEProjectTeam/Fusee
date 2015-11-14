using System.Diagnostics;
using System.Reflection;
using System;
using Fusee.Math;

namespace Fusee.Xirkit
{
    /// <summary>
    /// Member Accessor implementation specialized on accessing fields.
    /// </summary>
    /// <typeparam name="T">The type of the pin.</typeparam>
    /// <seealso cref="IMemberAccessor{T}"/>
    [DebuggerDisplay("{_fieldInfo.Name}")]
    public class FieldAccesssor<T> : IMemberAccessor<T>
    {
        private readonly FieldInfo _fieldInfo;
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAccesssor{T}"/> class.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        public FieldAccesssor(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        /// <summary>
        /// Sets the specified value to the given object. This instance holds information about which
        /// field is to be filled with the value and how to perform this assignment.
        /// </summary>
        /// <param name="o">The object on which the field identified with this instance's field accessor the given value is to be assigned to.</param>
        /// <param name="val">The value that should be assigned.</param>
        public void Set(object o, T val)
        {
            // NOTE: no checking done here.
            _fieldInfo.SetValue(o, val);
        }

        /// <summary>
        /// Retrieves the value currently stored in the object o. his instance holds information about which
        /// field of o the value should be retrieved from and how to perform this retrieval.
        /// </summary>
        /// <param name="o">The object from which the field identified with this instance's field accessor the value is to be retrieved from.</param>
        /// <returns>
        /// The value currently stored in o's field.
        /// </returns>
        public T Get(object o)
        {
            // NOTE: no checking done here.
            return (T) _fieldInfo.GetValue(o);
        }
    }

    /// <summary>
    /// Member Accessor implementation specialized on accessing fields where the type of the pin and the actual type of the field are different.
    /// The accsessor performs the conversion operation specefied when performing its set or get operations.
    /// </summary>
    /// <typeparam name="TPin">The type of the pin.</typeparam>
    /// <typeparam name="TObj">The type of the field.</typeparam>
    [DebuggerDisplay("{_fieldInfo.Name}")]
    public class ConvertingFieldAccessor<TPin, TObj> : IMemberAccessor<TPin>
    {
        private readonly FieldInfo _fieldInfo;
        Converter<TPin, TObj> _p2o;
        Converter<TObj, TPin> _o2p;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertingFieldAccessor{TPin, TObj}"/> class.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="p2o">The conversion operation to convert values from the pin type to the field type.</param>
        /// <param name="o2p">The inverse conversion operation (from field to pin).</param>
        public ConvertingFieldAccessor(FieldInfo fieldInfo, Converter<TPin, TObj> p2o, Converter<TObj, TPin> o2p)
        {
            _fieldInfo = fieldInfo;
            _p2o = p2o;
            _o2p = o2p;
        }

        /// <summary>
        /// Sets the specified value to the given object. This instance holds information about which
        /// field is to be filled with the value and how to perform this assignment.
        /// </summary>
        /// <param name="o">The object on which the field identified with this instance's field accessor the given value is to be assigned to.</param>
        /// <param name="val">The value that should be assigned.</param>
        public void Set(object o, TPin val)
        {
            _fieldInfo.SetValue(o, (object)_p2o(val));
        }

        /// <summary>
        /// Retrieves the value currently stored in the object o. his instance holds information about which
        /// field of o the value should be retrieved from and how to perform this retrieval.
        /// </summary>
        /// <param name="o">The object from which the field identified with this instance's field accessor the value is to be retrieved from.</param>
        /// <returns>
        /// The value currently stored in o's field.
        /// </returns>
        public TPin Get(object o)
        {
            // NOTE: no checking done here.
            return _o2p((TObj)_fieldInfo.GetValue(o));
        }
    }


}
