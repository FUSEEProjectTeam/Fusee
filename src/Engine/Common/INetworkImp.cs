using System.Collections.Generic;

namespace Fusee.Engine
{
    public enum ConnectionType
    {
        CtPeer,
        CtClient,
        CtServer
    }

    public interface INetworkImp
    {
        List<INetworkMsg> IncClientMsg { get; }
        List<INetworkMsg> IncServerMsg { get; }

        bool OpenConnection(ConnectionType type, string host, int port);
        bool SendMessage(string msg);
        void OnUpdateFrame();
    }
}
