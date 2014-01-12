using System.Collections.Generic;

namespace Fusee.Engine
{
    /// <summary>
    /// Network configuration object.
    /// </summary>
    public class NetConfigValues
    {
        /// <summary>
        /// The system type. (Client, Server, Peer) 
        /// </summary>
        public SysType SysType;
        /// <summary>
        /// The default port (default: 14242)
        /// </summary>
        public int DefaultPort;
        /// <summary>
        /// The automatic discovery in net (default: false)
        /// </summary>
        public bool Discovery;
        /// <summary>
        /// The automatic connect on discovery. (default: false)
        /// </summary>
        public bool ConnectOnDiscovery;
        /// <summary>
        /// The discovery timeout. (default: 5000 ms)
        /// </summary>
        public int DiscoveryTimeout;
        /// <summary>
        /// The redirect packets. Currently this is not used. (default: false)
        /// </summary>
        public bool RedirectPackets;
    }

    /// <summary>
    /// Connection status object.
    /// </summary>
    public class NetStatusValues
    {
        /// <summary>
        /// Gets or sets a value indicating whether [connected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [connected]; otherwise, <c>false</c>.
        /// </value>
        public bool Connected { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [connecting].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [connecting]; otherwise, <c>false</c>.
        /// </value>
        public bool Connecting { get; set; }

        /// <summary>
        /// Feedback about the last status.
        /// </summary>
        public ConnectionStatus LastStatus;
    }

    /// <summary>
    /// Type of the system enums.
    /// </summary>
    public enum SysType
    {
        /// <summary>
        /// None = 0.
        /// </summary>
        None,
        /// <summary>
        /// Peer = 1.
        /// </summary>
        Peer,
        /// <summary>
        /// Client = 2.
        /// </summary>
        Client,
        /// <summary>
        /// Server = 3.
        /// </summary>
        Server
    }

    /// <summary>
    /// Event that gets triggered on connection change, establishing or disconnection.
    /// </summary>
    /// <param name="eStatus">The <see cref="ConnectionStatus"/>.</param>
    /// <param name="connection">The <see cref="INetworkConnection"/>.</param>
    public delegate void ConnectionUpdateEvent(ConnectionStatus eStatus, INetworkConnection connection);

    /// <summary>
    /// The core network connection and sending object interface.
    /// </summary>
    /// <remarks>
    /// Implementation Tasks: This object has to handle everything that is required for network communication. 
    /// Possible topics are Configuration, Status of connection and system as well as connection and message handling.
    /// </remarks>
    public interface INetworkImp
    {
        /// <summary>
        /// Gets or sets the configuration of the network. <see cref="NetConfigValues"/>
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        NetConfigValues Config { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        NetStatusValues Status { get; set; }

        /// <summary>
        /// Gets all the connections that this instance is handling.
        /// </summary>
        /// <value>
        /// The connections of type <see cref="INetworkConnection"/>.
        /// </value>
        List<INetworkConnection> Connections { get; }

        /// <summary>
        /// Gets the local ip. Do not use this often due to performance reasons.
        /// </summary>
        /// <returns>The local ip as a string.</returns>
        string GetLocalIp();

        /// <summary>
        /// Gets all the incoming Messages that are not yet viewed.
        /// </summary>
        /// <value>
        /// The incoming Messages of type <see cref="INetworkMsg"/>.
        /// </value>
        List<INetworkMsg> IncomingMsg { get; }

        /// <summary>
        /// Initialize system on port.
        /// </summary>
        /// <param name="port">The port.</param>
        void StartPeer(int port);

        /// <summary>
        /// Occurs when [connection update] is triggered. <see cref="ConnectionUpdateEvent"/>
        /// </summary>
        event ConnectionUpdateEvent ConnectionUpdate;
        /// <summary>
        /// Establish connection to port and host.
        /// </summary>
        /// <param name="type">The type of the system (only peer or client are supported).</param>
        /// <param name="host">The host. Example: 129.12.12.12</param>
        /// <param name="port">The port.</param>
        /// <returns>True if could open connection succesfully.</returns>
        bool OpenConnection(SysType type, string host, int port);
        /// <summary>
        /// Closes the connection.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Sends a message through a channel with defined message delivery type.
        /// </summary>
        /// <param name="msg">The Message in byte[].</param>
        /// <param name="msgDelivery">The <see cref="MessageDelivery"/>.</param>
        /// <param name="msgChannel">The message channel.</param>
        /// <returns>True if the message arrived.</returns>
        bool SendMessage(byte[] msg, MessageDelivery msgDelivery, int msgChannel);

        /// <summary>
        /// Sends the discovery message on defined port.
        /// </summary>
        /// <param name="port">The port.</param>
        void SendDiscoveryMessage(int port);

        /// <summary>
        /// Called when [update frame] occurs.
        /// </summary>
        void OnUpdateFrame();

        /// <summary>
        /// Closes all network connections.
        /// </summary>
        void CloseDevices();
    }
}
