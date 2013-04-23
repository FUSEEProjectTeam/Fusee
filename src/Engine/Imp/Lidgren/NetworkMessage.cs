using System.Net;

namespace Fusee.Engine
{
    public class NetworkMessage : INetworkMsg
    {
        public MessageType Type { get; internal set; }
        public ConnectionStatus Status { get; internal set; }
        public IPEndPoint Sender { get; internal set; }

        public string Message { get; internal set; }
    }
}
