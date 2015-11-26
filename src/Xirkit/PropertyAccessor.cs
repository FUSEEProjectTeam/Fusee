using System.Diagnostics;
using System.Reflection;
using Fusee.Math.Core;

namespace Fusee.Xirkit
{
    /// <summary>
    /// Member Accessor implementation specialized on accessing properties (using set and get).
    /// </summary>
    /// <typeparam name="T">The type of the pin.</typeparam>
    /// <seealso cref="IMemberAccessor{T}"/>    
    [DebuggerDisplay("{_propertyInfo.Name}")]
    public class PropertyAccessor<T> : IMemberAccessor<T>
    {
        private readonly PropertyInfo _propertyInfo;
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessor{T}"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        /// <summary>
        /// Sets the specified value to the given object. This instance holds information about which
        /// property is to be filled with the value and how to perform this assignment.
        /// </summary>
        /// <param name="o">The object on which the property identified with this instance's property accessor the given value is to be assigned to.</param>
        /// <param name="val">The value that should be assigned.</param>
        public void Set(object o, T val)
        {
            // NOTE: no checking done here.
            _propertyInfo.SetValue(o, val, null);
        }

        /// <summary>
        /// Retrieves the value currently stored in the object o. his instance holds information about which
        /// property of o the value should be retrieved from and how to perform this retrieval.
        /// </summary>
        /// <param name="o">The object from which the property identified with this instance's property accessor the value is to be retrieved from.</param>
        /// <returns>
        /// The value currently stored in o's property.
        /// </returns>
        public T Get(object o)
        {
            // NOTE: no checking done here.
            return (T)_propertyInfo.GetValue(o, null);
        }
    }

    /// <summary>
    /// Member Accessor implementation specialized on accessing properties (using set and get) where the type of the pin and the actual type of the field are different.
    /// The accsessor performs the conversion operation specefied when performing its set or get operations.
    /// </summary>
    /// <typeparam name="TPin">The type of the pin.</typeparam>
    /// <typeparam name="TObj">The type of the field.</typeparam>
    [DebuggerDisplay("{_propertyInfo.Name}")]
    public class ConvertingPropertyAccessor<TPin, TObj> : IMemberAccessor<TPin>
    {
        private readonly PropertyInfo _propertyInfo;
        Converter<TPin, TObj> _p2o;
        Converter<TObj, TPin> _o2p;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertingPropertyAccessor{TPin, TObj}"/> class.
        /// </summary>
        /// <param name="propertyInfo">The field information.</param>
        /// <param name="p2o">The conversion operation to convert values from the pin type to the field type.</param>
        /// <param name="o2p">The inverse conversion operation (from field to pin).</param>
        public ConvertingPropertyAccessor(PropertyInfo propertyInfo, Converter<TPin, TObj> p2o, Converter<TObj, TPin> o2p)
        {
            _propertyInfo = propertyInfo;
            _p2o = p2o;
            _o2p = o2p;
        }

        /// <summary>
        /// Sets the specified value to the given object. This instance holds information about which
        /// property is to be filled with the value and how to perform this assignment.
        /// </summary>
        /// <param name="o">The object on which the property identified with this instance's property accessor the given value is to be assigned to.</param>
        /// <param name="val">The value that should be assigned.</param>
        public void Set(object o, TPin val)
        {
            _propertyInfo.SetValue(o, (object)_p2o(val), null);
        }

        /// <summary>
        /// Retrieves the value currently stored in the object o. his instance holds information about which
        /// property of o the value should be retrieved from and how to perform this retrieval.
        /// </summary>
        /// <param name="o">The object from which the property identified with this instance's property accessor the value is to be retrieved from.</param>
        /// <returns>
        /// The value currently stored in o's property.
        /// </returns>
        public TPin Get(object o)
        {
            // NOTE: no checking done here.
            return _o2p((TObj)_propertyInfo.GetValue(o, null));
        }
    }
    
}
