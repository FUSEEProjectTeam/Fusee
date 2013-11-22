using System.Net;
using Lidgren.Network;

namespace Fusee.Engine
{
    /// <summary>
    /// Lidgren implementation of <see cref="INetworkConnection" /> interface.
    /// </summary>
    public class NetworkConnection : INetworkConnection
    {
        #region Fields

        internal NetworkImp NetworkImp;

        private NetConnection _connection;

        internal NetConnection Connection
        {
            set
            {
                _connection = value;

                if (value != null)
                    if (value.RemoteEndPoint != null)
                        RemoteEndPoint = value.RemoteEndPoint;
            }
            get { return _connection; }
        }

        /// <summary>
        /// Gets the remote end point aka the connection partner.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public IPEndPoint RemoteEndPoint { internal set; get; }

        /// <summary>
        /// Gets the roundtrip time of a packet. This is the time in milliseconds that a packet requires to be send to the remote end point and back.
        /// </summary>
        /// <value>
        /// The roundtrip time.
        /// </value>
        public float RoundtripTime
        {
            get { return Connection.AverageRoundtripTime;}
        }

        #endregion

        #region Members

        /// <summary>
        /// Disconnects this instance from the remote end point.
        /// </summary>
        public void Disconnect()
        {
            Disconnect("Disconnecting");
        }

        /// <summary>
        /// Disconnects this instance with specified bye message for the remote end point.
        /// </summary>
        /// <param name="byeMessage">The bye message.</param>
        public void Disconnect(string byeMessage)
        {
            Connection.Disconnect(byeMessage);
        }

        /// <summary>
        /// Sends the message as <see cref="MessageType" />.RealiableOrdered on channel 0.
        /// </summary>
        /// <param name="packet">The packet in byte[].</param>
        /// <returns>True if the message was succesfully sent.</returns>
        public bool SendMessage(byte[] packet)
        {
            return SendMessage(packet, MessageDelivery.ReliableOrdered, 0);
        }

        /// <summary>
        /// Sends the message with options.
        /// </summary>
        /// <param name="packet">The packet in byte[].</param>
        /// <param name="msgDelivery">The <see cref="MessageDelivery" />.</param>
        /// <param name="msgChannel">The message channel.</param>
        /// <returns>True if the message was succesfully sent.</returns>
        public bool SendMessage(byte[] packet, MessageDelivery msgDelivery, int msgChannel)
        {
            return NetworkImp.SendMessage(packet, Connection, msgDelivery, msgChannel);
        }

        #endregion
    }
}
