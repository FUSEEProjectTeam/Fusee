using Lidgren.Network;

namespace Fusee.Engine
{
    public class NetworkConnection : INetworkConnection
    {
        internal NetConnection Connection;

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
    }
}
