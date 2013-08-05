using System.Net;
using Lidgren.Network;

namespace Fusee.Engine
{
    public class NetworkConnection : INetworkConnection
    {
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

        public IPEndPoint RemoteEndPoint { internal set; get; }

        public float RoundtripTime
        {
            get { return Connection.AverageRoundtripTime;}
        }

        public void Disconnect()
        {
            Disconnect("Disconnecting");
        }

        public void Disconnect(string byeMessage)
        {
            Connection.Disconnect(byeMessage);
        }

        public bool SendMessage(byte[] packet)
        {
            return SendMessage(packet, MessageDelivery.ReliableOrdered, 0);
        }

        public bool SendMessage(byte[] packet, MessageDelivery msgDelivery, int msgChannel)
        {
            return NetworkImp.SendMessage(packet, Connection, msgDelivery, msgChannel);
        }
    }
}
