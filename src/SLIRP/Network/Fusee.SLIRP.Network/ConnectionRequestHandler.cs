using Fusee.SLIRP.Network.Common;
using System.Net;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network
{
    internal class ConnectionRequestHandler : IConnectionRequestHandler
    {
        private ServerMetaData serverMetaData;
        private Socket serverSocket;

        IPEndPoint listeningEndPoint;

        bool threadContinue = false;

        public event Action<IConnectionRequestHandler, Socket> OnClientConnect;

        public ConnectionRequestHandler(ServerMetaData serverMetaData)
        {
            this.serverMetaData = serverMetaData;
        }


        public void Shutdown()
        {
            threadContinue = false;

            if(serverSocket != null) 
                serverSocket.Close();
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
                try
                {
                    Socket clientSocket = serverSocket.Accept();

                    OnClientConnect?.Invoke(this, clientSocket);

                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }
    }
}
