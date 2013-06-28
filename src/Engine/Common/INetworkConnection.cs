namespace Fusee.Engine
{
    public interface INetworkConnection
    {
        float RoundtripTime { get; }

        void Disconnect();
        void Disconnect(string byeMessage);
    }
}
