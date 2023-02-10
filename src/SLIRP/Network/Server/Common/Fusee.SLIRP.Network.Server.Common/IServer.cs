using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Server.Common
{
    public interface IServer
    {
        public void Init(in ServerConnectionMetaData? metaData = null, int maxConnections = 6);

        public void Shutdown();

        public void StartServer(in ServerConnectionMetaData? metaData = null);

        public void StopServer();
    }
}
