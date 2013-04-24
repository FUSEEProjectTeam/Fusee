// TODO: Comment stuff and remove #pragma
#pragma warning disable 1591

using System.Linq;

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

        // Start System //
        public void StartPeer()
        {
            StartPeer(Config.DefaultPort);
        }

        public void StartPeer(int port)
        {
            _networkImp.StartPeer(port);           
        }

        // Open Connections //
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

        public INetworkMsg IncomingMsg
        {
            get
            {
                var msg = _networkImp.IncomingMsg.DefaultIfEmpty(null).First();
                _networkImp.IncomingMsg.Remove(msg);
                return msg;
            }
        }

        // Send Messages //
        public bool SendMessage(string msg)
        {
            return _networkImp.SendMessage(msg);
        }

        public bool SendMessage(byte[] data)
        {
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