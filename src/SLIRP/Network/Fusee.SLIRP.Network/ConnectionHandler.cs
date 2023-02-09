using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network
{
    /// <summary>
    /// Handles all accepted connections by the <see cref="IConnectionRequestHandler"/> and also the disconnecting connections. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ConnectionHandler<T> : IConnectionHandler, IConnectHandler, IDisconnectHandler, IDisposable where T : IConnectionHandlingThread, new ()
    {
        private IConnectionRequestHandler _requestHandler;
        private bool _isInitialized;

        Dictionary<Socket, Thread> handlingThreads = new Dictionary<Socket, Thread>();

        public void Init(IConnectionRequestHandler requestHandler)
        {
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

            _requestHandler.OnClientConnect += HandleClient;
        }

        private void DeregisterFromRequestHandler()
        {
            if (_requestHandler != null)
            {
                _requestHandler.OnClientConnect -= HandleClient;
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            if (!_isInitialized)
                return;

            if (clientSocket == null) throw new NullReferenceException("Client Handler was called without an argument");

            T clientHandling = new T();
            clientHandling.Init(clientSocket, this);

            Thread clientThread = new Thread(new ThreadStart(clientHandling.RunHandleConnection));

            handlingThreads.Add(clientSocket, clientThread);

            clientThread.Start();
        }

        public void OnClientConnected(IConnectHandler sender, Socket clientSocket)
        {
            if (sender != _requestHandler)
                return;

            HandleClient(clientSocket);
        }

        public void OnClientDisconnected(IConnectionHandlingThread sender, Socket clientSocket)
        {
            
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
