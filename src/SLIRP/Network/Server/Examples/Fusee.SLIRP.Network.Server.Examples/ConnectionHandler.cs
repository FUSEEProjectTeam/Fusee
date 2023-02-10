using System.Net.Sockets;
using Fusee.SLIRP.Network.Common;
using Fusee.SLIRP.Network.Server.Common;

namespace Fusee.SLIRP.Network.Server.Examples
{
    /// <summary>
    /// Handles all accepted connections by the <see cref="IConnectionRequestHandler"/> and also the disconnecting connections. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ConnectionHandler<T> : IConnectionHandler, IDisposable where T : IConnectionHandlingThread, new()
    {
        private IConnectionRequestHandler _requestHandler;
        private NetworkPackageMeta _packageMeta;

        private bool _isInitialized;
        private bool _isHandling;

        private Dictionary<Socket, Thread> _handlingThreads = new Dictionary<Socket, Thread>();

        public bool IsRunning => _isHandling;

        public bool IsHandlingConnections => _isHandling;
        public bool IsHandlingDisconnecs => _isHandling;

        public event Action<IConnectionHandler, Socket> EventClientConnected;

        public event Action<IConnectionHandler, Socket> EventClientDisconnected;

        public void Init(NetworkPackageMeta networkPackageMeta)
        {
            _packageMeta = networkPackageMeta;

            //set first so incomming connectioncan be handled directly
            _isInitialized = true;
        }

        public void StartHandling()
        {
            if (!_isInitialized)
                return;

            if (_isHandling)
                return;

            _isHandling = true;
        }

        public void StopHandling()
        {
            if (!_isHandling)
                return;

            //if a handler stops handling, all handlings have to be shutdown
            //e.g. if a server stops, the connections have to be closed.
            // if the server starts all clients have to reconnect.
            ShutdownAllHandlings();

            _isHandling = false;
        }

        public void Shutdown()
        {
            //DeregisterFromRequestHandler();

            ShutdownAllHandlings();

            _isInitialized = false;
        }

        private void ShutdownAllHandlings()
        {
            if (_handlingThreads != null && _handlingThreads.Count > 0)
                foreach (var handling in _handlingThreads)
                {
                    Thread handlingThread = handling.Value;
                    Socket handlingSocket = handling.Key;

                    ShutdownThread(handlingThread);

                    ShutdownSocket(handlingSocket);

                }
        }

        private static void ShutdownSocket(Socket handlingSocket)
        {
            if (handlingSocket != null)
            {
                try
                {

                    handlingSocket.Close();
                    handlingSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was a problem closing and shutingdown socket! " + e.Message);
                }
            }
        }

        private static void ShutdownThread(Thread handlingThread)
        {
            if (handlingThread != null)
            {
                try
                {
                    if (handlingThread.ThreadState == ThreadState.Running)
                    {

                        handlingThread.Abort();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was a problem aborting connection handling thread! " + e.Message);
                }

            }
        }

        //private void RegisterAtRequestHandler()
        //{
        //    if (_requestHandler == null) throw new NullReferenceException("Client Handler initialization was called without an IConnectionRequestHandler!");

        //    _requestHandler.OnClientConnect += OnClientConnected;
        //}

        //private void DeregisterFromRequestHandler()
        //{
        //    if (_requestHandler != null)
        //    {
        //        _requestHandler.OnClientConnect -= OnClientConnected;
        //    }
        //}

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
