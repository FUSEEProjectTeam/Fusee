using System.Net.Sockets;
using Fusee.SLIRP.Network.Common;

namespace Fusee.SLIRP.Network
{
    /// <summary>
    /// Handles all accepted connections by the <see cref="IConnectionRequestHandler"/> and also the disconnecting connections. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ConnectionHandler<T> : IConnectionHandler, IConnectHandler, IDisconnectHandler, IDisposable where T : IConnectionHandlingThread, new ()
    {
        private IConnectionRequestHandler _requestHandler;
        private NetworkPackageMeta _packageMeta;    

        private bool _isInitialized;

        private Dictionary<Socket, Thread> _handlingThreads = new Dictionary<Socket, Thread>();

        public event Action<IConnectionHandler, Socket> EventClientConnected;

        public event Action<IConnectionHandler, Socket> EventClientDisconnected;

        public void Init(IConnectionRequestHandler requestHandler, NetworkPackageMeta packageMeta)
        {
            _packageMeta = packageMeta;

            //make sure that it is not already registered
            DeregisterFromRequestHandler();

            _requestHandler = requestHandler;

            //set first so incomming connectioncan be handled directly
            _isInitialized = true;

            RegisterAtRequestHandler();
        }

        public void Shutdown()
        {
           DeregisterFromRequestHandler();

            _isInitialized = false;
        }

        private void RegisterAtRequestHandler()
        {
            if (_requestHandler == null) throw new NullReferenceException("Client Handler initialization was called without an IConnectionRequestHandler!");

            _requestHandler.OnClientConnect += OnClientConnected;
        }

        private void DeregisterFromRequestHandler()
        {
            if (_requestHandler != null)
            {
                _requestHandler.OnClientConnect -= OnClientConnected;
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            if (!_isInitialized)
                return;

            if (clientSocket == null) throw new NullReferenceException("Client Handler was called without an argument");

            T clientHandling = new T();
            clientHandling.Init(this, clientSocket, _packageMeta);

            Thread clientThread = new Thread(new ThreadStart(clientHandling.RunHandleConnection));

            AddSocketToDictionary(clientSocket, clientThread);

            clientThread.Start();
        }

        public void OnClientConnected(IConnectionRequestHandler sender, Socket clientSocket)
        {
            if (sender != _requestHandler)
                return;

            HandleClient(clientSocket);

            EventClientConnected?.Invoke(this, clientSocket);
        }

        public void OnClientDisconnected(IConnectionHandlingThread sender, Socket clientSocket)
        {
            if (sender != _requestHandler)
                return;

            RemoveSocketFromDictionary(clientSocket);

            EventClientDisconnected?.Invoke(this, clientSocket);
        }

        private void AddSocketToDictionary(Socket clientSocket, Thread clientThread)
        {
            _handlingThreads.Add(clientSocket, clientThread);
        }

        private void RemoveSocketFromDictionary(Socket clientSocket)
        {
            if (_handlingThreads.ContainsKey(clientSocket))
            {
                _handlingThreads.Remove(clientSocket);
            }
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
