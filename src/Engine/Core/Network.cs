using System;
using System.Collections.ObjectModel;
using System.Linq;
using JSIL.Meta;

namespace Fusee.Engine
{
    /// <summary>
    /// Core network object. Handles all connections by accessing underlying interface implementations.
    /// </summary>
    public class Network
    {
        #region Fields

        private static Network _instance;

        private INetworkImp _networkImp;
        internal INetworkImp NetworkImp
        {
            set { _networkImp = value; }
        }

        /// <summary>
        /// Gets or sets the status of the network. <see cref="NetStatusValues"/>.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public NetStatusValues Status
        {
            get { return _networkImp.Status; }
            set { _networkImp.Status = value; }
        }

        /// <summary>
        /// Gets or sets the configuration. <see cref="NetConfigValues"/>
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public NetConfigValues Config
        {
            get { return _networkImp.Config; }
            set { _networkImp.Config = value; }
        }

        /// <summary>
        /// Gets all connections of type <see cref="INetworkConnection"/>.
        /// </summary>
        /// <value>
        /// The connections.
        /// </value>
        public Collection<INetworkConnection> Connections
        {
            get { return new Collection<INetworkConnection>(_networkImp.Connections); }
        }

        /// <summary>
        /// Gets the local ip. Do not use this often due to performance reasons.
        /// </summary>
        /// <value>
        /// The local ip as a string, e.g. 127.0.0.1
        /// </value>
        public string LocalIP
        {
            get { return _networkImp.GetLocalIp(); }
        }

        /// <summary>
        /// Gets the incoming message's count.
        /// </summary>
        /// <value>
        /// The incoming message count.
        /// </value>
        public int IncomingMsgCount
        {
            get { return _networkImp.IncomingMsg.Count; }
        }

        /// <summary>
        /// Gets the incoming <see cref="INetworkMsg"/>.
        /// </summary>
        /// <value>
        /// The incoming message.
        /// </value>
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

        #endregion

        #region Events
        /// <summary>
        /// Occurs when [on connection update] occurs and passes the event over to the network interface implementation.
        /// </summary>
        public event ConnectionUpdateEvent OnConnectionUpdate
        {
            add { _networkImp.ConnectionUpdate += value; }
            remove { _networkImp.ConnectionUpdate -= value; }
        }

        #endregion

        #region Members

        /// <summary>
        /// Initializes the peer on default port 14242.
        /// </summary>
        public void StartPeer()
        {
            StartPeer(Config.DefaultPort);
        }

        /// <summary>
        /// Initializes the peer on specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        public void StartPeer(int port)
        {
            _networkImp.StartPeer(port);
        }

        /// <summary>
        /// Opens the connection on default port 14242 and specified host.
        /// </summary>
        /// <param name="host">The host (e.g. 127.0.0.1).</param>
        public void OpenConnection(string host)
        {
            OpenConnection(host, Config.DefaultPort);
        }

        /// <summary>
        /// Opens the connection on specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        public void OpenConnection(int port)
        {
            OpenConnection(String.Empty, port);
        }

        /// <summary>
        /// Opens the connection on specified port and host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public void OpenConnection(string host, int port)
        {
            _networkImp.OpenConnection(Config.SysType, host, port);
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void CloseConnection()
        {
            _networkImp.CloseConnection();
        }

        /// <summary>
        /// Sends the message with <see cref="MessageDelivery"/>.RealiableOrdered on channel 0.
        /// </summary>
        /// <param name="msg">The message in byte[].</param>
        /// <returns>True if the message arrived.</returns>
        public bool SendMessage(byte[] msg)
        {
            return SendMessage(msg, MessageDelivery.ReliableOrdered, 0);
        }

        /// <summary>
        /// Sends the message with options.
        /// </summary>
        /// <param name="msg">The message in byte[].</param>
        /// <param name="msgDelivery">The <see cref="MessageDelivery"/>.</param>
        /// <param name="channelID">The channel identifier.</param>
        /// <returns>True if the message arrived.</returns>
        public bool SendMessage(byte[] msg, MessageDelivery msgDelivery, int channelID)
        {
            return _networkImp.SendMessage(msg, msgDelivery, channelID);
        }


        /// <summary>
        /// Sends the discovery message on default port 14242.
        /// </summary>
        public void SendDiscoveryMessage()
        {
            _networkImp.SendDiscoveryMessage(Config.DefaultPort);
        }

        /// <summary>
        /// Sends the discovery message on specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        public void SendDiscoveryMessage(int port)
        {
            _networkImp.SendDiscoveryMessage(port);
        }

        internal void OnUpdateFrame()
        {
            _networkImp.OnUpdateFrame();
        }

        /// <summary>
        /// Closes all network connections.
        /// </summary>
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

        [JSExternal]
        private INetworkMsg FirstMessage()
        {
            return _networkImp.IncomingMsg.DefaultIfEmpty(null).First();
        }

        #endregion

    }
}