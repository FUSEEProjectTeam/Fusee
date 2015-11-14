using System;
namespace Fusee.Xirkit
{
    /// <summary>
    /// Interface used to handle incoming pins to nodes. This interface contains all relevant parts of an out-pin
    /// implementation not actually bound to the type of the pin 
    /// </summary>
    /// <remarks>
    /// This interface allows out-pins of different types to  be handled similarly (e.g. stored in an array). 
    /// <see cref="OutPin{T}"/> for all implementation parts bound to a concrete pin type.
    /// </remarks>
    public interface IOutPin
    {
        /// <summary>
        /// Retrieves the type discription of the pin.
        /// </summary>
        /// <returns>The pin's type</returns>
        Type GetPinType();
        /// <summary>
        /// Gets the name of the member or the member chain.
        /// </summary>
        /// <value>
        /// The member' name.
        /// </value>
        string Member { get; }
        /// <summary>
        /// Propagates the value connected to this out-pin to all connected in-pins
        /// </summary>
        void Propagate();
        /// <summary>
        /// Attaches the specified in-pin to this out-pin.
        /// </summary>
        /// <param name="other">The in in-pin to attach to.</param>
        void Attach(IInPin other);
        /// <summary>
        /// Detaches the specified (and previously attached) in-pin from this out-pin
        /// </summary>
        /// <param name="other">The in-pin to detach from.</param>
        void Detach(IInPin other);
    }
}
