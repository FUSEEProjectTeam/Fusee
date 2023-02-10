using Fusee.SLIRP.Network.Common;
using Fusee.SLIRP.Network.Server.Common;
using System.Net;
using System.Net.Sockets;

namespace Fusee.SLIRP.Network.Server.Examples
{
    public class BasicServer : IServer
    {
        public const int BUFFERSIZE = 1024;
        public static ServerConnectionMetaData DefaultSLIRP = new ServerConnectionMetaData(IPAddress.Any, 1300, AddressFamily.InterNetwork, ProtocolType.Udp, SocketType.Stream, new NetworkPackageMeta(BUFFERSIZE));

        private bool _isInitialized;
        private bool _isRunning;
        private IConnectionRequestHandler? _connRequestHandler;
        private IConnectionHandler? _connectionHandler;
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

            _connectionHandler = new ConnectionHandler<BasicPingPongHandling>();
            _connRequestHandler = new ConnectionRequestHandler();

            _connectionHandler.Init(curServerConnectionMetaData.NetworkPackageMeta);
            _connRequestHandler.Init(serverMetaData, _connectionHandler);

            _connRequestHandlerThread = new Thread(new ThreadStart(_connRequestHandler.Run));
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

            if (_connRequestHandler != null)
            {
                _connRequestHandler.Shutdown();
            }

            if(_connRequestHandlerThread.ThreadState == ThreadState.Running)
            {
                try
                {
                    _connRequestHandlerThread.Abort();
                }catch(Exception e)
                {
                    Console.WriteLine("There was a problem aborting request handler thread!" + e.Message);
                }
            }



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

            _connectionHandler.StartHandling();

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

            //if the server is stopped, all running connection handlings
            //should shutdown. 
            if (_connectionHandler != null)
            {
                _connectionHandler.Shutdown();
            }

            _isRunning = false;

        }

    }
}