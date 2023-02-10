// <copyright file=BasicConnectionToServerHanding company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 10:46:34 AM</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Fusee.SLIRP.Network.Client.Common;
using Fusee.SLIRP.Network.Common;

namespace Fusee.SLIRP.Network.Client.Core
{
    public class BasicConnectionToServerHandler<T> : IConnectionToServerHandler, IDisconnectFromServerHandler where T : IConnToServerHandlingThread, new()
    {
        private NetworkPackageMeta _packageMeta;

        private bool _isInitialized;
        private bool _isRunning;

        private Dictionary<EstablishedConnectionData, Thread> _handlingThreads = new Dictionary<EstablishedConnectionData, Thread>();


        public event Action<IConnectionToServerHandler, EstablishedConnectionData> EventOnConnectedToServer;
        public event Action<IConnectionToServerHandler, EstablishedConnectionData> EventOnConnectionToServerFailed;
        public event Action<IConnectionToServerHandler, EstablishedConnectionData> EventOnDisconnectedFromServer;

        public void Init( NetworkPackageMeta networkPackageMeta)
        {
            _packageMeta = networkPackageMeta;


            //set first so incomming connectioncan be handled directly
            _isInitialized = true;

        }
        public void Shutdown()
        {
            if (!_isInitialized)
                return;

            StopHandling();

            _isInitialized = false;
        }

        private void ShutdownAllHandlings()
        {
            foreach (var connData in _handlingThreads)
            {
                AbortThreadHandling(connData.Value);

                ShutdownSocket(connData.Key.ClientSocket);
            }
        }

        private static void ShutdownSocket(Socket socket, SocketShutdown how = SocketShutdown.Both)
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(how);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not shutdown socket! " + e.Message);
                }
            }
        }

        private static void AbortThreadHandling(Thread handlingThread)
        {
            try
            {
                if (handlingThread.ThreadState == ThreadState.Running)
                {
                    handlingThread.Abort();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not abort thread! " + e.Message);
            }
        }

        public void StartHandling()
        {
            if (_isInitialized) return;

            if(_isRunning) return;

            _isRunning = true;
    }

        public void StopHandling()
        {
            if (!_isInitialized) return;

            if(!_isRunning) return;

            ShutdownAllHandlings();

            _isRunning = false;
    }

        private void HandleConnection(EstablishedConnectionData connData)
        {
            if (!_isInitialized)
                return;

            if (connData.ClientSocket == null) throw new NullReferenceException("_connectionToServerReceiver was called without an argument");

            T connectionToServerHandling = new T();
            connectionToServerHandling.Init(this, connData, _packageMeta);

            Thread clientThread = new Thread(new ThreadStart(connectionToServerHandling.RunHandleConnection));

            AddSocketToDictionary(connData, clientThread);

            clientThread.Start();
        }

        public void OnConnectionEstablished(EstablishedConnectionData connData)
        {
            HandleConnection(connData);

            EventOnConnectedToServer?.Invoke(this, connData);
        }

        public void OnConnectionFailed(EstablishedConnectionData connData)
        {
            EventOnConnectionToServerFailed?.Invoke(this, connData);
        }

        private void AddSocketToDictionary(EstablishedConnectionData connData, Thread handledThread)
        {
            _handlingThreads.Add(connData, handledThread);
        }

        private void RemoveSocketFromDictionary(EstablishedConnectionData connData)
        {
            if (_handlingThreads.ContainsKey(connData))
            {
                _handlingThreads.Remove(connData);
            }
        }





    }


}

