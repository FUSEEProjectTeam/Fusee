using System;
using System.Net;
using Fusee.Engine.Common;
using Lidgren.Network;

namespace Fusee.Engine.Imp.Network.Desktop
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
                        _remoteEndPoint = value.RemoteEndPoint;
            }
            get { return _connection; }
        }

        /// <summary>
        /// Gets the remote end point aka the connection partner.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        IPEndpointData INetworkConnection.RemoteEndPoint
        {
            get
            {
                return new IPEndpointData { Address = IPToLong(_remoteEndPoint.Address.ToString()), Port = _remoteEndPoint.Port};
            }
        }

        internal IPEndPoint _remoteEndPoint; /// { internal set; get; }

        private static long IPToLong(string ipAddress)
        {
            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(ipAddress, out ip))
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                return (((long)ip.GetAddressBytes()[0] << 24) | ((int)ip.GetAddressBytes()[1] << 16) | ((int)ip.GetAddressBytes()[2] << 8) | ip.GetAddressBytes()[3]);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
            else return 0;
        }

        private static string LongToIP(long ipAddress)
        {
            System.Net.IPAddress tmpIp;
            if (System.Net.IPAddress.TryParse(ipAddress.ToString(), out tmpIp))
            {
                try
                {
                    Byte[] bytes = tmpIp.GetAddressBytes();
                    long addr = (long)BitConverter.ToInt32(bytes, 0);
                    return new System.Net.IPAddress(addr).ToString();
                }
                catch (Exception e) { return e.Message; }
            }
            else return String.Empty;
        }




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
