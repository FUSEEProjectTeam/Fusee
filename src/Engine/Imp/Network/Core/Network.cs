using Fusee.Base.Core;
using Fusee.Engine.Imp.Network.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fusee.Engine.Imp.Network.Core
{
    /// <summary>
    /// Core network object. Handles all connections by accessing underlying interface implementations.
    /// </summary>
    public class Network
    {
        #region Fields

        private static Network _instance;

        private INetworkImp _networkImp;

        public INetworkImp NetworkImp
        {
            set
            {
                if (value == null)
                {
                    Diagnostics.Warn("No Network implementation set. To enable Network functionality inject an appropriate implementation of INetworkImp in your platform specific application main module.");
                    _networkImp = new DummyNetworkImp();
                }
                else
                {
                    _networkImp = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the status of the network. <see cref="NetStatusValues"/>.
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
        /// Gets and sets the configuration. <see cref="NetConfigValues"/>
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

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when [on connection update] occurs and passes the event over to the network interface implementation.
        /// </summary>
        public event ConnectionUpdateEvent OnConnectionUpdate
        {
            add { _networkImp.ConnectionUpdate += value; }
            remove { _networkImp.ConnectionUpdate -= value; }
        }

        #endregion Events

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

        public void OnUpdateFrame()
        {
            _networkImp?.OnUpdateFrame();
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _instance = null;
        }

        /// <summary>
        /// Firsts the message.
        /// </summary>
        /// <returns></returns>
        private INetworkMsg FirstMessage()
        {
            return _networkImp.IncomingMsg.DefaultIfEmpty(null).First();
        }

        #endregion Members
    }

    /// <summary>
    /// Dummy implementation without functinoality
    /// </summary>
    internal class DummyNetworkImp : INetworkImp
    {
        /// <summary>
        /// Gets and sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public NetConfigValues Config { get; set; }

        /// <summary>
        /// Gets and sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public NetStatusValues Status { get; set; }

        public List<INetworkConnection> Connections { get; }

        /// <summary>
        /// Gets the local ip.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string GetLocalIp()
        {
            return "";
        }

        /// <summary>
        /// Gets the incoming MSG.
        /// </summary>
        /// <value>
        /// The incoming MSG.
        /// </value>
        public List<INetworkMsg> IncomingMsg { get; }

        /// <summary>
        /// Starts the peer.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void StartPeer(int port)
        {
        }

        /// <summary>
        /// Occurs when [connection update].
        /// </summary>
        public event ConnectionUpdateEvent ConnectionUpdate;

        public bool OpenConnection(SysType type, string host, int port)
        {
            return false;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CloseConnection()
        {
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="msgDelivery">The MSG delivery.</param>
        /// <param name="msgChannel">The MSG channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool SendMessage(byte[] msg, MessageDelivery msgDelivery, int msgChannel)
        {
            return false;
        }

        /// <summary>
        /// Sends the discovery message.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SendDiscoveryMessage(int port)
        {
        }

        /// <summary>
        /// Called when [update frame].
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void OnUpdateFrame()
        {
        }

        /// <summary>
        /// Closes the devices.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CloseDevices()
        {
        }
    }
}