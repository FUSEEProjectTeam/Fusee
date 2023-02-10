using Fusee.SLIRP.Network.Common;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Server.Common
{
    public interface IDisconnectHandler
    {

        public bool IsHandlingDisconnecs { get; }

        public event Action<IConnectionHandler, Socket> EventClientDisconnected;

        public void OnClientDisconnected(IConnectionHandlingThread sender, Socket clientSocket);
    }
}
