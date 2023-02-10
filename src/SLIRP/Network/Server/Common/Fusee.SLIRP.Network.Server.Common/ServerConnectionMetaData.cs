using Fusee.SLIRP.Network.Common;
using System.Net;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Server.Common
{
    public struct ServerConnectionMetaData
    {
        /// <summary>
        /// Which IP addresses will be accepted. 
        /// </summary>
        public IPAddress ValidIPAdress = IPAddress.Any;
        public ConnectionMetaData connData;

        public int Port => connData.Port;
        public AddressFamily AddressFamily => connData.AddressFamily;   
        public ProtocolType ProtocolType => connData.ProtocolType;
        public SocketType SocketType => connData.SocketType;
        public NetworkPackageMeta NetworkPackageMeta=> connData.NetworkPackageMeta; 

        public ServerConnectionMetaData(IPAddress validIPAdress, int port, AddressFamily addressFamily,ProtocolType protocolType, SocketType socketType, NetworkPackageMeta networkPackageMeta)
        {
            ValidIPAdress = validIPAdress;
            connData = new ConnectionMetaData(port, addressFamily, protocolType, socketType, networkPackageMeta);
        }

        public ServerConnectionMetaData(IPAddress validIPAdress, ConnectionMetaData connectionMetaData, NetworkPackageMeta networkPackageMeta)
        {
            ValidIPAdress = validIPAdress;
            connData = new ConnectionMetaData(connectionMetaData);
        }
    }
}
