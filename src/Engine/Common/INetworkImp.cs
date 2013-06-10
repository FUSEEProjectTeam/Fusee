using System.Collections.Generic;

namespace Fusee.Engine
{
    public class NetConfigValues
    {
        public SysType SysType;
        public int DefaultPort;
        public bool Discovery;
        public bool ConnectOnDiscovery;
    }

    public class NetStatusValues
    {
        public bool Connected { get; set; }
        public ConnectionStatus LastStatus;
    }

    public enum SysType
    {
        Peer,
        Client,
        Server
    }

    public interface INetworkImp
    {
        NetConfigValues Config { get; set; }
        NetStatusValues Status { get; set; }
 
        List<INetworkMsg> IncomingMsg { get; }

        void StartPeer(int port);

        bool OpenConnection(SysType type, string host, int port);
        void CloseConnection();

        bool SendMessage(string msg);
        bool SendMessage(object data, bool compress);

        void SendDiscoveryMessage(int port);

        void OnUpdateFrame();

        void CloseDevices();
    }
}
