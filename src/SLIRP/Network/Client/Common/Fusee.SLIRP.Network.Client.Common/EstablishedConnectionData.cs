// <copyright file=EstablishedConnectionData company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 10:22:35 AM</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Common
{
    public class EstablishedConnectionData
    {
        private IPAddress _connectionAddress;
        private int _port;
        private Socket _clientSocket;

        public IPAddress ConnectionAddress => _connectionAddress;
        public int Port => _port;

        public Socket ClientSocket => _clientSocket;

        public EstablishedConnectionData(IPAddress connectionAddress, int port, Socket clientSocket)
        {
            _connectionAddress = connectionAddress;
            _port = port;
            _clientSocket = clientSocket;
        }
    }
}
