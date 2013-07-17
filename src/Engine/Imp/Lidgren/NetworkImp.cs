using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Timers;
using Lidgren.Network;

using Timer = System.Timers.Timer;

namespace Fusee.Engine
{
    public class NetworkImp : INetworkImp
    {
        private NetPeer _netPeer;
        private NetServer _netServer;
        private NetClient _netClient;

        private Timer _discoveryTimeout;

        private int _peerID;
        private string _peerIDString;

        private NetConfigValues _config;
        private NetPeerConfiguration _netConfig;

        public NetConfigValues Config
        {
            get { return _config; }
            set
            {
                _netConfig.RedirectPackets = value.RedirectPackets;

                if (value.Discovery)
                {
                    _netConfig.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
                    _netConfig.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                }
                else
                {
                    _netConfig.DisableMessageType(NetIncomingMessageType.DiscoveryResponse);
                    _netConfig.DisableMessageType(NetIncomingMessageType.DiscoveryRequest);
                }
            }
        }

        public event ConnectionUpdateEvent ConnectionUpdate;
        public List<INetworkConnection> Connections { get; private set; }

        public NetStatusValues Status { get; set; }
        public List<INetworkMsg> IncomingMsg { get; private set; }

        public NetworkImp()
        {
            _netConfig = new NetPeerConfiguration("FUSEE3D");

            IncomingMsg = new List<INetworkMsg>();

            _config = new NetConfigValues
                          {
                              SysType = SysType.None,
                              DefaultPort = 14242,
                              Discovery = false,
                              ConnectOnDiscovery = false,
                              DiscoveryTimeout = 5000,
                              RedirectPackets = false,
                          };

            // _netConfig.RedirectedPacketsList.CollectionChanged += PackageCapture;
            Connections = new List<INetworkConnection>();

            Status = new NetStatusValues
            {
                Connected = false,
                LastStatus = ConnectionStatus.None
            };
        }

        public void StartPeer(int port)
        {
            // Check if already running
            EndPeers();
            CloseDevices();

            _netConfig = _netConfig.Clone();
            _netConfig.Port = port;

            // peerID
            var random = new Random();
            _peerID = random.Next(0, 100);
            
            switch (_config.SysType)
            {
                case SysType.Peer:
                    _netPeer = new NetPeer(_netConfig);
                    _netPeer.Start();

                    _peerIDString = "FUSEE3D:Peer:" + _peerID;

                    break;

                case SysType.Client:
                    _netClient = new NetClient(_netConfig);
                    _netClient.Start();

                    _peerIDString = "FUSEE3D:Client:" + _peerID;

                    break;

                case SysType.Server:
                    _netServer = new NetServer(_netConfig);
                    _netServer.Start();

                    _peerIDString = "FUSEE3D:Server:" + _peerID;

                    break;
            }
        }

        public void EndPeers()
        {
            EndPeer(SysType.Peer);
            EndPeer(SysType.Client);
            EndPeer(SysType.Server);
        }

        public void EndPeer(SysType sysType)
        {
            switch (sysType)
            {
                case SysType.Peer:
                    if (_netPeer == null) return;

                    if (_netPeer.Status == NetPeerStatus.Running)
                        _netPeer.Shutdown("Shutting Down");

                    break;

                case SysType.Client:
                    if (_netClient == null) return;

                    if (_netClient.Status == NetPeerStatus.Running)
                        _netClient.Shutdown("Shutting Down");

                    break;

                case SysType.Server:
                    if (_netServer == null) return;

                    if (_netServer.Status == NetPeerStatus.Running)
                        _netServer.Shutdown("Shutting Down");

                    break;
            }
        }

        public string GetLocalIp()
        {
            IPAddress ipMask;
            return NetUtility.GetMyAddress(out ipMask).ToString();
        }

        private void OnConnectionUpdate(ConnectionStatus lastStatus, NetConnection senderConnection)
        {
            Connections.Clear();

            switch (_config.SysType)
            {
                case SysType.Peer:
                    break;

                case SysType.Client:
                    break;

                case SysType.Server:
                    if (_netServer == null)
                        break;

                    Connections.AddRange(
                        _netServer.Connections.Select(
                            connection => new NetworkConnection {Connection = connection, NetworkImp = this}));

                    break;
            }

            // event OnConnectionUpdate
            var newConnection = new NetworkConnection {Connection = senderConnection, NetworkImp = this};

            if (ConnectionUpdate != null)
                ConnectionUpdate(lastStatus, newConnection);
        }

