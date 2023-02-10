// <copyright file=BasicConnToServerHandlingThread company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 11:21:17 AM</date>

using Fusee.SLIRP.Network.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Core
{
    internal class BasicPingPongWithServerHandling : IDisposableConnToServerHandlingThread
    {

        public override void OnClientDisconnected(IConnToServerHandlingThread handler, EstablishedConnectionData connData)
        {
            throw new NotImplementedException();
        }

        public override void RunHandleConnection()
        {
            while (true)
            {
                byte[] buffer = new byte[NetworkPackageMeta.BufferSize];
                int bytesReceived = SocketToServer.Receive(buffer);

                buffer = ASCIIEncoding.ASCII.GetBytes("Ping");
                Console.WriteLine("[Basic Ping Pong] Send: " + "Ping");
                SocketToServer.Send(buffer);

                string messageReceived = ASCIIEncoding.ASCII.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("[Basic Ping Pong] Received: " + messageReceived);

                bytesReceived = SocketToServer.Receive(buffer);
                messageReceived = ASCIIEncoding.ASCII.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("[Basic Ping Pong] Received: " + messageReceived);

                buffer = ASCIIEncoding.ASCII.GetBytes("Pong");
                Console.WriteLine("[Basic Ping Pong] Send: " + "Pong");
                SocketToServer.Send(buffer);                

                break;
            }
        }
    }
}
