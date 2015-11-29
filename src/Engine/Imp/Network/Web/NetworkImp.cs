// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System.Collections.Generic;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Network.Web
{
    public class NetworkImp : INetworkImp
    {
        [JSExternal]
        public NetConfigValues Config { get; set; }
        [JSExternal]
        public NetStatusValues Status { get; set; }
        [JSExternal]
        public List<INetworkConnection> Connections { get; }
        [JSExternal]
        public string GetLocalIp()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public List<INetworkMsg> IncomingMsg { get; }
        [JSExternal]
        public void StartPeer(int port)
        {
            throw new System.NotImplementedException();
        }

#pragma warning disable 0067 // events are used in external javascript
        public event ConnectionUpdateEvent ConnectionUpdate;
#pragma warning restore 0067


        [JSExternal]
        public bool OpenConnection(SysType type, string host, int port)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void CloseConnection()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool SendMessage(byte[] msg, MessageDelivery msgDelivery, int msgChannel)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SendDiscoveryMessage(int port)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void OnUpdateFrame()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void CloseDevices()
        {
            throw new System.NotImplementedException();
        }
    }
}
