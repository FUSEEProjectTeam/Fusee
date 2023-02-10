using Fusee.SLIRP.Network.Common;
using Fusee.SLIRP.Network.Server.Common;
using System.Net.Sockets;
using System.Text;

namespace Fusee.SLIRP.Network.Server.Examples
{
    /// <summary>
    /// Controls what happens while a client is connected.
    /// Calls <see cref="IConnectionHandlingThread.OnClientDisconnected(IConnectionHandlingThread, Socket)"/> when disposed.
    /// </summary>
    public abstract class IDisposableConnectionHandling : IConnectionHandlingThread, IDisposable
    {
        public IDisposableConnectionHandling():base() { }

        /// <summary>
        /// Called when disposed. 
        /// Calls <see cref="OnDispose"/> first then <see cref="IConnectionHandlingThread.OnClientDisconnected(IConnectionHandlingThread, Socket)"/> 
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            OnClientDisconnected(this, ClientSocket);
        }


        protected virtual void OnDispose() { }
    }
}
