using System.Net;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Common
{
    public struct ServerConnectionMetaData
    {
        public IPAddress ValidIPAdress = IPAddress.Any;
        public Int32 Port = 1300;
        public AddressFamily AddressFamily = AddressFamily.InterNetwork;
        public ProtocolType ProtocolType = ProtocolType.Udp;
       
        public SocketType SocketType = SocketType.Stream;

        public NetworkPackageMeta NetworkPackageMeta; 

        public ServerConnectionMetaData(IPAddress validIPAdress, int port, AddressFamily addressFamily,ProtocolType protocolType, NetworkPackageMeta networkPackageMeta)
        {
            ValidIPAdress = validIPAdress;
            Port = port;
            AddressFamily = addressFamily;
            ProtocolType = protocolType;
            NetworkPackageMeta = networkPackageMeta;
        }
    }
}
