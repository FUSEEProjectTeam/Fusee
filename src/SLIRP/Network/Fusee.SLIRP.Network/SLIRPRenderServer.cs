using Fusee.SLIRP.Network.Common;
using System.Net;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network
{
    public class SLIRPRenderServer : IServer
    {
        public const int BUFFERSIZE = 1024;
        public static ServerConnectionMetaData DefaultSLIRP = new ServerConnectionMetaData(IPAddress.Any, 1300, AddressFamily.InterNetwork, ProtocolType.Udp, new NetworkPackageMeta(BUFFERSIZE));

        private bool _isInitialized;
        private bool _isRunning;
        private IConnectionRequestHandler? _connRequestHandler;
        private IConnectionHandler? _clientHandler;
        private Thread? _connRequestHandlerThread;

        private ServerConnectionMetaData curServerConnectionMetaData;
        private ServerMetaData serverMetaData;

        private Socket? renderSocket;
        private int maxConnections = 10;

        public void Init(in ServerConnectionMetaData? metaData = null, int maxConnections = 10)
        {
            if (_isRunning || _isInitialized) return;

            _isInitialized = true;

            this.maxConnections = maxConnections;

            if (metaData != null)
            {
                curServerConnectionMetaData = metaData.Value;
            }
            else
            {
                curServerConnectionMetaData = DefaultSLIRP;
                //curServerConnectionMetaData.NetworkPackageMeta = new NetworkPackageMeta(BUFFERSIZE);
            }

            renderSocket = new Socket(curServerConnectionMetaData.AddressFamily, curServerConnectionMetaData.SocketType, curServerConnectionMetaData.ProtocolType);
            serverMetaData = new ServerMetaData(this, renderSocket, this.maxConnections, curServerConnectionMetaData);

            _connRequestHandler = new ConnectionRequestHandler(serverMetaData);

            _clientHandler = new ConnectionHandler<BasicPingPongHandler>();
            _clientHandler.Init(_connRequestHandler);
        }


        public void Shutdown()
        {
            if (!_isInitialized)
                return;

            if (_isRunning)
            {
                Console.WriteLine("Stop server before calling \"Shutdown()\".");
                return;
            }

            if(_connRequestHandler!= null)  
                _connRequestHandler.Shutdown();

            _isInitialized = false;
        }

        public void StartServer(in ServerConnectionMetaData? metaData = null)
        {
            if (!_isInitialized)
                Init(metaData);

            if (_isRunning)
            {
                Console.WriteLine("Server is already running.");
                return;
            }

            _isRunning = true;

            _connRequestHandlerThread = new Thread(new ThreadStart(_connRequestHandler.Run));

            _connRequestHandlerThread.Start();

        }

        public void StopServer()
        {
            if (!_isRunning)
            {
                Console.WriteLine("Server is not running.");
                return;
            }

            if (_connRequestHandlerThread != null)
                _connRequestHandlerThread.Interrupt();

            _isRunning = false;

        }

    }
}