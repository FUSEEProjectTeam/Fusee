using System;

namespace Fusee.Xirkit
{
    /// <summary>
    /// Event handler type. Methods with this signature can be registered on the <see cref="IInPin.ReceivedValue"/> event.
    /// </summary>
    /// <param name="inPin">The in-pin receiving a value.</param>
    /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
    public delegate void ReceivedValueHandler(IInPin inPin, EventArgs args);

    /// <summary>
    /// Interface used to handle incoming pins to nodes. This interface contains all relevant parts of an in-pin
    /// implementation not actually bound to the type of the pin.
    /// </summary>
    /// <remarks>
    /// This interface allows in-pins of different types to  be handled similarly (e.g. stored in an array). 
    /// <see cref="InPin{T}"/> for all implementation parts
    /// bound to a concrete pin type.
    /// </remarks> 
    public interface IInPin
    {
        /// <summary>
        /// Gets the name of the member or the member chain.
        /// </summary>
        /// <value>
        /// The member' name.
        /// </value>
        string Member { get; }
        /// <summary>
        /// Occurs when this pin received a value.
        /// </summary>
        event ReceivedValueHandler ReceivedValue;
        /// <summary>
        /// Retrieves the type description of the pin.
        /// </summary>
        /// <returns>The pin's type</returns>
        Type GetPinType();
    }
}