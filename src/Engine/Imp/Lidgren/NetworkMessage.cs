using System.Net;

namespace Fusee.Engine
{
    /// <summary>
    /// Lidgren implementation of <see cref="INetworkMsg"/>.
    /// </summary>
    public class NetworkMessage : INetworkMsg
    {
        #region Fields

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The <see cref="MessageType"/>.
        /// </value>
        public MessageType Type { get; internal set; }
        /// <summary>
        /// Gets the status of the connection.
        /// </summary>
        /// <value>
        /// The <see cref="ConnectionStatus"/>.
        /// </value>
        public ConnectionStatus Status { get; internal set; }
        /// <summary>
        /// Gets the sender of the message.
        /// </summary>
        /// <value>
        /// The <see cref="INetworkConnection"/>.
        /// </value>
        public INetworkConnection Sender { get; internal set; }

        /// <summary>
        /// Gets the message's data dontent.
        /// </summary>
        /// <value>
        /// The <see cref="NetworkMsgType"/>.
        /// </value>
        public NetworkMsgType Message { get; internal set; }

        #endregion
    }
}
