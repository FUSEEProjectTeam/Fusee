using Fusee.SLIRP.Network.Common;
using Fusee.SLIRP.Network.Server.Common;
using System.Net;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Server.Examples
{
    /// <summary>
    /// Binds the socket and listens on it for new connections.
    /// In order to be able to accept connections, the passed <see cref="IConnectHandler"/>
    /// has to be running.
    /// </summary>
    internal class ConnectionRequestHandler : IConnectionRequestHandler
    {
        private IConnectionHandler _connectHandler;
        private ServerMetaData serverMetaData;
        private Socket serverSocket;

        private IPEndPoint listeningEndPoint;

        bool threadContinue = false;

        private bool _isInitialized;

        public void Init (ServerMetaData serverMetaData, IConnectionHandler connectHandler)
        {
            this.serverMetaData = serverMetaData;
            _connectHandler = connectHandler;
            _isInitialized = true;
        }


        public void Shutdown()
        {
            if (!_isInitialized && !threadContinue)
                return;

            threadContinue = false;

            if (serverSocket != null)
            {
                try
                {
                    serverSocket.Close();
                    serverSocket.Shutdown(SocketShutdown.Both);
                }
                catch(Exception e)
                {
                    Console.WriteLine("There was a problem closing and shutting down the socket! " + e.Message);
                }
            }

            _isInitialized = false;
        }

        public void Run()
        {
            threadContinue = true;

            serverSocket = serverMetaData.ServerSocket;

            listeningEndPoint = new IPEndPoint(serverMetaData.ConnectionData.ValidIPAdress, serverMetaData.ConnectionData.Port);

            serverSocket.Bind(listeningEndPoint);
            serverSocket.Listen(serverMetaData.MaxConnections);

            while (threadContinue)
            {
                if (!_connectHandler.IsRunning)
                    continue;

                try
                {
                    Socket clientSocket = serverSocket.Accept();

                    _connectHandler.OnClientConnected(this, clientSocket);
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }
    }
}
