// <copyright file=BasicConnectionRequester company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 9:51:54 AM</date>

using Fusee.SLIRP.Network.Client.Common;
using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Core
{
    public class BasicConnectionRequester : IConnectionRequester
    {
        private int _port;
        private IPAddress _serverAddress;
        private AddressFamily _addressFamily = AddressFamily.InterNetwork;
        private ProtocolType  _protocolType;
        private SocketType _socketType;

        private Socket _clientSocket;
        private IConnToServerReceiver _connectionToServerReceiver;

        private bool _isInitialized;
        private bool _isRunning;


        public void Init(ClientConnectionMetaData connectionMetaData, IConnToServerReceiver connectionToServerHandler)
        {
            _port= connectionMetaData.Port;
            _serverAddress= connectionMetaData.ServerIPAddress;
            _addressFamily = connectionMetaData.AddressFamily;
            _protocolType = connectionMetaData.ProtocolType;
            _socketType = connectionMetaData.SocketType;

            _isInitialized = true;
        }

        public void Shutdown()
        {
            if(!_isInitialized) return; 

            if(_isRunning)
                StopRequesting();

            _isInitialized = false;
        }

        public void StartRequesting()
        {
            if (!_isInitialized) return;

            if (_isRunning) return;

            _isRunning = true;

            RequestConnection();

        }

        public void StopRequesting()
        {
            _isRunning= false;
        }

        public virtual void RequestConnection()
        {
            _clientSocket = new Socket(_addressFamily,_socketType,_protocolType);
    
            EstablishedConnectionData establishedUdpConnectionData = new EstablishedConnectionData(_serverAddress, _port, _clientSocket);

            try
            {
                _clientSocket.Connect(_serverAddress, _port);
                _connectionToServerReceiver.OnConnectionEstablished(establishedUdpConnectionData);

            }
            catch(Exception e) 
            {
                Console.WriteLine("Verbindung fehlgeschlagen: " + e.Message);
                Console.WriteLine(e.StackTrace);

                _connectionToServerReceiver.OnConnectionFailed(establishedUdpConnectionData);
            }

            StopRequesting();
        }
    }
}
