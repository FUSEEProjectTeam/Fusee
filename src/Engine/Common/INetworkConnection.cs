using System.Net;

namespace Fusee.Engine
{
    public interface INetworkConnection
    {
        IPEndPoint RemoteEndPoint { get; }
        
        float RoundtripTime { get; }

        void Disconnect();
        void Disconnect(string byeMessage);
        
        bool SendMessage(byte[] packet);
        bool SendMessage(byte[] packet, MessageDelivery msgDelivery, int msgChannel);
    }
}
