using System;

namespace Fusee.Engine
{
    public class InputDevice
    {
        private IInputDeviceImp _inputDeviceImp;

        public InputDevice(IInputDeviceImp inputDeviceImp)
        {
            _inputDeviceImp = inputDeviceImp;
        }

        public InputDevice()
        {

        }

        public float GetAxis(string axis)
        {
            switch (axis)
            {
                case "horizontal":
                    return _inputDeviceImp.GetXAxis();

                case "vertical":
                    return _inputDeviceImp.GetYAxis();

                case "z":
                    return _inputDeviceImp.GetZAxis();
                //TODO implement more axis

                default:
                    return 0.0f;
            }
        }

        
        public string Name()
        {
            return _inputDeviceImp.GetName();
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
