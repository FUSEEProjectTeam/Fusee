using System;
using System.Collections.Generic;
using Fusee.Engine.Common;
using SlimDX.DirectInput;

namespace Fusee.Engine.Imp.Input.Desktop
{
    /// <summary>
    /// Slim DX specific implementation for the <see cref="IInputDriverImp"/>.
    /// </summary>
    public class InputDriverImp : IInputDriverImp
    {
        /// <summary>
        /// The list of devices.
        /// </summary>
        //public List<DeviceInstance> Devices = new List<DeviceInstance>();

        event EventHandler<NewDeviceImpConnectedArgs> IInputDriverImp.NewDeviceConnected
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<DeviceImpDisconnectedArgs> IInputDriverImp.DeviceDisconnected
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// All SlimDX - compatible input devices are initialised and added to a List of the type <see cref="IInputDeviceImp"/>.
        /// </summary>
        /// <returns>A list containing all SlimDX - compatible input devices.</returns>
        public List<IInputDeviceImp> DeviceImps()
        {
            var directInput = new DirectInput();
            var devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance deviceInstance in devices)
            {
                Devices.Add(deviceInstance);
            }


            var retList = new List<IInputDeviceImp>();
            foreach (DeviceInstance instance in Devices)
            {
                retList.Add(new InputDeviceImp(instance));
            }
            return retList;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IInputDeviceImp> Devices { get; }
        public string DriverId { get; }
        public string DriverDesc { get; }
        public event EventHandler NewDeviceConnected;
        public event EventHandler DeviceDisconnected;
    }
}
