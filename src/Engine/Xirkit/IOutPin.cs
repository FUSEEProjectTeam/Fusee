using System;
namespace Fusee.Xirkit
{
    public interface IOutPin
    {
        Type GetPinType();
        string Member { get; }
        void Propagate();
        void Attach(IInPin other);
        void Detach(IInPin other);
    }
}
