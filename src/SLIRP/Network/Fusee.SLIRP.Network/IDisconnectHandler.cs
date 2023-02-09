using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network
{
    internal interface IDisconnectHandler
    {

        public void OnClientDisconnected(IConnectionHandlingThread sender, Socket clientSocket);
    }
}
