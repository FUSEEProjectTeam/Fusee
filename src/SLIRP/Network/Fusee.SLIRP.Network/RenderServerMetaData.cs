using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network
{
    public struct RenderServerMetaData
    {
        public SLIRPRenderServer RenderServer;
        public Socket RenderSocket;
        public int MaxConnections;
        public ServerConnectionMetaData ConnectionData;

        public RenderServerMetaData(SLIRPRenderServer server, Socket socket, int maxConnections, ServerConnectionMetaData connectionData)
        {
            RenderServer = server;
            RenderSocket = socket;
            MaxConnections = maxConnections;
            ConnectionData = connectionData;
        }
    }
}
