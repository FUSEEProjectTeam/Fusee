// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Network.Web
{
    public class NetworkMessage : INetworkMsg
    {
        [JSExternal]
        public MessageType Type { get; }
        [JSExternal]
        public ConnectionStatus Status { get; }
        [JSExternal]
        public INetworkConnection Sender { get; }
        [JSExternal]
        public NetworkMsgType Message { get; }
    }
}
