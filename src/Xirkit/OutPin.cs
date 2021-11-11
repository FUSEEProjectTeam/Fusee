using System;
using System.Collections.Generic;

namespace Fusee.Xirkit
{
    /// <summary>
    /// Class representing outgoing pins to nodes. 
    /// </summary>
    /// <typeparam name="T">The type of the values transmitted over this pin.</typeparam>
    /// <seealso cref="IOutPin"/>
    /// <seealso cref="Pin"/>
    public class OutPin<T> : Pin, IOutPin
    {
        private readonly List<InPin<T>> _links;
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
        /// Initializes a new instance of the <see cref="OutPin{T}"/> class.
        /// </summary>
        /// <param name="n">The node this out-pin should be connected to.</param>
        /// <param name="member">The member (property or field) exposed by this out-pin.</param>
        /// <param name="memberAccessor">The member accessor used to write data to the member.</param>
        public OutPin(Node n, string member, IMemberAccessor<T> memberAccessor)
            : base(n, member)
        {
            _links = new List<InPin<T>>();
            _memberAccessor = memberAccessor;
        }

        /// <summary>
        /// Attaches the specified in-pin to this out-pin.
        /// </summary>
        /// <param name="other">The in in-pin to attach to.</param>
        public void Attach(IInPin other)
        {
            _links.Add((InPin<T>)other);
        }

        /// <summary>
        /// Detaches the specified (and previously attached) in-pin from this out-pin
        /// </summary>
        /// <param name="other">The in-pin to detach from.</param>
        public void Detach(IInPin other)
        {
            _links.Remove((InPin<T>)other);
        }

        /// <summary>
        /// Retrieves the value currently stored in the member (field or property) connected to this out-pin.
        /// </summary>
        /// <returns></returns>
        public T GetValue()
        {
            return _memberAccessor.Get(N.O);
        }

        /// <summary>
        /// Propagates the value connected to this out-pin to all connected in-pins
        /// </summary>
        public void Propagate()
        {
            foreach (InPin<T> inPin in _links)
            {
                inPin.SetValue(GetValue());
            }
        }
        /// <summary>
        /// Gets the in-pins connected to this out-pin.
        /// </summary>
        /// <value>
        /// The in-pins.
        /// </value>
        public IEnumerable<IInPin> InPins
        {
            get { return _links; }
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
    }
}