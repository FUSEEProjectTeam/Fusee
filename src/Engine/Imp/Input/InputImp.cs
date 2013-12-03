using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    class InputImp : IInputImp
    {
        List<InputDevice> _devices = new List<InputDevice>();

        public void InitializeDevices()
        {
            DirectInput directInput = new DirectInput();
            var devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);


            foreach (DeviceInstance deviceInstance in devices)
            {
                System.Diagnostics.Debug.WriteLine(deviceInstance.ProductName);
                _devices.Add(new InputDevice(deviceInstance));

            }
        }

    }
}