        public bool OpenConnection(SysType type, IPEndPoint ip)
        {
            return OpenConnection(type, ip.Address.ToString(), ip.Port);
        }

        public bool OpenConnection(SysType type, string host, int port)
        {
            NetConnection connection = null;

            if (type == SysType.Peer)
            {
                // START FIRST!
                var hail = _netPeer.CreateMessage("OpenConnection");
                connection = _netPeer.Connect(host, port, hail);
            }

            if (type == SysType.Client)
            {
                // START FIRST!
                var hail = _netClient.CreateMessage("OpenConnection");
                connection = _netClient.Connect(host, port, hail);
            }

            return
                (connection != null) && connection.Status == NetConnectionStatus.Connected;
        }

        public void CloseConnections()
        {
            CloseConnection(SysType.Peer);
            CloseConnection(SysType.Client);
            CloseConnection(SysType.Server);
        }

        public void CloseConnection()
        {
            CloseConnection(_config.SysType);
        }

        public void CloseConnection(SysType sysType)
        {
            switch (sysType)
            {
                case SysType.Peer:
                    foreach (var con in _netPeer.Connections)
                        con.Disconnect("Disconnecting");
                        
                    break;

                case SysType.Client:
                    foreach (var con in _netClient.Connections)
                        con.Disconnect("Disconnecting");
                    
                    break;

                case SysType.Server:
                    foreach (var con in _netServer.Connections)
                        con.Disconnect("Disconnecting");

                    break;
            }

            Thread.Sleep(1000);
        }

        public void CloseDevices()
        {
            if (_netPeer != null)
            {
                CloseConnection(SysType.Peer);
                _netPeer = null;
            }

            if (_netClient != null)
            {
                CloseConnection(SysType.Client);
                _netClient = null;
            }

            if (_netServer != null)
            {
                CloseConnection(SysType.Server);
                _netServer = null;
            }
        }

        public bool SendMessage(byte[] msg, MessageDelivery msgDelivery, int msgChannel)
        {
            // _netConfig.RedirectPackets = true;

            NetSendResult sendResult;

            switch (_config.SysType)
            {
                case SysType.Peer:
                    //sendMsg = _netPeer.CreateMessage(msg);
                    //sendMsg.Write(msg);

                    break;

                case SysType.Client:
                    var sendMsgClient = _netClient.CreateMessage();
                    sendMsgClient.Write(msg);

                    sendResult = _netClient.SendMessage(sendMsgClient, (NetDeliveryMethod) msgDelivery, msgChannel);
                    return (sendResult == NetSendResult.Sent);

                case SysType.Server:
                    var sendMsgServer = _netServer.CreateMessage();
                    sendMsgServer.Write(msg);

                    if (_netServer.ConnectionsCount > 0)
                        _netServer.SendMessage(sendMsgServer, _netServer.Connections, (NetDeliveryMethod)msgDelivery, msgChannel);

                    return true;
            }

            return false;
        }

        public bool SendMessage(byte[] msg, NetConnection connection, MessageDelivery msgDelivery, int msgChannel)
        {
            // _netConfig.RedirectPackets = true;

            NetSendResult sendResult;

            switch (_config.SysType)
            {
                case SysType.Peer:
                    //sendMsg = _netPeer.CreateMessage(msg);
                    //sendMsg.Write(msg);

                    break;

                case SysType.Client:
                    var sendMsgClient = _netClient.CreateMessage();
                    sendMsgClient.Write(msg);

                    sendResult = _netClient.SendMessage(sendMsgClient, connection, (NetDeliveryMethod)msgDelivery, msgChannel);
                    return (sendResult == NetSendResult.Sent);

                case SysType.Server:
                    var sendMsgServer = _netServer.CreateMessage();
                    sendMsgServer.Write(msg);

                    sendResult = _netServer.SendMessage(sendMsgServer, connection, (NetDeliveryMethod)msgDelivery, msgChannel);
                    return (sendResult == NetSendResult.Sent);
            }

            return false;
        }

        private void OnDiscoveryTimeout(object source, ElapsedEventArgs e)
        {
            _discoveryTimeout.Dispose();

            if (Status.Connecting)
            {
                Status.Connecting = false;
                Status.LastStatus = ConnectionStatus.Disconnected;
            }
        }

        public void SendDiscoveryMessage(int port)
        {
            switch (_config.SysType)
            {
                case SysType.Peer:
                    _netPeer.DiscoverLocalPeers(port);
                    break;

                case SysType.Client:
                    _netClient.DiscoverLocalPeers(port);
                    break;

                case SysType.Server:
                    _netServer.DiscoverLocalPeers(port);
                    break;
            }

            Status.Connecting = true;

            if (_config.DiscoveryTimeout > 0)
            {
                _discoveryTimeout = new Timer(_config.DiscoveryTimeout);
                _discoveryTimeout.Elapsed += OnDiscoveryTimeout;
                _discoveryTimeout.Enabled = true;
            }
        }

