// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Network.Web
{
    public class NetworkConnection : INetworkConnection
    {
        [JSExternal]
        public IPEndpointData RemoteEndPoint { get; }
        [JSExternal]
        public float RoundtripTime { get; }
        [JSExternal]
        public void Disconnect()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Disconnect(string byeMessage)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool SendMessage(byte[] packet)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool SendMessage(byte[] packet, MessageDelivery msgDelivery, int msgChannel)
        {
            throw new System.NotImplementedException();
        }
    }
}
