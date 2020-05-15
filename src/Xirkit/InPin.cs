using System;
namespace Fusee.Xirkit
{
    /// <summary>
    /// Class representing incoming pins to nodes. 
    /// </summary>
    /// <typeparam name="T">The type of the values transmitted over this pin.</typeparam>
    /// <seealso cref="IInPin"/>
    /// <seealso cref="Pin"/>
    public class InPin<T> : Pin, IInPin
    {
        private IMemberAccessor<T> _memberAccessor;
        /// <summary>
        /// Gets and sets the member accessor.
        /// </summary>
        /// <value>
        /// The member accessor.
        /// </value>
        public IMemberAccessor<T> MemberAccessor
        {
            get { return _memberAccessor; }
            set { _memberAccessor = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InPin{T}"/> class.
        /// </summary>
        /// <param name="n">The node this pin should belong to.</param>
        /// <param name="member">The name of the member or the member chain within the object hosted by the node n.</param>
        /// <param name="memberAccessor">The member accessor used to deliver incoming values from the in-pin to the specified member of the node.</param>
        public InPin(Node n, string member, IMemberAccessor<T> memberAccessor)
            : base(n, member)
        {
            _memberAccessor = memberAccessor;
            // TODO: build a set accessor for the property.
        }

        /// <summary>
        /// Sets the value. Triggers the <see cref="ReceivedValue"/> event.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(T value)
        {
            _memberAccessor.Set(N.O, value);
            if (ReceivedValue != null)
                ReceivedValue(this, null);
        }

        /// <summary>
        /// Retrieves the type description of the pin.
        /// </summary>
        /// <returns>
        /// The pin's type
        /// </returns>
        public Type GetPinType()
        {
            return typeof(T);
        }

        /// <summary>
        /// Occurs when this pin received a value.
        /// </summary>
        public event ReceivedValueHandler ReceivedValue;
    }
}
