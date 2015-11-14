namespace Fusee.Xirkit
{
    /// <summary>
    /// Base interface for all accessors. An accessor implements the connections between the Pin 
    /// (<see cref="IInPin"/> or <see cref="IOutPin"/>) of a <see cref="Node"/> and a member
    /// (such as a Field or a Property) of the object "hosted" by the Node.
    /// </summary>
    /// <typeparam name="T">The type of the pin. This is not necessarily the exact type
    /// of the member exposed by the object due to Xirkits ability to perform data conversions.</typeparam>
    public interface IMemberAccessor<T>
    {
        /// <summary>
        /// Sets the specified value to the given object. This instance holds information about which 
        /// member is to be filled with the value and how to perform this assignment.
        /// </summary>
        /// <param name="o">The object on which the member identified with this instance's member accessor the given value is to be assigned to.</param>
        /// <param name="val">The value that should be assigned.</param>
        void Set(object o, T val);
        /// <summary>
        /// Retrieves the value currently stored in the object o. his instance holds information about which 
        /// member of o the value should be retrieved from and how to perform this retrieval.
        /// </summary>
        /// <param name="o">The object from which the member identified with this instance's member accessor the value is to be retrieved from.</param>
        /// <returns>The value currently stored in o's member.</returns>
        T Get(object o);
    } 
}
