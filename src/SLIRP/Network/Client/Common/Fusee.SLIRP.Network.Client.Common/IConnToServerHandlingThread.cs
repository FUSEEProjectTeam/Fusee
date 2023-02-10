using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Common
{
    public abstract class IConnToServerHandlingThread
    {
        private EstablishedConnectionData _connectionData;
        private IDisconnectFromServerHandler _diconnectHandler;
        private NetworkPackageMeta _packageMeta;
        private Socket socketToServer;

        private bool _isInitialized;

        public event Action<Socket> OnDisconnectedFromServer;

        protected EstablishedConnectionData ConnData
        {
            get => _connectionData;
        }

        protected IDisconnectFromServerHandler DisconnectHandler
        {
            get => _diconnectHandler;
        }

        protected NetworkPackageMeta NetworkPackageMeta => _packageMeta;

        protected Socket SocketToServer => ConnData.ClientSocket;

        protected IConnToServerHandlingThread()
        {
        }

        public void Init(IDisconnectFromServerHandler disconnectHandler, EstablishedConnectionData connData, NetworkPackageMeta packageMeta)
        {
            _connectionData = connData;
            _diconnectHandler = disconnectHandler;
            _packageMeta = packageMeta;
            _isInitialized = true;
        }

        public virtual void Shutdown()
        {
            _isInitialized = false;
        }

        public abstract void RunHandleConnection();

        public abstract void OnClientDisconnected(IConnToServerHandlingThread handler, EstablishedConnectionData connData);

    }
}