        private void SendDiscoveryResponse(IPEndPoint ip)
        {
            NetOutgoingMessage response;

            switch (_config.SysType)
            {
                case SysType.Peer:
                    response = _netPeer.CreateMessage();
                    response.Write(_peerIDString);

                    _netPeer.SendDiscoveryResponse(response, ip);

                    break;

                case SysType.Client:
                    response = _netClient.CreateMessage();
                    response.Write(_peerIDString);

                    _netClient.SendDiscoveryResponse(response, ip);

                    break;

                case SysType.Server:
                    response = _netServer.CreateMessage();
                    response.Write(_peerIDString);

                    _netServer.SendDiscoveryResponse(response, ip);

                    break;
            }
        }

        private NetworkMessage ReadMessage(NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    Status.LastStatus = (ConnectionStatus) msg.ReadByte();

                    switch (Status.LastStatus)
                    {
                        case ConnectionStatus.Connected:
                            OnConnectionUpdate(Status.LastStatus, msg.SenderConnection);

                            // TODO: This is not correct if server
                            Status.Connecting = false;
                            Status.Connected = true;

                            break;
                        case ConnectionStatus.Disconnected:
                            OnConnectionUpdate(Status.LastStatus, msg.SenderConnection);

                            // TODO: This is not correct if server
                            Status.Connecting = false;
                            Status.Connected = false;

                            break;
                        case ConnectionStatus.InitiatedConnect:
                            // TODO: This is not correct if server
                            Status.Connected = false;
                            Status.Connecting = true;

                            break;
                    }

                    return new NetworkMessage
                        {
                            Type = (MessageType) msg.MessageType,
                            Status = Status.LastStatus,
                            Sender = new NetworkConnection {Connection = msg.SenderConnection}
                        };

                case NetIncomingMessageType.DiscoveryRequest:
                        SendDiscoveryResponse(msg.SenderEndPoint);

                    return new NetworkMessage
                        {
                            Type = (MessageType) msg.MessageType,
                            Sender = new NetworkConnection {Connection = msg.SenderConnection}
                        };

                case NetIncomingMessageType.DiscoveryResponse:
                    var discoveryID = msg.ReadString();

                    if (discoveryID == _peerIDString)
                        return null;

                    if (_config.ConnectOnDiscovery)
                        OpenConnection(_config.SysType, msg.SenderEndPoint);

                    if (_discoveryTimeout != null)
                        _discoveryTimeout.Dispose();

                    return new NetworkMessage
                        {
                            Type = (MessageType) msg.MessageType,
                            Sender = new NetworkConnection {RemoteEndPoint = msg.SenderEndPoint},
                            Message = new NetworkMsgType {MsgType = MsgDataTypes.String, ReadString = discoveryID}
                        };

                case NetIncomingMessageType.Data:
                    return new NetworkMessage
                        {
                            Type = (MessageType) msg.MessageType,
                            Sender = new NetworkConnection {Connection = msg.SenderConnection},
                            Message =
                                new NetworkMsgType
                                    {
                                        MsgType = MsgDataTypes.Bytes,
                                        ReadBytes = msg.ReadBytes(msg.LengthBytes)
                                    }
                        };

                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    return new NetworkMessage
                        {
                            Type = (MessageType) msg.MessageType,
                            Sender = new NetworkConnection(),
                            Message = new NetworkMsgType {MsgType = MsgDataTypes.String, ReadString = msg.ReadString()}
                        };
            }

            return null;
        }

        public void OnUpdateFrame()
        {
            NetIncomingMessage msg;

            if (_netPeer != null)
            {
                while ((msg = _netPeer.ReadMessage()) != null)
                {
                    IncomingMsg.Add(ReadMessage(msg));
                    _netPeer.Recycle(msg);
                }
            }

            if (_netClient != null)
            {
                while ((msg = _netClient.ReadMessage()) != null)
                {
                    IncomingMsg.Add(ReadMessage(msg));
                    _netClient.Recycle(msg);
                }
            }

            if (_netServer != null)
            {
                while ((msg = _netServer.ReadMessage()) != null)
                {
                    IncomingMsg.Add(ReadMessage(msg));
                    _netServer.Recycle(msg);
                }
            }
        }
    }
}
