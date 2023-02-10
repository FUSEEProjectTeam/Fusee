// <copyright file=BasicPingPongHandler company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 9:36:43 AM</date>

using Fusee.SLIRP.Network.Server.Common;
using System.Net.Sockets;
using System.Text;

namespace Fusee.SLIRP.Network.Server.Examples
{
    /// <summary>
    /// Basic example for a <see cref="IDisposableConnectionHandling"/> who just follows a ping pong.
    /// </summary>
    public class BasicPingPongHandling : IDisposableConnectionHandling
    {
        public override void OnClientDisconnected(IConnectionHandlingThread handler, Socket disconnectedClient)
        {
            throw new NotImplementedException();
        }

        public override void RunHandleConnection()
        {
            while (true)
            {
                byte[] buffer = new byte[NetworkPackageMeta.BufferSize];
                int bytesReceived = ClientSocket.Receive(buffer);

                string messageReceived = ASCIIEncoding.ASCII.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("[Basic Ping Pong] Received: " + messageReceived);

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

    }
}
