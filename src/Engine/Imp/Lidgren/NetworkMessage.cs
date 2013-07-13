using System.Net;

namespace Fusee.Engine
{
    public class NetworkMessage : INetworkMsg
    {
        public MessageType Type { get; internal set; }
        public ConnectionStatus Status { get; internal set; }
        public INetworkConnection Sender { get; internal set; }

        public NetworkMsgType Message { get; internal set; }
    }
}
