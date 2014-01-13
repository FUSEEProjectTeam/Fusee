using System.Collections.Generic;
using Fusee.Engine;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    class InputDriverImp : IInputDriverImp
    {
        public List<GameController> _devices = new List<GameController>();

        public List<IInputDeviceImp> DeviceImps()
        {
            CreateDevices();
            var list = new List<IInputDeviceImp>();
            foreach (GameController controller in _devices)
            {
                list.Add(new InputDeviceImp(controller));
            }
            return list;
        }

        public void CreateDevices()
        {
            DirectInput directInput = new DirectInput();
            var devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance deviceInstance in devices)
            {
                _devices.Add(new GameController(deviceInstance));
            }
        }
    }
}
