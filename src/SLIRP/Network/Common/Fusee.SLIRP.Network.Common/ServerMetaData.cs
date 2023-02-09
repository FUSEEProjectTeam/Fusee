using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Common
{
    public struct ServerMetaData
    {
        public IServer Server;
        public Socket ServerSocket;
        public int MaxConnections;
        public ServerConnectionMetaData ConnectionData;

        public ServerMetaData(IServer server, Socket socket, int maxConnections, ServerConnectionMetaData connectionData)
        {
            Server = server;
            ServerSocket = socket;
            MaxConnections = maxConnections;
            ConnectionData = connectionData;
        }
    }
}
