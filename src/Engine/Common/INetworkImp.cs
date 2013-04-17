using System.Collections.Generic;

namespace Fusee.Engine
{
    public interface INetworkImp
    {
        List<INetworkMsg> IncClientMsg { get; }
        List<INetworkMsg> IncServerMsg { get; }

        bool OpenConnection(string host, int port);

        void OnUpdateFrame();
    }
}
