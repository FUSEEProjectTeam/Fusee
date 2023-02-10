// <copyright file=ConnectionMetaData company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 9:02:23 AM</date>


using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Common
{   public struct ConnectionMetaData
    {
        public int Port = 1337;

        public AddressFamily AddressFamily = AddressFamily.InterNetwork;
        public ProtocolType ProtocolType = ProtocolType.Udp;
        public SocketType SocketType = SocketType.Stream;
        public NetworkPackageMeta NetworkPackageMeta;

        public ConnectionMetaData(int port, AddressFamily addressFamily, ProtocolType protocolType, SocketType socketType, NetworkPackageMeta networkPackageMeta)
        {
            Port = port;
            AddressFamily = addressFamily;
            ProtocolType = protocolType;
            SocketType = socketType;
            NetworkPackageMeta = networkPackageMeta;
        }

        public ConnectionMetaData(ConnectionMetaData connectionMetaData)
        {
            Port = connectionMetaData.Port;
            AddressFamily = connectionMetaData.AddressFamily;
            ProtocolType = connectionMetaData.ProtocolType;
            SocketType = connectionMetaData.SocketType;
            NetworkPackageMeta = connectionMetaData.NetworkPackageMeta;
        }
    }
}
