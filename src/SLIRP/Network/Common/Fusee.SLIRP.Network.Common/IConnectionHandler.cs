using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network
{
    internal interface IConnectionHandler
    {
        /// <summary>
        /// Initialize the Handler so that he is able to handle accepted connections and disconnected clients.
        /// </summary>
        /// <param name="requestHandler">The handler which accepts incoming connections.</param>
        /// <param name="handlingPattern">What kind of handling thread will be instantiated when a new client connects.</param>
        public void Init(IConnectionRequestHandler requestHandler);

        public void Shutdown();

        //public void HandleClient(IConnectionRequestHandler sender, Socket clientSocket);
        

    }
}
