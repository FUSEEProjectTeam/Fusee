// TODO: Comment stuff and remove #pragma
#pragma warning disable 1591

using System.Collections.Generic;
using System.Linq;
using JSIL.Meta;

namespace Fusee.Engine
{
    public class Network
    {
        private static Network _instance;

        private INetworkImp _networkImp;
        internal INetworkImp NetworkImp
        {
            set { _networkImp = value; }
        }

        public NetStatusValues Status
        {
            get { return _networkImp.Status; }
            set { _networkImp.Status = value; }
        }

        public NetConfigValues Config
        {
            get { return _networkImp.Config; }
            set { _networkImp.Config = value; }
        }

        public event ConnectionUpdateEvent OnConnectionUpdate
        {
            add { _networkImp.ConnectionUpdate += value; }
            remove { _networkImp.ConnectionUpdate -= value; }
        }

        public List<INetworkConnection> Connections
        {
            get { return _networkImp.Connections; }
        }

        public string LocalIP
        {
            get { return _networkImp.GetLocalIp(); }
        }

        // Start System //
        public void StartPeer()
        {
            StartPeer(Config.DefaultPort);
        }

        public void StartPeer(int port)
        {
            _networkImp.StartPeer(port);
        }

        // Connections //
        public void OpenConnection(string host)
        {
            OpenConnection(host, Config.DefaultPort);
        }

        public void OpenConnection(int port)
        {
            OpenConnection("", port);
        }

        public void OpenConnection(string host, int port)
        {
            _networkImp.OpenConnection(Config.SysType, host, port);
        }

        public void CloseConnection()
        {
            _networkImp.CloseConnection();
        }

        // Incoming Messages //
        public int IncomingMsgCount
        {
            get { return _networkImp.IncomingMsg.Count; }
        }

        [JSExternal]
        private INetworkMsg FirstMessage()
        {
            return _networkImp.IncomingMsg.DefaultIfEmpty(null).First();
        }

        public INetworkMsg IncomingMsg
        {
            get
            {
                var msg = FirstMessage();

                if (msg != null)
                    _networkImp.IncomingMsg.Remove(msg);

                return msg;
            }
        }

        // Send Messages //
        public bool SendMessage(string msg)
        {
            var data = new NetworkMsgType {MsgType = MsgDataTypes.String, ReadString = msg};
            return _networkImp.SendMessage(data);
        }

        public bool SendMessage(int msg)
        {
            var data = new NetworkMsgType {MsgType = MsgDataTypes.Int, ReadInt = msg};
            return _networkImp.SendMessage(data);
        }

        public bool SendMessage(byte[] msg)
        {
            var data = new NetworkMsgType {MsgType = MsgDataTypes.Bytes, ReadBytes = msg};
            return _networkImp.SendMessage(data);
        }

        public void SendDiscoveryMessage()
        {
            _networkImp.SendDiscoveryMessage(Config.DefaultPort);
        }

        public void SendDiscoveryMessage(int port)
        {
            _networkImp.SendDiscoveryMessage(port);
        }

        internal void OnUpdateFrame()
        {
            _networkImp.OnUpdateFrame();
        }

        public void CloseDevice()
        {
            _networkImp.CloseDevices();
        }

        /// <summary>
        /// Provides the Instance of the Network Class.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Network Instance
        {
            get { return _instance ?? (_instance = new Network()); }
        }
    }
}

#pragma warning restore 1591