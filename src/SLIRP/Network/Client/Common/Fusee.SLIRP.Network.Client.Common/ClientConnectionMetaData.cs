// <copyright file=ClientConnectionMetaData company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 8:41:06 AM</date>

using Fusee.SLIRP.Network.Common;
using System.Net;
using System.Net.Sockets;


namespace Fusee.SLIRP.Network.Client.Common
{
    public struct ClientConnectionMetaData
    {
        public ConnectionMetaData connData;

        /// <summary>
        /// The IP address to connect to. 
        /// </summary>
        public IPAddress ServerIPAddress;


        public int Port => connData.Port;
        public AddressFamily AddressFamily => connData.AddressFamily;
        public ProtocolType ProtocolType => connData.ProtocolType;
        public SocketType SocketType => connData.SocketType;
        public NetworkPackageMeta NetworkPackageMeta => connData.NetworkPackageMeta;

        public ClientConnectionMetaData (IPAddress serverIpAddress, int port, AddressFamily addressFamily, ProtocolType protocolType, SocketType socketType, NetworkPackageMeta networkPackageMeta)
        {
            ServerIPAddress = serverIpAddress;
            connData = new ConnectionMetaData (port,addressFamily,protocolType,socketType,networkPackageMeta);
        }

        public ClientConnectionMetaData(IPAddress serverIpAddress, ConnectionMetaData connectionMetaData, NetworkPackageMeta networkPackageMeta)
        {
            ServerIPAddress = serverIpAddress;
            connData = new ConnectionMetaData(connectionMetaData);
        }
    }
}
