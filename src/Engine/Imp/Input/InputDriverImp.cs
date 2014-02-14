using System.Collections.Generic;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    /// <summary>
    /// Slim DX specific implementation for the <see cref="IInputDriverImp"/>.
    /// </summary>
    class InputDriverImp : IInputDriverImp
    {
        public List<DeviceInstance> Devices = new List<DeviceInstance>();

        /// <summary>
        /// All SlimDX - compatible input devices are initialised and added to a List of the type <see cref="IInputDeviceImp"./>
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
    }
}
