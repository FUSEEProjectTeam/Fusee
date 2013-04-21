using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine
{
    public class Network
    {
        private static Network _instance;

        private INetworkImp _networkImp;

        internal INetworkImp NetworkImp
        {
            set
            {
                Connected = false;
                _networkImp = value;
            }
        }

        public ConnectionType NetType;
        public bool Connected;

        public void OpenConnection(ConnectionType type, int port)
        {
            NetType = type;
            _networkImp.OpenConnection(type, "", port);
        }

        public void OpenConnection(ConnectionType type, string host, int port)
        {
            NetType = type;
            _networkImp.OpenConnection(type, host, port);
        }

        public int IncClientMsgCount
        {
            get { return _networkImp.IncClientMsg.Count; }
        }

        public int IncServerMsgCount
        {
            get { return _networkImp.IncServerMsg.Count; }
        }

        public INetworkMsg IncClientMsg
        {
            get
            {
                var msg = _networkImp.IncClientMsg.FirstOrDefault();
                _networkImp.IncClientMsg.RemoveAt(0);
                return msg;
            }
        }

        public INetworkMsg IncServerMsg
        {
            get
            {
                var msg = _networkImp.IncServerMsg.FirstOrDefault();
                _networkImp.IncServerMsg.RemoveAt(0);
                return msg;
            }
        }

        public void SendMessage(string msg)
        {
            var x = _networkImp.SendMessage(msg);
        }

        internal void OnUpdateFrame()
        {
            _networkImp.OnUpdateFrame();
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
