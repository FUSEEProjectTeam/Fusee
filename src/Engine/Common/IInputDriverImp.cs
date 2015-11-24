using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// The InputDriver is used to manage different implementations of input devices.
    /// As different input devices require different implementations, this is necessary to
    /// have only one interface. So one InputDriver can hold multiple instances of one specific implementation.
    /// </summary>
    public interface IInputDriverImp
    {
        /// <summary>
        /// Retrieves a list of device implementations ("drivers").
        /// </summary>
        /// <returns>The list of devices.</returns>
        List<IInputDeviceImp> DeviceImps();
    }
}
