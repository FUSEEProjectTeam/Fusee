using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Server.Common
{

    /// <summary>
    /// Interface for connection handlers who should notice connections and disconnections.
    /// </summary>
    public interface IConnectionHandler : IConnectHandler, IDisconnectHandler
    {

        public bool IsRunning { get; }

        /// <summary>
        /// Initialize the Handler so that he is able to handle accepted connections and disconnected clients.
        /// </summary>
        /// <param name="handlingPattern">What kind of handling thread will be instantiated when a new client connects.</param>
        public void Init(NetworkPackageMeta networkPackageMeta);

        public void StartHandling();

        public void StopHandling();

        public void Shutdown();

    }
}
