using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Common
{
    public interface IConnectionRequestHandler
    {
        public event Action<IConnectionRequestHandler, Socket> OnClientConnect;

        public void Shutdown();

        public void Run();
    }
}
