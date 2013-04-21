using System.Collections.Generic;
using Lidgren.Network;

namespace Fusee.Engine
{
    public class NetworkImp : INetworkImp
    {
        private NetPeerConfiguration _configuration;

        private NetPeer _netPeer;
        private NetServer _netServer;
        private NetClient _netClient;

        private bool _serverActive;

        public List<INetworkMsg> IncClientMsg { get; private set; }
        public List<INetworkMsg> IncServerMsg { get; private set; }

        public NetworkImp()
        {
            _configuration = new NetPeerConfiguration("FUSEE3D");

            _netPeer = new NetPeer(_configuration);
            _netClient = new NetClient(_configuration);
            _netServer = new NetServer(_configuration);

            IncClientMsg = new List<INetworkMsg>();
            IncServerMsg = new List<INetworkMsg>();

            _serverActive = false;
        }

        public bool OpenConnection(ConnectionType type, string host, int port)
        {
            if (type == ConnectionType.CtPeer)
            {
                _netPeer.Start();

                var hail = _netPeer.CreateMessage("OpenConnection");
                var connection = _netPeer.Connect(host, port, hail);
            }

            if (type == ConnectionType.CtClient)
            {
                _netClient.Start();

                var hail = _netClient.CreateMessage("OpenConnection");
                var connection = _netClient.Connect(host, port, hail);
            }

            if (type == ConnectionType.CtServer)
            {
                _netServer.Configuration.Port = port;
                _netServer.Start();

                _serverActive = true;
            }

            return true;
        }

        public bool SendMessage(string msg)
        {
            NetOutgoingMessage sendMsg = _netClient.CreateMessage();
            sendMsg.Write(msg);
            
            _netClient.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);

            return true;
        }

        public void OnUpdateFrame()
        {
            NetIncomingMessage msg;

            //if (_netClient.ConnectionStatus == NetConnectionStatus.Connected)
                while ((msg = _netClient.ReadMessage()) != null)
                {
                    var netMsg = new NetworkMessage();

                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus)msg.ReadByte();
                            netMsg = new NetworkMessage((MessageType)msg.MessageType, (ConnectionStatus)status);
                            break;
                        case NetIncomingMessageType.Data:
                            netMsg = new NetworkMessage((MessageType)msg.MessageType, msg.ReadString());
                            break;
                    }

                    IncClientMsg.Add(netMsg);
                    _netClient.Recycle(msg);
                }

            if (!_serverActive) return;

            while ((msg = _netServer.ReadMessage()) != null)
            {
                var netMsg = new NetworkMessage();

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        var status = (NetConnectionStatus) msg.ReadByte();
                        netMsg = new NetworkMessage((MessageType) msg.MessageType, (ConnectionStatus) status);
                        break;
                    case NetIncomingMessageType.Data:
                        netMsg = new NetworkMessage((MessageType) msg.MessageType, msg.ReadString());
                        break;
                }

                IncServerMsg.Add(netMsg);
                _netServer.Recycle(msg);

                /*switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = msg.ReadString();
                        System.Diagnostics.Debug.WriteLine(text);
                        break;
                  
                    case NetIncomingMessageType.Data:
                        // incoming chat message from a client
                        string chat = im.ReadString();

                        System.Diagnostics.Debug.WriteLine("Broadcasting '" + chat + "'");*/

            }
        }
    }
    
}
