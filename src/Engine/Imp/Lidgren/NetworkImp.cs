using System.Collections.Generic;
using Lidgren.Network;

namespace Fusee.Engine
{
    public class NetworkImp : INetworkImp
    {
        private NetPeerConfiguration _configuration;
        private NetServer _netServer;
        private NetClient _netClient;

        private bool _serverActive;

        private List<NetworkMessage> _incClientMsg;
        private List<NetworkMessage> _incServerMsg; 

        public NetworkImp()
        {
            _configuration = new NetPeerConfiguration("FUSEE");
            
            _netClient = new NetClient(_configuration);
            _netServer = new NetServer(_configuration);

            _serverActive = false;
        }

        public List<INetworkMsg> IncClientMsg { get; private set; }
        public List<INetworkMsg> IncServerMsg { get; private set; }

        public bool OpenConnection(string host, int port)
        {
            _netClient.Start();

            var hail = _netClient.CreateMessage("OpenConnection");
            var connection = _netClient.Connect(host, port, hail);

            return true;
        }

        //public bool 

        public void OnUpdateFrame()
        {
            NetIncomingMessage msg;
            
            if (_netClient.ConnectionStatus == NetConnectionStatus.Connected)
                while ((msg = _netClient.ReadMessage()) != null)
                {
                    var netMsg = new NetworkMessage((int) msg.MessageType, msg.ReadString());
                    _incClientMsg.Add(netMsg);

                    _netClient.Recycle(msg);
                }
            
            if (!_serverActive) return;

            while ((msg = _netServer.ReadMessage()) != null)
            {
                var netMsg = new NetworkMessage((int)msg.MessageType, msg.ReadString());
                _incServerMsg.Add(netMsg);

                _netServer.Recycle(msg);
            }

            
        }
    }
}
