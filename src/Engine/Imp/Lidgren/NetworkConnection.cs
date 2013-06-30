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

        public void SendMessage(byte[] packet)
        {
            NetworkImp.SendMessage(packet, Connection);
        }
    }
}
