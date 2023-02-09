using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network
{
    /// <summary>
    /// Controls what happens while a client is connected.
    /// </summary>
    internal class ConnectionHandlingThread : IConnectionHandlingThread, IDisposable
    {
        public ConnectionHandlingThread():base() { }

        public void Dispose()
        {
        
        }

        public override void RunHandleConnection()
        {
            throw new NotImplementedException();
        }

        public override void OnClientDisconnected(IConnectionHandlingThread handler, Socket disconnectedClient)
        {

        }
    }
}
