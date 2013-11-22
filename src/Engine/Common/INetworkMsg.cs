using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Fusee.Engine
{

    /// <summary>
    /// Connection status enums.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// No connection = 0
        /// </summary>
        None,
        /// <summary>
        /// Connection initialization = 1
        /// </summary>
        InitiatedConnect,
        /// <summary>
        /// Initiation Received = 2
        /// </summary>
        ReceivedInitiation,
        /// <summary>
        /// Responded awaiting approval = 3
        /// </summary>
        RespondedAwaitingApproval,
        /// <summary>
        /// The responded connect(agree to accept connection) = 4
        /// </summary>
        RespondedConnect,
        /// <summary>
        /// Connection established = 5
        /// </summary>
        Connected,
        /// <summary>
        /// Initialization of Disconnection = 6
        /// </summary>
        Disconnecting,
        /// <summary>
        /// Disconnected = 7
        /// </summary>
        Disconnected,
    }

    /// <summary>
    /// Network transfer message type enums.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///  error = 0
        /// </summary>
        Error = 0,
        /// <summary>
        ///  status changed = 1
        /// </summary>
        StatusChanged = 1,
        /// <summary>
        ///  unconnected data = 2
        /// </summary>
        UnconnectedData = 2,
        /// <summary>
        ///  connection approval = 4
        /// </summary>
        ConnectionApproval = 4,
        /// <summary>
        ///  data = 8
        /// </summary>
        Data = 8,
        /// <summary>
        ///  receipt = 16
        /// </summary>
        Receipt = 16,
        /// <summary>
        ///  discovery request = 32
        /// </summary>
        DiscoveryRequest = 32,
        /// <summary>
        ///  discovery response = 64
        /// </summary>
        DiscoveryResponse = 64,
        /// <summary>
        ///  verbose debug message = 18
        /// </summary>
        VerboseDebugMessage = 128,
        /// <summary>
        ///  debug message = 256
        /// </summary>
        DebugMessage = 256,
        /// <summary>
        ///  warning message = 512
        /// </summary>
        WarningMessage = 512,
        /// <summary>
        ///  error message = 1024
        /// </summary>
        ErrorMessage = 1024,
        /// <summary>
        ///  nat introduction success = 2048
        /// </summary>
        NatIntroductionSuccess = 2048,
        /// <summary>
        ///  connection latency updated = 4096
        /// </summary>
        ConnectionLatencyUpdated = 4096,
    }

    /// <summary>
    /// Message delivery type enums.
    /// </summary>
    public enum MessageDelivery
    {
        /// <summary>
        /// Unknown type = 0
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Unreliable = 1, Packets can get lost and the order doesn't have to be correct.
        /// </summary>
        Unreliable = (byte) 1,
        /// <summary>
        /// Unrealiable Sequenced = 2, Packets can get lost, if an older(order) packet is received after a new one it(the older) is deleted.
        /// </summary>
        UnreliableSequenced = (byte) 2,
        /// <summary>
        /// Reliable Unordered = 3, Packets don't get lost andthe order doesnt have to be correct.
        /// </summary>
        ReliableUnordered = (byte) 34,
        /// <summary>
        /// Reliable Sequenced = 4, Packets don't get lost, if an older(order) packet is received after a new one it(the older) is deleted. 
        /// </summary>
        ReliableSequenced = (byte) 35,
        /// <summary>
        /// Realiable Ordered = 5, Packets don't get lost and are always transmitted in correct order.
        /// </summary>
        ReliableOrdered = (byte) 67,
    }

    /// <summary>
    /// Data type of the message data. 
    /// Data is always received as Byte[] and will be casted to appropriate enum type.
    /// </summary>
    public enum MsgDataTypes
    {
        /// <summary>
        ///  object = 0. Can be any object.
        /// </summary>
        Object = 0,
        /// <summary>
        ///  bytes = 1. This is an array.
        /// </summary>
        Bytes = 1,
        /// <summary>
        ///  int = 2. This is a single value.
        /// </summary>
        Int = 2,
        /// <summary>
        ///  float = 3. This is a single value.
        /// </summary>
        Float = 3,
        /// <summary>
        ///  string = 4.
        /// </summary>
        String = 4,
    }

    /// <summary>
    /// Message type container struct. (= Packet container) 
    /// </summary>
    public struct NetworkMsgType
    {
        /// <summary>
        /// The Message's data type (object, int, bytes, float, string).
        /// </summary>
        public MsgDataTypes MsgType;
        /// <summary>
        /// The Message's transmission type.
        /// </summary>
        public MessageDelivery MsgDelivery;
        /// <summary>
        /// The Message's channel.
        /// </summary>
        public int MsgChannel;

        /// <summary>
        /// The read bytes.
        /// </summary>
        public byte[] ReadBytes;
        /// <summary>
        /// The read int.
        /// </summary>
        public int ReadInt;
        /// <summary>
        /// The read float.
        /// </summary>
        public float ReadFloat;
        /// <summary>
        /// The read string.
        /// </summary>
        public string ReadString;
    }

    /// <summary>
    /// The Network message interface. 
    /// </summary>
    /// <remarks>
    /// Implementation Tasks: In order to use this with own implementations own enums for MessageType, Connectionstatus and Network message type are required.
    /// An implementation of <see cref="INetworkConnection"/> is required to identify the sender of the message.
    /// </remarks>
    public interface INetworkMsg
    {
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        MessageType Type { get; }
        /// <summary>
        /// Gets the status of the connection.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        ConnectionStatus Status { get; }
        /// <summary>
        /// Gets the sender of the message.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        INetworkConnection Sender { get; }

        /// <summary>
        /// Gets the message's data dontent.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        NetworkMsgType Message { get; }
    }
}
