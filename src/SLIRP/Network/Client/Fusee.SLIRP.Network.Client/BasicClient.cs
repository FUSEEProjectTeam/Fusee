// <copyright file=Client company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/9/2023 5:08:56 PM</date>

using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Fusee.SLIRP.Network.Client.Common;

namespace Fusee.SLIRP.Network.Client.Core
{
    public class BasicClient : IClient
    {
        public const int BUFFERSIZE = 1024;
        public static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        public static ClientConnectionMetaData DefaultSLIRP = new ClientConnectionMetaData(IPAddress.Any, 1300, AddressFamily.InterNetwork, ProtocolType.Udp, SocketType.Stream, new NetworkPackageMeta(BUFFERSIZE));

        private bool _isInitialized;
        private bool _isRunning;

        private Socket _socket;

        private IConnectionRequester _requester;
        private IConnectionToServerHandler? _connToServerHandler;

        private Thread? _connRequesterThread;
        private Thread? _connToServerHandlerThread;

        private ClientConnectionMetaData curClientConnectionMetaData;


        public void Init(in ClientConnectionMetaData? metaData = null)
        {
            if (_isRunning || _isInitialized) return;

            _isInitialized = true;

            if (metaData != null)
            {
                curClientConnectionMetaData = metaData.Value;
            }
            else
            {
                curClientConnectionMetaData = DefaultSLIRP;
                //curServerConnectionMetaData.NetworkPackageMeta = new NetworkPackageMeta(BUFFERSIZE);
            }

            _socket = new Socket(curClientConnectionMetaData.AddressFamily, curClientConnectionMetaData.SocketType, curClientConnectionMetaData.ProtocolType);

            _requester = new BasicConnectionRequester();
            _connToServerHandler = new BasicConnectionToServerHandler<BasicPingPongWithServerHandling>();

            _connToServerHandler.Init(curClientConnectionMetaData.NetworkPackageMeta);
            _requester.Init(curClientConnectionMetaData, _connToServerHandler);

            _connRequesterThread = new Thread(new ThreadStart(_requester.StartRequesting));
            _connToServerHandlerThread = new Thread(new ThreadStart(_connToServerHandler.StartHandling));
        }

        public void Shutdown()
        {
            if (!_isInitialized)
                return;

            StopClient();

            _requester.Shutdown();
            _connToServerHandler.Shutdown();
        }

        public void StartClient()
        {
            if (!_isInitialized)
                return;

            if (_isRunning) return;

            _connToServerHandlerThread.Start();
            _connRequesterThread.Start();
        }

        public void StopClient()
        {
            if (!_isInitialized)
                return;

            if(!_isRunning) return;

            _requester.StopRequesting();
            _connToServerHandler.StopHandling();

            InterruptThread(_connRequesterThread);

            InterruptThread(_connToServerHandlerThread);
        }

        public static void InterruptThread(Thread threadToInterrupt)
        {
            if (threadToInterrupt.ThreadState == ThreadState.Running)
            {
                try
                {
                    threadToInterrupt.Interrupt();
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was a problem interrupting the thread! " + e.Message);
                }
            }
        }
    }
}
