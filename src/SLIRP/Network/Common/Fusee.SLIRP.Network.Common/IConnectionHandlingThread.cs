using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Common
{
    public abstract class IConnectionHandlingThread
    {
        private Socket _clientSocket;
        private IDisconnectHandler _connectionHandler;
        private NetworkPackageMeta _packageMeta;

        private bool _isInitialized;

        //workaround to be able to assign a socket to the thread. See instantiation at ClientHandlers method "HandleClient".
        protected Socket ClientSocket
        {
            get => _clientSocket;
        }

        protected IDisconnectHandler ConnectionHandler
        {
            get => _connectionHandler;
        }

        protected NetworkPackageMeta NetworkPackageMeta => _packageMeta;

        public event Action<Socket> OnClientClosed;

        public IConnectionHandlingThread()
        { }

        public void Init(IDisconnectHandler connectionHandler, Socket clientSocket,  NetworkPackageMeta packageMeta)
        {
            _clientSocket = clientSocket;
            _connectionHandler = connectionHandler;
            _packageMeta = packageMeta;
            _isInitialized = true;
        }

        public abstract void RunHandleConnection();

        public abstract void OnClientDisconnected(IConnectionHandlingThread handler, Socket disconnectedClient);
    }
}
