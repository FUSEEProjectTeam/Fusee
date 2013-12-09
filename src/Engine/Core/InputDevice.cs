using System;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{
    public class InputDevice
    {
        private IInputDeviceImp _inputDeviceImp;

        public InputDevice(IInputDeviceImp inputDeviceImp)
        {
            _inputDeviceImp = inputDeviceImp;
        }
        

        public float getAxis(string axis)
        {
            return _inputDeviceImp.getAxis(axis);
        }

        public enum DeviceCategory
        {
            Mouse,
            Keyboard,
            GameController,
            Touch,
        }

    }
}
