using System;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{
    public class InputDevice
    {
        private IInputDeviceImp _inputDeviceImp;
        private string _name;

        public InputDevice(IInputDeviceImp inputDeviceImp)
        {
            _inputDeviceImp = inputDeviceImp;
        }

        

        public float getAxis(string axis)
        {
            return _inputDeviceImp.getAxis(axis);
        }

        
        public string Name
        {
            get { return _inputDeviceImp.Name; }
        }

        public int GetPressedButton()
        {
            return _inputDeviceImp.GetPressedButton();
        }

        public bool IsButtonDown(int buttonIndex)
        {
            return _inputDeviceImp.IsButtonDown(buttonIndex);

        }

        public bool IsButtonPressed(int buttonIndex)
        {
            return _inputDeviceImp.IsButtonPressed(buttonIndex);

        }

        public int GetButtonCount()
        {
            return _inputDeviceImp.GetButtonCount();
        }

        public String GetCategory()
        {
            return _inputDeviceImp.GetCategory();
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
