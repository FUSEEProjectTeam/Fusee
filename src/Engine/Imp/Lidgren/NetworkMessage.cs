using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    struct NetworkMessage : INetworkMsg
    {
        public string Message { get; private set; }
        public int ID { get; private set; }

        public NetworkMessage(int id, string msg) : this()
        {
            Message = msg;
            ID = id;
        }
    }
}
