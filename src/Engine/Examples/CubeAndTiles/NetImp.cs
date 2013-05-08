using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using ProtoBuf;

namespace Examples.CubeAndTiles
{
    internal static class NetImp
    {
        internal static void StartUpServer()
        {
            Network.Instance.Config.SysType = SysType.Server;
            Network.Instance.Config.Discovery = true;
            Network.Instance.Config.DefaultPort = 14242;

            Network.Instance.StartPeer();
        }

        internal static void StartUpClient()
        {
            Network.Instance.Config.SysType = SysType.Client;
            Network.Instance.Config.Discovery = true;
            Network.Instance.Config.ConnectOnDiscovery = true;
            Network.Instance.Config.DefaultPort = 54954;

            Network.Instance.StartPeer();
            Network.Instance.SendDiscoveryMessage(14242);
        }

        internal static void SendGameState()
        {
            if (Network.Instance.Status.Connected || Network.Instance.Config.SysType == SysType.Client)
            {
                var gameState = new GameState
                                    {
                                        GsCubeModelView = CubeAndTiles.ExampleLevel.RCube.ModelView,
                                        CubeState = CubeAndTiles.ExampleLevel.RCube.State,
                                        LvlState = CubeAndTiles.ExampleLevel.State
                                    };

                Network.Instance.SendMessage(gameState);
            }          
        }

        internal static void RecvGameState(GameState gameState)
        {
            //
            //    Network.Instance.SendMessage(_exampleLevel.RCube.ModelView);


        }

        internal static void HandleNetwork()
        {
            INetworkMsg msg;
            while ((msg = Network.Instance.IncomingMsg) != null)
            {
                if (msg.Type == MessageType.StatusChanged)
                {
                    Debug.WriteLine("StatusChange: " + msg.Status);

                    if (msg.Status == ConnectionStatus.Connected)
                        CubeAndTiles.ExampleLevel.CubeColor = new float3(0f, 1f, 0);
                }

                if (msg.Type == MessageType.Data)
                {
                    GameState msgObj;

                    using (var ms = new MemoryStream())
                    {
                        ms.Write(msg.Message, 0, msg.Message.Length);
                        ms.Position = 0;
                        msgObj = Serializer.Deserialize<GameState>(ms);
                    }

                    RecvGameState(msgObj);

                    Debug.WriteLine(msgObj);
                }

                if (msg.Type == MessageType.DiscoveryRequest)
                    Debug.WriteLine("DiscoveryRequest");
            }
        }
    }
}
