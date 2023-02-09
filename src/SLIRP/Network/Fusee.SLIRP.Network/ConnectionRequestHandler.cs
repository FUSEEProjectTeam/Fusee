using System.Net;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network
{
    internal class ConnectionRequestHandler : IConnectionRequestHandler
    {
        private RenderServerMetaData serverMetaData;

        IPEndPoint listeningEndPoint;

        List<Socket> connectedClients;

        bool threadContinue = false;

        public event Action<IConnectionRequestHandler, Socket> OnClientConnect;

        public ConnectionRequestHandler(RenderServerMetaData serverMetaData)
        {
            this.serverMetaData = serverMetaData;
            connectedClients = new List<Socket>();
        }
        
        public void Run()
        {
            threadContinue = true;

            Socket workingSocket = serverMetaData.RenderSocket;

            listeningEndPoint = new IPEndPoint(serverMetaData.ConnectionData.ValidIPAdress, serverMetaData.ConnectionData.Port);

            workingSocket.Bind(listeningEndPoint);
            workingSocket.Listen(serverMetaData.MaxConnections);
 
            while (threadContinue)
            {
                Socket clientSocket = workingSocket.Accept();
                connectedClients.Add(clientSocket);

                OnClientConnect?.Invoke(this, clientSocket);
            }
        }

    }
}
