using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Fusee.Engine
{
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

    public enum MessageDelivery
    {
        Unknown = 0,
        Unreliable = (byte) 1,
        UnreliableSequenced = (byte) 2,
        ReliableUnordered = (byte) 34,
        ReliableSequenced = (byte) 35,
        ReliableOrdered = (byte) 67,
    }

    public enum MsgDataTypes
    {
        Object = 0,
        Bytes = 1,
        Int = 2,
        Float = 3,
        String = 4,
    }

    public struct NetworkMsgType
    {
        public MsgDataTypes MsgType;
        public MessageDelivery MsgDelivery;
        public int MsgChannel;

        public byte[] ReadBytes;
        public int ReadInt;
        public float ReadFloat;
        public string ReadString;
    }

    public interface INetworkMsg
    {
        MessageType Type { get; }
        ConnectionStatus Status { get; }
        INetworkConnection Sender { get; }

        NetworkMsgType Message { get; }
    }
}
