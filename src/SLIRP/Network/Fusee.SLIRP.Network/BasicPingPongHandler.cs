using Fusee.SLIRP.Network.Common;
using System.Net.Sockets;
using System.Text;

namespace Fusee.SLIRP.Network
{
    /// <summary>
    /// Controls what happens while a client is connected.
    /// </summary>
    internal class BasicPingPongHandler : IConnectionHandlingThread, IDisposable
    {
        public BasicPingPongHandler():base() { }

        public void Dispose()
        {
            OnClientDisconnected(this, ClientSocket);
        }

        public override void RunHandleConnection()
        {
          while (true)
            {
                byte[] buffer = new byte[NetworkPackageMeta.BufferSize];
                int bytesReceived = ClientSocket.Receive(buffer);
                
                string messageReceived = ASCIIEncoding.ASCII.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("[Basic Ping Pong] Received: "+messageReceived);
               
                buffer = ASCIIEncoding.ASCII.GetBytes("Pong");
                Console.WriteLine("[Basic Ping Pong] Send: " + "Pong");
                ClientSocket.Send(buffer);

                buffer = ASCIIEncoding.ASCII.GetBytes("Ping");
                Console.WriteLine("[Basic Ping Pong] Send: " + "Ping");
                ClientSocket.Send(buffer);

                bytesReceived = ClientSocket.Receive(buffer);
                messageReceived = ASCIIEncoding.ASCII.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("[Basic Ping Pong] Received: " + messageReceived);

                break;
            }
        }

        public override void OnClientDisconnected(IConnectionHandlingThread handler, Socket disconnectedClient)
        {

        }
    }
}
