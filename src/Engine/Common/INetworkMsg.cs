using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public enum MessageType
    {
        Error = 0,
        StatusChanged = 1,
        UnconnectedData = 2,
        ConnectionApproval = 4,
        Data = 8,
        Receipt = 16,
        DiscoveryRequest = 32,
        DiscoveryResponse = 64,
        VerboseDebugMessage = 128,
        DebugMessage = 256,
        WarningMessage = 512,
        ErrorMessage = 1024,
        NatIntroductionSuccess = 2048,
        ConnectionLatencyUpdated = 4096,
    }

    public enum ConnectionStatus
    {
        None,
        InitiatedConnect,
        ReceivedInitiation,
        RespondedAwaitingApproval,
        RespondedConnect,
        Connected,
        Disconnecting,
        Disconnected,
    }

    public interface INetworkMsg
    {
        MessageType Type { get; }
        ConnectionStatus Status { get; }

        dynamic Message { get; }
    }
}
