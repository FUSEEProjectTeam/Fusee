using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    struct NetworkMessage : INetworkMsg
    {
        public MessageType Type { get; private set; }
        public ConnectionStatus Status { get; private set; }

        public string Message { get; private set; }

        public NetworkMessage(MessageType type, string msg) : this()
        {
            Type = type;
            Status = 0;

            Message = msg;
        }

        public NetworkMessage(MessageType type, ConnectionStatus status)
            : this()
        {
            Type = type;
            Status = status;

            Message = status.ToString();
        }
    }
}
