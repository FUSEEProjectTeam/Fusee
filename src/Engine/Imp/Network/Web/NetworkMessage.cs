// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Network.Web
{
    /// <summary>
    /// Implements a network message.
    /// </summary>
    public class NetworkMessage : INetworkMsg
    {
        /// <summary>
        /// Returns the message type.
        /// </summary>
        [JSExternal]
        public MessageType Type { get; }
        /// <summary>
        /// Returns the connection status.
        /// </summary>
        [JSExternal]
        public ConnectionStatus Status { get; }
        /// <summary>
        /// Returns the sender.
        /// </summary>
        [JSExternal]
        public INetworkConnection Sender { get; }
        /// <summary>
        /// Returns the message.
        /// </summary>
        [JSExternal]
        public NetworkMsgType Message { get; }
    }
}
