using System.Collections.Generic;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    class InputDriverImp : IInputDriverImp
    {
        public List<DeviceInstance> Devices = new List<DeviceInstance>();

        public List<IInputDeviceImp> DeviceImps()
        {
            CreateDevices();
            var retList = new List<IInputDeviceImp>();
            foreach (DeviceInstance instance in Devices)
            {
                retList.Add(new InputDeviceImp(instance));
            }
            return retList;
        }

        public void CreateDevices()
        {
            var directInput = new DirectInput();
            var devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance deviceInstance in devices)
            {
                Devices.Add(deviceInstance);
            }
        }
    }
}
