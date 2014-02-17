using System.Net;

namespace Fusee.Engine
{
    /// <summary>
    /// Interface for a network connection object.
    /// </summary>
    /// <remarks>
    /// Implementation tasks: A network connection has to implement functionality to send messages via a network and to disconnect.
    /// </remarks>
    public interface INetworkConnection
    {
        /// <summary>
        /// Gets the remote end point. 
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets the roundtrip time of a packet. This is the time in milliseconds that a packet requires to be send to the remote end point and back.
        /// </summary>
        /// <value>
        /// The roundtrip time.
        /// </value>
        float RoundtripTime { get; }

        /// <summary>
        /// Disconnects this instance from the remote end point.
        /// </summary>
        void Disconnect();
        /// <summary>
        /// Disconnects this instance with specified bye message for the remote end point.
        /// </summary>
        /// <param name="byeMessage">The bye message.</param>
        void Disconnect(string byeMessage);

        /// <summary>
        /// Sends the message as <see cref="MessageType"/>.RealiableOrdered on channel 0.
        /// </summary>
        /// <param name="packet">The packet in byte[].</param>
        /// <returns>True if the message arrived.</returns>
        bool SendMessage(byte[] packet);
        /// <summary>
        /// Sends the message with options.
        /// </summary>
        /// <param name="packet">The packet in byte[].</param>
        /// <param name="msgDelivery">The <see cref="MessageDelivery"/>.</param>
        /// <param name="msgChannel">The message channel.</param>
        /// <returns>True if the message arrived.</returns>
        bool SendMessage(byte[] packet, MessageDelivery msgDelivery, int msgChannel);
    }
}
