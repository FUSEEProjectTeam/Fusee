using System;

namespace Fusee.Xirkit
{
    public delegate void ReceivedValueHandler(IInPin inPin, EventArgs args);
    public interface IInPin
    {
        string Member { get; }
        event ReceivedValueHandler ReceivedValue;
        Type GetPinType();
    }
}
