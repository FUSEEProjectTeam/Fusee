// <copyright file=IConnectionToServerHandler company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 10:44:41 AM</date>

using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Common
{
    public interface IConnectionToServerHandler : IConnToServerReceiver
    {

        public event Action<IConnectionToServerHandler, EstablishedConnectionData> EventOnConnectedToServer;
        public event Action<IConnectionToServerHandler, EstablishedConnectionData> EventOnDisconnectedFromServer;

        /// <summary>
        /// Initialize the Handler so that he is able to handle the running connection handling threads.
        /// </summary>
        /// <param name="handlingPattern">What kind of handling thread will be instantiated when a new client connects.</param>
        public void Init(NetworkPackageMeta networkPackageMeta);

        public void Shutdown();

        public void StartHandling();

        public void StopHandling();

    }
}
