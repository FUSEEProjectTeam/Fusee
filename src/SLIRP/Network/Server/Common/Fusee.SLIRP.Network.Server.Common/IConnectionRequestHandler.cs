using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Server.Common
{
    public interface IConnectionRequestHandler
    {
        public void Init(ServerMetaData serverMetaData, IConnectionHandler connectHandler);

        public void Shutdown();

        public void Run();
    }
}
