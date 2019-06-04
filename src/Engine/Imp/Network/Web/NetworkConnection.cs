// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Network.Web
{
    /// <summary>
    /// Implements the networking functions.
    /// </summary>
    public class NetworkConnection : INetworkConnection
    {
        [JSExternal]
        public IPEndpointData RemoteEndPoint { get; }
        /// <summary>
        /// Returns the ping value.
        /// </summary>
        [JSExternal]
        public float RoundtripTime { get; }
        /// <summary>
        /// Disconnects the client from the server.
        /// </summary>
        [JSExternal]
        public void Disconnect()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Disconnects the client from the server while sending a bye message.
        /// </summary>
        /// <param name="byeMessage">The message to be delivered.</param>
        [JSExternal]
        public void Disconnect(string byeMessage)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sends a message to a client or server.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        [JSExternal]
        public bool SendMessage(byte[] packet)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sends a message to a specific channel while inquiring if the message has been delivered.
        /// </summary>
        /// <param name="packet"></param>
        ///// <param name="msgDelivery">Returns if the message has been delivered.?</param>
        /// <param name="msgChannel">The channel in which the message will be send.</param>
        /// <returns></returns>
        [JSExternal]
        public bool SendMessage(byte[] packet, MessageDelivery msgDelivery, int msgChannel)
        {
            throw new System.NotImplementedException();
        }
    }
}
