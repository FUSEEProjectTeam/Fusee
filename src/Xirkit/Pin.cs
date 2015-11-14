using System.Diagnostics;
using System;
using System.Reflection;

namespace Fusee.Xirkit
{
    /// <summary>
    /// A pin is a connection point where <see cref="Node"/> instances can be connected to each others.
    /// Pin connections carry values of certain types. There are in-pins and out-pins. One out-pin
    /// can be connected to one or more in-pins but each in-pin can only be connected to one out-pin.
    /// </summary>
    /// <remarks>
    /// This calss contains base implementations similar on all pin types, no matter which type 
    /// its value is or whether it's an in- or an out-pin. It is a building block to create various 
    /// pin types used within Xirkit.
    /// </remarks>
    /// <seealso cref="InPin{T}"/>
    /// <seealso cref="OutPin{T}"/>
    /// <seealso cref="IInPin"/>
    /// <seealso cref="IOutPin"/>
    [DebuggerDisplay("{N.O}.{Member}")]
    public class Pin
    {
        private Node _n;
        /// <summary>
        /// Gets the Node this pin is accessing.
        /// </summary>
        /// <value>
        /// The Node.
        /// </value>
        public Node N
        {
            get { return _n; }
        }

        private string _member;
        /// <summary>
        /// Gets the name of the member or the member chain.
        /// </summary>
        /// <value>
        /// The member' name.
        /// </value>
        public string Member
        {
            get { return _member; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pin"/> class.
        /// </summary>
        /// <param name="n">The node this pin should belong to.</param>
        /// <param name="member">The name of the member or the member chain within the object hosted by the node n.</param>
        public Pin(Node n, string member)
        {
            _n = n;
            _member = member;
        }
    }
}
