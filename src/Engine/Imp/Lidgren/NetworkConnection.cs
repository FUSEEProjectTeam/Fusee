using System.Net;
using Lidgren.Network;

namespace Fusee.Engine
{
    public class NetworkConnection : INetworkConnection
    {
        internal NetworkImp NetworkImp;
        internal NetConnection Connection;

        public IPEndPoint RemoteEndPoint
        {
            get { return Connection.RemoteEndPoint; }
        }

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
