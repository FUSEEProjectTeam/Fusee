using System.Net;

namespace Fusee.Engine
{
    public interface INetworkConnection
    {
        IPEndPoint RemoteEndPoint { get; }
        
        float RoundtripTime { get; }

        void Disconnect();
        void Disconnect(string byeMessage);
        
        void SendMessage(byte[] packet);
    }
}
