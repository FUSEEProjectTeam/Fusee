// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System.Collections.Generic;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Network.Web
{
    /// <summary>
    /// Implements the network functions
    /// </summary>
    public class NetworkImp : INetworkImp
    {
        /// <summary>
        /// Gets and sets the network config.
        /// </summary>
        [JSExternal]
        public NetConfigValues Config { get; set; }
        /// <summary>
        /// Gets and sets the network status.
        /// </summary>
        [JSExternal]
        public NetStatusValues Status { get; set; }
        /// <summary>
        /// Returns the network connections.
        /// </summary>
        [JSExternal]
        public List<INetworkConnection> Connections { get; }
        /// <summary>
        /// Returns the local ip(not implemented!).
        /// </summary>
        /// <returns></returns>
        [JSExternal]
        public string GetLocalIp()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns an incoming message.
        /// </summary>
        [JSExternal]
        public List<INetworkMsg> IncomingMsg { get; }
        /// <summary>
        /// Starts a peer connection.
        /// </summary>
        /// <param name="port"></param>
        [JSExternal]
        public void StartPeer(int port)
        {
            throw new System.NotImplementedException();
        }

#pragma warning disable 0067 // events are used in external javascript
        /// <summary>
        /// Triggers when the connection is updated.
        /// </summary>
        public event ConnectionUpdateEvent ConnectionUpdate;
#pragma warning restore 0067

        /// <summary>
        /// Opens the connection(not implemented!).
        /// </summary>
        /// <param name="type"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [JSExternal]
        public bool OpenConnection(SysType type, string host, int port)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Terminates the connection(not implemented!).
        /// </summary>
        [JSExternal]
        public void CloseConnection()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sends a message(not implemented!).
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgDelivery"></param>
        /// <param name="msgChannel"></param>
        /// <returns></returns>
        [JSExternal]
        public bool SendMessage(byte[] msg, MessageDelivery msgDelivery, int msgChannel)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sends a discovery message(not implemented!).
        /// </summary>
        /// <param name="port"></param>
        [JSExternal]
        public void SendDiscoveryMessage(int port)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        [JSExternal]
        public void OnUpdateFrame()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Closes all devices(not implemented!).
        /// </summary>
        [JSExternal]
        public void CloseDevices()
        {
            throw new System.NotImplementedException();
        }
    }
}
