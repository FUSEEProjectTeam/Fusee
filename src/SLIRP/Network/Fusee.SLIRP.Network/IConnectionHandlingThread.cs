using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network
{
    internal abstract class IConnectionHandlingThread
    {
        private Socket _clientSocket;
        private IDisconnectHandler _connectionHandler;

        private bool _isInitialized;

        //workaround to be able to assign a socket to the thread. See instantiation at ClientHandlers method "HandleClient".
        public Socket ClientSocket
        {
            get => _clientSocket;
            set
            {
                if (_clientSocket == null) _clientSocket = value;
                else Console.WriteLine("Dont assign clients sockets when already constructed.");
            }
        }


        public IDisconnectHandler ConnectionHandler
        {
            protected get => _connectionHandler;
            set
            {
                if (_connectionHandler == null)
                {
                    _connectionHandler = value;
                }
                else Console.WriteLine("Dont assign a disconnect handler when already initialized.");
            }
        }

        public event Action<Socket> OnClientClosed;

        public IConnectionHandlingThread()
        { }

        public void Init(Socket clientSocket, IDisconnectHandler connectionHandler)
        {
            _clientSocket = clientSocket;
            _connectionHandler = connectionHandler;

            _isInitialized = true;
        }

        public abstract void RunHandleConnection();

        public abstract void OnClientDisconnected(IConnectionHandlingThread handler, Socket disconnectedClient);
    }
}
