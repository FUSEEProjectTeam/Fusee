// <copyright file=IConnectHandler company="Hochschule Furtwangen">
//   Copyright (c) 2023 All Rights Reserved
//   </copyright>
//   <author>Marc-Alexander Lohfink</author>
//   <date>2/9/2023 4:58:00 PM</date>


using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Common
{
    public interface IConnectHandler
    {
        public void OnClientConnected(IConnectionRequestHandler sender, Socket clientSocket);
    }
}
