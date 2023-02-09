using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network
{
    public struct ServerConnectionMetaData
    {
        public IPAddress ValidIPAdress = IPAddress.Any;
        public Int32 Port = 1300;
        public AddressFamily AddressFamily = AddressFamily.InterNetwork;
        public ProtocolType ProtocolType = ProtocolType.Udp;
       
        public SocketType SocketType = SocketType.Stream;

        public ServerConnectionMetaData(IPAddress validIPAdress, int port, AddressFamily addressFamily,ProtocolType protocolType)
        {
            ValidIPAdress = validIPAdress;
            Port = port;
            AddressFamily = addressFamily;
            ProtocolType = protocolType;
           
        }
    }
}
